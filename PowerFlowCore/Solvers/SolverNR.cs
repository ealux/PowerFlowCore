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

            // Save PV nodes being transformed
            List<int> gensOnLimits = new List<int>();
            
            bool crossLimits = true;
            bool success     = false;
            int  iteration   = 0;
            int  iter        = 0;

            // U vectors 
            var U = Vector<Complex>.Build.DenseOfEnumerable(U_initial);

            while (crossLimits)
            {
                // Calculate power flow
                (U, success, iteration) = NewtonRaphson(grid, U, options);

                // Increment iteration count
                iter += iteration + 1;

                // Inspect PV
                InspectPV_Classic(grid, ref U, ref gensOnLimits, out crossLimits);

                // Rebuild scheme if needed
                if (crossLimits)
                    grid.InitParameters(grid.Nodes, grid.Branches);

                // Set up voltages after rebuilding
                U = grid.Ucalc.Clone();
            }

            // !!!!!
            // TODO:
            // 1. (Not)Success logic
            // 2. Voltage restrictions
            // 3. Angle restrictions in branches
            // !!!!!

            // Total iteration cost
            Console.WriteLine($"Iterations total: {iter}");

            // Nodes convert back
            for (int i = 0; i < gensOnLimits.Count; i++) 
                grid.Nodes.First(n => n.Num == gensOnLimits[i]).Type = NodeType.PV;
            gensOnLimits.Clear();

            //Re - building scheme back
            grid.InitParameters(grid.Nodes, grid.Branches);

            return grid;
        }



        /// <summary>
        /// Calculate Voltage <seealso cref="Vector{Complex}"/> by Newton-Raphson technique
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U_initial">Present voltage <seealso cref="Vector{Complex}"/> to start from</param>
        /// <param name="options"><seealso cref="CalculationOptions"/> for calculus</param>
        /// <returns>Return <seealso cref="Tuple{Vector{Complex}, bool, int}"/> Voltage, successfull indicator and iteration count</returns>
        private static (Vector<Complex>, bool success, int iter) NewtonRaphson(Grid grid,
                                                                               Vector<Complex> U_initial,
                                                                               CalculationOptions options)
        {
            // U vectors 
            var U    = Vector<Complex>.Build.DenseOfEnumerable(U_initial);
            var Uold = Vector<Complex>.Build.DenseOfEnumerable(U_initial);

            // Iter number
            int iter = 0;

            // Convergence
            bool success = false;

            // Voltage estimation
            while (iter < options.IterationsCount)
            {
                // Input voltage magnitude and phase vectors
                var Um = U.Map(x => x.Magnitude).ToArray(); 
                var ph = U.Map(x => x.Phase).ToArray();

                Vector<double> dPQ  = Resuduals_Polar(grid, U);     // Residuals vector 
                Matrix<double> J    = Jacobian_Polar(grid, U);    // Jacoby matrix                

                // Calculation of increments
                Vector<double> dx = J.Solve(-dPQ);

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


                //Power residual stop criteria
                if (dPQ.InfinityNorm() <= options.Accuracy)
                {
                    Console.WriteLine($"N-R iterations: {iter}" + $" of {options.IterationsCount} (Power residual criteria)");
                    success = true;
                    break;
                }
                // Voltage convergence stop criteria
                if (dx.SubVector(grid.PQ_Count + grid.PV_Count, grid.PQ_Count).InfinityNorm() <= options.VoltageConvergence)
                {
                    Console.WriteLine($"N-R iterations: {iter}" + $" of {options.IterationsCount} (Voltage convergence criteria)");
                    success = true;
                    break;
                }

                // Next iteration
                iter++; 
            }

            if(iter == options.IterationsCount)
            {
                // Iterations convergence
                Console.WriteLine($"N-R iterations: {options.IterationsCount}" + $" of {options.IterationsCount} (Iterations criteria)");
                success = false;
            }

            return (U, success, iter);
        }


        /// <summary>
        /// Logic on PV busses inspection for Q limits (switch-bus)
        /// </summary>
        /// <param name="grid">Input <seealso cref="Grid"/> for calculus</param>
        /// <param name="U">Actual voltage <seealso cref="Vector{Complex}"/></param>
        /// <param name="gensOnLimits"><seealso cref="List{int}"/> of PV buses on limits</param>
        /// <param name="crossLimits">PV bus switching flag</param>
        private static void InspectPV_Classic(Grid grid,
                                             ref Vector<Complex> U,
                                             ref List<int> gensOnLimits,
                                             out bool crossLimits)
        {
            // Constraints flag
            crossLimits = false;

            for (int nodeNum = 0; nodeNum < grid.Nodes.Count; nodeNum++)
            {
                if (grid.Nodes[nodeNum].Type == NodeType.PV | gensOnLimits.Contains(grid.Nodes[nodeNum].Num))
                {
                    // Current node
                    var Node = grid.Nodes[nodeNum];

                    #region [Calc new Q value]

                    // New Q element
                    var Q_new = Node.S_load.Imaginary;
                    // Build new Q element
                    for (int j = 0; j < grid.Nodes.Count; j++)
                        Q_new -= U[nodeNum].Magnitude * U[j].Magnitude * grid.Y[nodeNum, j].Magnitude *
                                 Math.Sin(grid.Y[nodeNum, j].Phase + U[j].Phase - U[nodeNum].Phase);

                    // Get current Voltage magnitude and Q conststraints at node
                    var Vcurr = U[nodeNum].Magnitude;
                    var qmin = Node.Q_min;
                    var qmax = Node.Q_max;

                    #endregion                    

                    // On PQ state
                    if (Node.Type == NodeType.PQ)
                    {
                        // If node is PQ and on LOWER LIMIT
                        if (Node.S_gen.Imaginary == qmin)
                        {
                            // Q == Qmin, but V < Vpre
                            if (Vcurr < Node.Vpre)
                            {
                                Node.S_gen = new Complex(Node.S_gen.Real, Q_new);
                                Node.Type = NodeType.PV;
                                Node.U = Complex.FromPolarCoordinates(Node.Vpre, Node.U.Phase);
                                if (gensOnLimits.Contains(Node.Num))
                                    gensOnLimits.Remove(Node.Num);
                                crossLimits = true;
                            }
                        }
                        // If node is PQ and on UPPER LIMIT
                        else if (Node.S_gen.Imaginary == qmax)
                        {
                            // Q == Qmax, but V > Vpre
                            if (Vcurr > Node.Vpre)
                            {
                                Node.S_gen = new Complex(Node.S_gen.Real, Q_new);
                                Node.Type = NodeType.PV;
                                Node.U = Complex.FromPolarCoordinates(Node.Vpre, Node.U.Phase);
                                if (gensOnLimits.Contains(Node.Num))
                                    gensOnLimits.Remove(Node.Num);
                                crossLimits = true;
                            }
                        }
                    }
                    // On PV state
                    else if (Node.Type == NodeType.PV)
                        crossLimits = ChangeQgen(gensOnLimits, Node, Q_new, qmin, qmax);

                }
            }
        }

        /// <summary>
        /// Change PV bus type by new Qgen value
        /// </summary>
        /// <param name="gensOnLimits"><seealso cref="List{int}"/> of PV buses on limits</param>
        /// <param name="Node"><seealso cref="INode"/> object PV bus depends on</param>
        /// <param name="Qgen">Calculatad Qgen value</param>
        /// <param name="qmin">PV bus lower Q limit</param>
        /// <param name="qmax">PV bus upper Q limit</param>
        /// <returns></returns>
        private static bool ChangeQgen(List<int> gensOnLimits,
                                        INode Node,
                                        double Qgen,
                                        double qmin,
                                        double qmax)
        {
            var crossLimits = false;

            // Check limits conditions
            if (Qgen <= qmin)
            {
                // If Qgen on LOWER limit but voltage BIGGER then Vpre 
                Node.S_gen = new Complex(Node.S_gen.Real, qmin);
                Node.Type = NodeType.PQ;
                if (!gensOnLimits.Contains(Node.Num))
                {
                    gensOnLimits.Add(Node.Num);
                    crossLimits = true;
                }
            }
            else if (Qgen >= qmax)
            {
                // If Qgen on UPPER limit but voltage LESS then Vpre                        
                Node.S_gen = new Complex(Node.S_gen.Real, qmax);
                Node.Type = NodeType.PQ;
                if (!gensOnLimits.Contains(Node.Num))
                {
                    gensOnLimits.Add(Node.Num);
                    crossLimits = true;
                }
            }
            else
            {
                // Still PV 
                Node.S_gen = new Complex(Node.S_gen.Real, Qgen);
                Node.Type = NodeType.PV;
                Node.U = Complex.FromPolarCoordinates(Node.Vpre, Node.U.Phase);
                if (gensOnLimits.Contains(Node.Num))
                {
                    gensOnLimits.Remove(Node.Num);
                    Node.Type = NodeType.PV;
                    Node.U = Complex.FromPolarCoordinates(Node.Vpre, Node.U.Phase);
                    crossLimits = true;
                }
            }

            return crossLimits;
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

            var Um = U.Map(u => u.Magnitude);
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

            //if(p.Count > 0) 
            //    res = Vector<double>.Build.DenseOfEnumerable(res.Concat(p));

            return res;
        }



        // TODO !!!
        // Inject PV bus limit logic into iteration procedure
        private static void NPCAddition(Grid grid, 
                                        Vector<Complex> U, 
                                        ref Vector<double> dPQ, 
                                        ref Matrix<double> J)
        {
            // Ro merit function and additional values
            var p_list = Vector<double>.Build.Dense(grid.PV_Count);            
            var s_list = Vector<double>.Build.Dense(grid.PV_Count);
            var m_list = Vector<double>.Build.Dense(grid.PV_Count);

            var Um  = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            int pv_counter = 0;

            for (int i = 0; i < grid.Nodes.Count; i++)
            {
                if(grid.Nodes[i].Type == NodeType.PV)
                {                    
                    // Current node
                    var Node = grid.Nodes[i];

                    // New Q element
                    var Q_new = Node.S_load.Imaginary;
                    // Build new Q element
                    for (int j = 0; j < grid.Nodes.Count; j++)
                        Q_new -= U[i].Magnitude * U[j].Magnitude * grid.Y[i, j].Magnitude *
                                 Math.Sin(grid.Y[i, j].Phase + U[j].Phase - U[i].Phase);

                    // Get current Voltage magnitude and Q conststraints at node
                    var Vcurr = U[i].Magnitude;
                    var qmin = Node.Q_min;
                    var qmax = Node.Q_max;

                    if (Q_new >= qmax)
                    {
                        p_list[pv_counter] = Math.Sqrt(Math.Pow(qmax - Q_new, 2.0) + Math.Pow(grid.Nodes[i].Vpre - Vcurr, 2.0))
                                             - (qmax - Q_new) - (grid.Nodes[i].Vpre - Vcurr);

                        s_list[pv_counter] = (-(qmax - Q_new)) /
                                              Math.Sqrt(Math.Pow(qmax - Q_new, 2.0) + Math.Pow(grid.Nodes[i].Vpre - Vcurr, 2.0))
                                              + 1;

                        m_list[pv_counter] = (((Vcurr - grid.Nodes[i].Vpre) / Math.Sqrt(Math.Pow(qmax - Q_new, 2.0) + Math.Pow(grid.Nodes[i].Vpre - Vcurr, 2.0))) + 1) * Vcurr;

                    }
                    else if (Q_new <= qmin)
                    {
                        p_list[pv_counter] = Math.Sqrt(Math.Pow(Q_new - qmin, 2.0) + Math.Pow(grid.Nodes[i].Vpre - Vcurr, 2.0))
                                             - (Q_new - qmin) - (grid.Nodes[i].Vpre - Vcurr);

                        s_list[pv_counter] = ((Q_new - qmin) / Math.Sqrt(Math.Pow(Q_new - qmin, 2.0) + Math.Pow(Vcurr - grid.Nodes[i].Vpre, 2.0))) - 1;

                        m_list[pv_counter] = (((Vcurr - grid.Nodes[i].Vpre) / Math.Sqrt(Math.Pow(Q_new - qmin, 2.0) + Math.Pow(grid.Nodes[i].Vpre - Vcurr, 2.0))) - 1) * Vcurr;
                    }
                    else
                    {
                        p_list[pv_counter] = 0;
                        s_list[pv_counter] = 0;
                        m_list[pv_counter] = 0;
                    }

                    pv_counter++;
                }                
            }
            // Break procedure if no PV on limits
            if (p_list.All(elem => elem == 0.0)) return;

            // Create Ro vector
            var p = Vector<double>.Build.Dense(p_list.ToArray());

            // Create S amd M matrices
            Matrix<double> S = Matrix<double>.Build.DenseDiagonal(s_list.Count, s_list.Count, (s) => s_list[s]);
            Matrix<double> M = Matrix<double>.Build.DenseDiagonal(m_list.Count, m_list.Count, (m) => s_list[m]);

            // Correct Ro vector
            p = S.Inverse() * p;            

            var dim = grid.PQ_Count + grid.PV_Count;

            var P_delta = J.SubMatrix(0, dim, 0, dim);
            var P_V     = J.SubMatrix(0, dim, dim, grid.PQ_Count);
            var Q_delta = J.SubMatrix(dim, grid.PQ_Count, 0, dim);
            var Q_V     = J.SubMatrix(dim, grid.PQ_Count, dim, grid.PQ_Count);


            var N_1 = Matrix<double>.Build.Dense(dim, grid.PV_Count);
            var L_1 = Matrix<double>.Build.Dense(grid.PQ_Count, grid.PV_Count);
            var J_1 = Matrix<double>.Build.Dense(grid.PV_Count, dim);
            var L_2 = Matrix<double>.Build.Dense(grid.PV_Count, grid.PQ_Count);
            var L_3 = Matrix<double>.Build.Dense(grid.PV_Count, grid.PV_Count);


            // N_1 (P_V) -> PV nodes
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < grid.PV_Count; j++)
                {
                    if (i != j)
                        N_1[i, j] = Um[i] * grid.Y[i, grid.PQ_Count + j].Magnitude *
                                    Math.Cos(grid.Y[i, grid.PQ_Count + j].Phase + Uph[grid.PQ_Count + j] - Uph[i]);
                    else
                    {
                        // Component with deleted part (one of two) (i==j)
                        N_1[i, j] = Um[i] * grid.Y[i, i].Magnitude *
                                    Math.Cos(grid.Y[i, i].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++)
                            N_1[i, j] += Um[k] * grid.Y[i, k].Magnitude *
                                         Math.Cos(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }
            }

            // L_1 (Q_V) -> PV nodes
            for (int i = 0; i < grid.PQ_Count; i++)
            {
                for (int j = 0; j < grid.PV_Count; j++)
                {
                    if (i != j)
                        L_1[i, j] = -Um[i] * grid.Y[i, grid.PQ_Count + j].Magnitude *
                                     Math.Sin(grid.Y[i, grid.PQ_Count + j].Phase + Uph[grid.PQ_Count + j] - Uph[i]);
                    else
                    {
                        // Component with deleted part (one of two) (i==j)
                        L_1[i, j] = -Um[i] * grid.Y[i, i].Magnitude *
                                    Math.Sin(grid.Y[i, i].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++)
                            L_1[i, j] -= Um[k] * grid.Y[i, k].Magnitude *
                                         Math.Sin(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }
            }

            //J_1 (Q_Delta) -> PV nodes
            for (int i = 0; i < grid.PV_Count; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (i != j)
                        J_1[i, j] = -Um[grid.PQ_Count + i] * Um[j] * grid.Y[grid.PQ_Count + i, j].Magnitude *
                                     Math.Cos(grid.Y[grid.PQ_Count + i, j].Phase + Uph[j] - Uph[grid.PQ_Count + i]);
                    else
                    {
                        // Component to DELETE from sum (i==j)
                        J_1[i, j] = -grid.Y[grid.PQ_Count + i, grid.PQ_Count + i].Magnitude *
                                     Math.Pow(Um[grid.PQ_Count + i], 2) *
                                     Math.Cos(grid.Y[grid.PQ_Count + i, grid.PQ_Count + i].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++)
                            J_1[i, j] += Um[grid.PQ_Count + i] * Um[k] * grid.Y[grid.PQ_Count + i, k].Magnitude *
                                         Math.Cos(grid.Y[grid.PQ_Count + i, k].Phase + Uph[k] - Uph[grid.PQ_Count + i]);
                    }
                }
            }

            // L_2 (Q_V) -> PV nodes
            for (int i = 0; i < grid.PV_Count; i++)
            {
                for (int j = 0; j < grid.PQ_Count; j++)
                {
                    if (i != j)
                        L_2[i, j] = -Um[grid.PQ_Count + i] * grid.Y[grid.PQ_Count + i, j].Magnitude *
                                    Math.Sin(grid.Y[grid.PQ_Count + i, j].Phase + Uph[j] - Uph[grid.PQ_Count + i]);
                    else
                    {
                        // Component with deleted part (one of two) (i==j)
                        L_2[i, j] = -Um[grid.PQ_Count + i] * grid.Y[grid.PQ_Count + i, grid.PQ_Count + i].Magnitude *
                                    Math.Sin(grid.Y[grid.PQ_Count + i, grid.PQ_Count + i].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++)
                            L_2[i, j] -= Um[k] * grid.Y[grid.PQ_Count + i, k].Magnitude *
                                         Math.Sin(grid.Y[grid.PQ_Count + i, k].Phase + Uph[k] - Uph[grid.PQ_Count + i]);
                    }
                }
            }

            // L_3 (Q_V) -> PV nodes
            for (int i = 0; i < grid.PV_Count; i++)
            {
                for (int j = 0; j < grid.PV_Count; j++)
                {
                    if (i != j)
                        L_3[i, j] = -Um[grid.PQ_Count + i] * grid.Y[grid.PQ_Count + i, grid.PQ_Count + j].Magnitude *
                                    Math.Sin(grid.Y[grid.PQ_Count + i, grid.PQ_Count + j].Phase + Uph[grid.PQ_Count + j] - Uph[grid.PQ_Count + i]);
                    else
                    {
                        // Component with deleted part (one of two) (i==j)
                        L_3[i, j] = -Um[grid.PQ_Count + i] * grid.Y[grid.PQ_Count + i, grid.PQ_Count + i].Magnitude *
                                    Math.Sin(grid.Y[grid.PQ_Count + i, grid.PQ_Count + i].Phase);

                        // Basic sum (i==j)
                        for (int k = 0; k < dim + grid.Slack_Count; k++)
                            L_3[i, j] -= Um[k] * grid.Y[grid.PQ_Count + i, k].Magnitude *
                                         Math.Sin(grid.Y[grid.PQ_Count + i, k].Phase + Uph[k] - Uph[grid.PQ_Count + i]);
                    }
                }
            }
                       
            // Output Jacobian matrix
            J = Matrix<double>.Build.DenseOfMatrixArray(new[,] { { P_delta, P_V, N_1 }, 
                                                                 { Q_delta, Q_V , L_1}, 
                                                                 { J_1, L_2, (S.Inverse() * M) + L_3 } });


            // Output dPQ vector 
            dPQ = Vector<double>.Build.DenseOfEnumerable(dPQ.Concat(p));
        }


        
    }
}
