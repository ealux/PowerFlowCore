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
            Grid gridReserve = grid.DeepCopy(); // Reserve initial grid

            var Um = U_initial.Map(x => x.Magnitude).ToArray(); // Input voltages magnitude vector
            var ph = U_initial.Map(x => x.Phase).ToArray();     // Input voltages phase vector

            var Uold = Vector<Complex>.Build.Dense(U_initial.Count); // Vector for voltages on previous iteration

            // Fill U vector with initial values
            var U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u, angle) => Complex.FromPolarCoordinates(u, angle)));
            // Fill Uold vector with initial U values
            Uold = Vector<Complex>.Build.DenseOfEnumerable(U);

            // MAIN CYCLE
            for (int i = 0; i < options.IterationsCount; i++)
            {
                var dPQ = Resuduals_Polar(grid, ref U);   //Power residuals                       
                var J   = Jacobian_Polar(grid, ref U);    //Jacoby matrix                

                //Calculation of increments
                var dx = J.Solve(-dPQ);

                //Voltage residual
                var Udx = Vector<double>.Build.Dense(grid.PQ_Count);

                //Update voltage levels
                for (int j = 0; j < (grid.PQ_Count + grid.PV_Count); j++) 
                    ph[j] -= dx[j];

                // Fill residuals vector
                for (int j = 0; j < grid.PQ_Count; j++) 
                { 
                    Um[j] -= dx[j + grid.PQ_Count];             // Fill vector of voltage magnitude increments
                    Udx[j] = Math.Abs(dx[j + grid.PQ_Count]);   // Fill vector of voltage phase residuals
                }

                Uold = U.Clone();
                U    = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u, angle) => Complex.FromPolarCoordinates(u, angle)));

                #region [CHECKS]               

                // Checks               
                // Power residual check
                if (dPQ.InfinityNorm() <= options.Accuracy)
                {
                    Console.WriteLine($"N-R iterations: {i}" + $" of {options.IterationsCount} (Power residual criteria)");
                    //Update voltage levels
                    for (int n = 0; n < grid.Nodes.Count; n++) 
                        grid.Nodes[n].U = U[n];
                    U.CopyTo(grid.Ucalc);

                    return grid;
                }
                // Voltage convergence check
                if (Udx.InfinityNorm() <= options.VoltageConvergence)
                {
                    Console.WriteLine($"N-R iterations: {i}" + $" of {options.IterationsCount} (Voltage convergence criteria)");
                    //Update voltage levels
                    for (int n = 0; n < grid.Nodes.Count; n++) 
                        grid.Nodes[n].U = U[n];
                    U.CopyTo(grid.Ucalc);

                    return grid;
                }

                #endregion
            }

            //Update voltage levels
            for (int n = 0; n < grid.Nodes.Count; n++) 
                grid.Nodes[n].U = U[n];
            U.CopyTo(grid.Ucalc);

            return grid;

        }


        /// <summary>
        /// Jacobian matrix calculation on each iteration
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U">Present <seealso cref="Vector{Complex}"/>voltage value</param>
        /// <returns>Jacobian <seealso cref="Matrix{double}"/></returns>
        private static Matrix<double> Jacobian_Polar(Grid grid,
                                                     ref Vector<Complex> U)
        {
            var dim = grid.PQ_Count + grid.PV_Count;

            var G = grid.Y.Real();
            var B = grid.Y.Imaginary();

            var Um  = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            var P_Delta = Matrix<double>.Build.Dense(dim, dim);
            var P_V = Matrix<double>.Build.Dense(dim, grid.PQ_Count);
            var Q_Delta = Matrix<double>.Build.Dense(grid.PQ_Count, dim);
            var Q_V = Matrix<double>.Build.Dense(grid.PQ_Count, grid.PQ_Count);

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
                        P_Delta[i, j] = -grid.Y[i, j].Magnitude * 
                                        Math.Pow(Um[i], 2) * 
                                        Math.Sin(grid.Y[i, j].Phase);

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
                        P_V[i, j] = Um[i] * grid.Y[i, j].Magnitude * 
                                    Math.Cos(grid.Y[i, j].Phase);

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
                        Q_Delta[i, j] = -grid.Y[i, j].Magnitude * 
                                        Math.Pow(Um[i], 2) * 
                                        Math.Cos(grid.Y[i, j].Phase);

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
                        Q_V[i, j] = -Um[i] * grid.Y[i, j].Magnitude * 
                                    Math.Sin(grid.Y[i, j].Phase);

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
        /// Power residuals vector
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U">Present <seealso cref="Vector{Complex}"/>voltage value</param>
        /// <returns> Power residuals <seealso cref="Vector{Complex}"/></returns>
        private static Vector<double> Resuduals_Polar(Grid grid,
                                                     ref Vector<Complex> U)
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
