using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using Complex = System.Numerics.Complex;
using PowerFlowCore.Data;

namespace PowerFlowCore.Solvers
{

    public static partial class Solvers
    {
        /// <summary>
        /// Newton-Raphson Solver
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U_initial">Initial voltage <seealso cref="Vector{Complex}"/></param>
        /// <param name="options"><seealso cref="CalculationOptions"/> for calculus</param>
        /// <returns><seealso cref="Grid"/> with calculated voltages</returns>
        public static Grid SolverNR(this Grid grid,
                                         Vector<Complex> U_initial,
                                         CalculationOptions options)
        {
            // Reserve initial grid
            Grid gridReserve = grid.DeepCopy(); 

            // U vectors 
            var U = Vector<Complex>.Build.DenseOfEnumerable(U_initial);
            var Uold = Vector<Complex>.Build.DenseOfEnumerable(U_initial);

            // Power residuals, voltage and angle mismatch
            Vector<double> dPQ, dx;

            // Save PV nodes being transformed
            List<int> gensOnLimits = new List<int>();

            // Voltage estimation
            for (int i = 0; i < options.IterationsCount; i++)
            {
                // Use of N-R technique to estimate voltage
                // Also return dPQ and dx for stoping criteria
                U = getdU(grid, ref U, out Uold, out dPQ, out dx);

                var dx_norm = dx.SubVector(grid.PQ_Count + grid.PV_Count, grid.PQ_Count).PointwiseAbs().InfinityNorm();

                bool isOnLimits, isOouOfLimits;

                InspectPV(grid, ref U, ref gensOnLimits, out isOnLimits, out isOouOfLimits);

                if (isOnLimits | isOouOfLimits)
                    grid.InitParameters(grid.Nodes, grid.Branches);
                U = grid.Ucalc.Clone();


                //Power residual
                if (dPQ.InfinityNorm() <= options.Accuracy)
                {
                    Console.WriteLine($"N-R iterations: {i}" + $" of {options.IterationsCount} (Power residual criteria)");
                    for (int n = 0; n < grid.Nodes.Count; n++)
                        grid.Nodes[n].U = U[n];
                    break;
                }
                // Voltage convergence
                if (dx_norm <= options.VoltageConvergence)
                {
                    Console.WriteLine($"N-R iterations: {i}" + $" of {options.IterationsCount} (Voltage convergence criteria)");
                    for (int n = 0; n < grid.Nodes.Count; n++)
                        grid.Nodes[n].U = U[n];
                    break;
                }

            }

            for (int i = 0; i < gensOnLimits.Count; i++)
            {
                grid.Nodes.First(n => n.Num == gensOnLimits[i]).Type = NodeType.PV;
            }

            gensOnLimits.Clear();
            grid.InitParameters(grid.Nodes, grid.Branches);
            U = grid.Ucalc.Clone();

            //Update voltage levels
            for (int n = 0; n < grid.Nodes.Count; n++)
                grid.Nodes[n].U = U[n];

            return grid;
        }



        private static void InspectPV(Grid grid, 
                                     ref Vector<Complex> U, 
                                     ref List<int> gensOnLimits, 
                                     out bool isOnLimits, 
                                     out bool isOutOfLimits)
        {
            // Constraints flag
            isOnLimits = false;
            isOutOfLimits = false;

            for (int nodeNum = 0; nodeNum < grid.Nodes.Count; nodeNum++)
            {
                if (grid.Nodes[nodeNum].Type == NodeType.PV | gensOnLimits.Contains(grid.Nodes[nodeNum].Num))
                {
                    // New Q element
                    var Q_new = 0.0;

                    // Build new Q element
                    for (int j = 0; j < grid.Nodes.Count; j++)
                        Q_new -= U[nodeNum].Magnitude * U[j].Magnitude * grid.Y[nodeNum, j].Magnitude *
                                 Math.Sin(grid.Y[nodeNum, j].Phase + U[j].Phase - U[nodeNum].Phase);

                    Q_new = Q_new + grid.Nodes[nodeNum].S_load.Imaginary;

                    // Q conststraints
                    var qmin = grid.Nodes[nodeNum].Q_min;
                    var qmax = grid.Nodes[nodeNum].Q_max;

                    // Check limits conditions
                    if (Q_new <= qmin)
                    {
                        grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, qmin);
                        if (!gensOnLimits.Contains(grid.Nodes[nodeNum].Num))
                        {
                            gensOnLimits.Add(grid.Nodes[nodeNum].Num);
                            grid.Nodes[nodeNum].Type = NodeType.PQ;
                            isOutOfLimits = true;
                        }

                    }
                    else if (Q_new >= qmax)
                    {
                        grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, qmax);
                        if (!gensOnLimits.Contains(grid.Nodes[nodeNum].Num))
                        {
                            gensOnLimits.Add(grid.Nodes[nodeNum].Num);
                            grid.Nodes[nodeNum].Type = NodeType.PQ;
                            isOutOfLimits = true;
                        }
                    }
                    else
                    {
                        grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, Q_new);
                        if (gensOnLimits.Contains(grid.Nodes[nodeNum].Num))
                        {
                            gensOnLimits.Remove(grid.Nodes[nodeNum].Num);
                            grid.Nodes[nodeNum].Type = NodeType.PV;
                            grid.Nodes[nodeNum].U = Complex.FromPolarCoordinates(grid.Nodes[nodeNum].Vpre, grid.Nodes[nodeNum].U.Phase);
                            isOnLimits = true;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Calculate U voltage <seealso cref="Vector{Complex}"/> by Newton-Raphson technique
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U">Present <seealso cref="Vector{Complex}"/>voltage value</param>
        /// <param name="Uold">Previous iteration <seealso cref="Vector{Complex}"/>voltage value</param>
        /// <param name="dPQ">Power residuals <seealso cref="Vector{Complex}"/> value</param>
        /// <param name="dx">Mismatch voltage and angle <seealso cref="Vector{Complex}"/> value</param>
        /// <returns>Voltage approximation <seealso cref="Vector{Complex}"/></returns>
        private static Vector<Complex> getdU(Grid grid, 
                                             ref Vector<Complex> U, 
                                             out Vector<Complex> Uold, 
                                             out Vector<double> dPQ, 
                                             out Vector<double> dx)
        {
            var Um = U.Map(x => x.Magnitude).ToArray(); // Input voltages magnitude vector
            var ph = U.Map(x => x.Phase).ToArray();     // Input voltages phase vector

            dPQ   = Resuduals_Polar(grid, U);
            var J = Jacobian_Polar(grid, U);    //Jacoby matrix                

            // Calculation of increments
            dx = J.Solve(-dPQ);

            // Voltage residual
            var dU = Vector<double>.Build.Dense(grid.PQ_Count);

            // Update angles
            for (int j = 0; j < (grid.PQ_Count + grid.PV_Count); j++)
                ph[j] -= dx[j];

            // Update magnitudes
            for (int j = 0; j < grid.PQ_Count; j++)
                Um[j] -= dx[grid.PQ_Count + grid.PV_Count + j];

            // Save old and calc new voltages
            Uold = U.Clone();
            U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u, angle) => Complex.FromPolarCoordinates(u, angle)));

            // Set new value to Nodes
            for (int n = 0; n < grid.Nodes.Count; n++)
                grid.Nodes[n].U = U[n];

            return U;
        }


        /// <summary>
        /// Jacobian <seealso cref="Matrix{Complex}"/> calculation on each iteration
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U">Present <seealso cref="Vector{Complex}"/>voltage value</param>
        /// <returns>Jacobian <seealso cref="Matrix{double}"/></returns>
        private static Matrix<double> Jacobian_Polar(Grid grid,
                                                     Vector<Complex> U)
        {
            var dim = grid.PQ_Count + grid.PV_Count;

            var G = grid.Y.Real();
            var B = grid.Y.Imaginary();

            var Um  = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            var P_Delta = Matrix<double>.Build.Dense(dim, dim);
            var P_V     = Matrix<double>.Build.Dense(dim, grid.PQ_Count);
            var Q_Delta = Matrix<double>.Build.Dense(grid.PQ_Count, dim);
            var Q_V     = Matrix<double>.Build.Dense(grid.PQ_Count, grid.PQ_Count);

            //P_Delta
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (i != j) 
                        P_Delta[i, j] = -Um[i] * Um[j] * grid.Y[i, j].Magnitude * 
                                        Math.Sin(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        // Component to DELETE from sum (i==j)
                        P_Delta[i, j] = -grid.Y[i, j].Magnitude * 
                                        Math.Pow(Um[i], 2) * 
                                        Math.Sin(grid.Y[i, j].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++) 
                            P_Delta[i, j] += Um[i] * Um[k] * grid.Y[i, k].Magnitude * 
                                             Math.Sin(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }
            }

            //P_V
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < grid.PQ_Count; j++)
                {
                    if (i != j) 
                        P_V[i, j] = Um[i] * grid.Y[i, j].Magnitude * 
                                    Math.Cos(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        // Component with deleted part (one of two) (i==j)
                        P_V[i, j] = Um[i] * grid.Y[i, j].Magnitude * 
                                    Math.Cos(grid.Y[i, j].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++) 
                            P_V[i, j] += Um[k] * grid.Y[i, k].Magnitude * 
                                         Math.Cos(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }
            }

            //Q_Delta
            for (int i = 0; i < grid.PQ_Count; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (i != j) 
                        Q_Delta[i, j] = -Um[i] * Um[j] * grid.Y[i, j].Magnitude * 
                                        Math.Cos(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        // Component to DELETE from sum (i==j)
                        Q_Delta[i, j] = -grid.Y[i, j].Magnitude * 
                                        Math.Pow(Um[i], 2) * 
                                        Math.Cos(grid.Y[i, j].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++) 
                            Q_Delta[i, j] += Um[i] * Um[k] * grid.Y[i, k].Magnitude * 
                                             Math.Cos(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }
            }

            //Q_V
            for (int i = 0; i < grid.PQ_Count; i++)
            {
                for (int j = 0; j < grid.PQ_Count; j++)
                {
                    if (i != j) 
                        Q_V[i, j] = -Um[i] * grid.Y[i, j].Magnitude * 
                                    Math.Sin(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        // Component with deleted part (one of two) (i==j)
                        Q_V[i, j] = -Um[i] * grid.Y[i, j].Magnitude * 
                                    Math.Sin(grid.Y[i, j].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++) 
                            Q_V[i, j] -= Um[k] * grid.Y[i, k].Magnitude * 
                                         Math.Sin(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }

            }

            // Form result matrix
            var res = Matrix<double>.Build.DenseOfMatrixArray(new[,] { { P_Delta, P_V }, { Q_Delta, Q_V } });

            return res;
        }


        /// <summary>
        /// Power residuals <seealso cref="Vector{Complex}"/>
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U">Present <seealso cref="Vector{Complex}"/>voltage value</param>
        /// <returns> Power residuals <seealso cref="Vector{Complex}"/></returns>
        private static Vector<double> Resuduals_Polar(Grid grid,
                                                      Vector<Complex> U)
        {
            var dim = grid.PQ_Count + grid.PV_Count;

            var G = grid.Y.Real();
            var B = grid.Y.Imaginary();

            var Um = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            var dP = Vector<double>.Build.Dense(dim);
            var dQ = Vector<double>.Build.Dense(grid.PQ_Count);


            //dP
            for (int i = 0; i < dim; i++)
            {
                dP[i] = grid.S[i].Real;
                for (int j = 0; j < dim + grid.Slack_Count; j++) 
                    dP[i] -= Um[i] * Um[j] * grid.Y[i, j].Magnitude * 
                             Math.Cos(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
            }

            //dQ
            for (int i = 0; i < grid.PQ_Count; i++)
            {
                dQ[i] = grid.S[i].Imaginary;
                for (int j = 0; j < dim + grid.Slack_Count; j++) 
                    dQ[i] -= -Um[i] * Um[j] * grid.Y[i, j].Magnitude * 
                              Math.Sin(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
            }

            // Form result vector
            var res = Vector<double>.Build.DenseOfEnumerable(dP.Concat(dQ));

            return res;
        }
    }
}
