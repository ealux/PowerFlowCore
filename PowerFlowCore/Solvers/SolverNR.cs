using PowerFlowCore.Algebra;
using PowerFlowCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Schema;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Solvers
{
    internal static partial class Solvers
    {
        /// <summary>
        /// Newton-Raphson Solver
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> for calculus</param>
        /// <param name="U_initial">Initial voltage <see cref="Complex[]"/></param>
        /// <param name="options"><see cref="CalculationOptions"/> for calculus</param>
        /// <returns><see cref="Grid"/> with calculated voltages</returns>
        internal static Grid SolverNR(this Grid grid,
                                         Complex[] U_initial,
                                         CalculationOptions options,
                                         out bool success)
        {                        
            // Save PV nodes being transformed
            List<int> gensOnLimits = new List<int>();

            // Buffer area for log messages
            List<string> LogBuffer = new List<string>();
            
            bool crossLimits = true;
                 success     = true;
            int  iteration   = 0;
            int  iter        = 0;

            // U vectors 
            var U = VectorComplex.Create(U_initial);

            while (crossLimits & success)
            {
                // Calculate power flow
                (U, success, iteration) = NewtonRaphson(grid, U, options, iter, ref LogBuffer);

                // Increment iteration count
                iter = iteration + 1;

                // Inspect PV
                InspectPV_Classic(grid, ref U, ref gensOnLimits, out crossLimits);

                // Rebuild scheme if needed
                if (crossLimits)
                    grid.InitParameters(grid.Nodes, grid.Branches);

                // Set up voltages after rebuilding
                U = grid.Ucalc.Copy();
            }            

            // Logging
            lock (Logger._lock)
            {
                for (int i = 0; i < LogBuffer.Count; i++)
                    Logger.LogInfo(LogBuffer[i], grid.Id);
                if (success)
                    Logger.LogSuccess($"Converged (N-R solver) in {iter} of {options.IterationsCount} iterations", grid.Id);
                else
                    Logger.LogCritical($"Not converged (N-R solver) in {iter - 1} of {options.IterationsCount} iterations", grid.Id);
            }

            // Nodes convert back
            for (int i = 0; i < gensOnLimits.Count; i++) 
                grid.Nodes.First(n => n.Num == gensOnLimits[i]).Type = NodeType.PV;

            //Re - building scheme back
            grid.InitParameters(grid.Nodes, grid.Branches);

            // Constraints
            if (options.UseVoltageConstraint)
            {
                // Check voltage constraints
                var outOfVoltageOverflow = grid.CheckVoltageOverflow(options.VoltageConstraintPercentage / 100);
                var outOfVoltageLack = grid.CheckVoltageLack(options.VoltageConstraintPercentage / 100);

                if (outOfVoltageOverflow.Any() || outOfVoltageLack.Any())
                {
                    success = false;
                    foreach (var item in outOfVoltageOverflow.OrderByDescending(i => i.Item2))
                        Logger.LogWarning($"Voltage constraints is violated in node {item.Item1.Num} (+{item.Item2}%)", grid.Id);
                    foreach (var item in outOfVoltageLack.OrderByDescending(i => i.Item2))
                        Logger.LogWarning($"Voltage constraints is violated in node {item.Item1.Num} ({item.Item2}%)", grid.Id);
                    Logger.LogCritical($"Calculation failed due to voltage restrictions violation", grid.Id);
                }
            }

            return grid;
        }



        /// <summary>
        /// Calculate Voltage <see cref="Vector{Complex}"/> by Newton-Raphson technique
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> for calculus</param>
        /// <param name="U_initial">Present voltage <see cref="Complex[]"/> to start from</param>
        /// <param name="options"><see cref="CalculationOptions"/> for calculus</param>
        /// <returns>Return <see cref="Tuple{Complex[], bool, int}"/> as (Voltage, Success indicator, Iteration count)</returns>
        private static (Complex[], bool success, int iter) NewtonRaphson(Grid grid,
                                                                         Complex[] U_initial,
                                                                         CalculationOptions options,
                                                                         int current_iter,
                                                                         ref List<string> logBuffer)
        {
            // U vectors 
            var U    = VectorComplex.Create(U_initial);
            var Uold = VectorComplex.Create(U_initial);

            double[] dPQ; // Residuals
            CSCMatrix J;   // Jacobian matrix

            // Iter number
            int iter = current_iter;

            // Convergence
            bool success = false;

            // Voltage estimation
            while (iter < options.IterationsCount)
            {
                // Input voltage magnitude and phase vectors
                var Um = U.Map(x => x.Magnitude); 
                var ph = U.Map(x => x.Phase);

                (J, dPQ) = Sparse_J_and_dPQ_Polar(grid, U);

                // Calculation of increments
                double[] dx = CSCMatrixSolver.Solve(J, dPQ.Negative());

                // Update angles
                for (int j = 0; j < (grid.PQ_Count + grid.PV_Count); j++)
                    ph[j] -= dx[j];

                // Update magnitudes
                for (int j = 0; j < grid.PQ_Count; j++)
                    Um[j] -= dx[grid.PQ_Count + grid.PV_Count + j];

                // Save old and calc new voltages
                Uold = U.Copy();
                U = VectorComplex.Create(Um.Zip(ph, (u, angle) => Complex.FromPolarCoordinates(u, angle)));

                // Set new value to Nodes
                for (int n = 0; n < grid.Nodes.Count; n++)
                    grid.Nodes[n].U = U[n];                

                // Evaluate static load model
                EvaluateLoadModel(grid);

#region [Logging on iteration]

                // Internal logging
                if (options.SolverInternalLogging)
                {
                    (INode max_node, double max_v) = grid.MaxVoltageNode();    // Get node and max voltage
                    (INode min_node, double min_v) = grid.MinVoltageNode();    // Get node and min voltage
                    (IBranch br, double delta)     = grid.MaxAngleBranch();    // Get branch and max delta (angle)
                    (INode maxPnode, double max_dP, INode maxQnode, double max_dQ) = grid.GetMaximumResiduals(dPQ); // Get max residuals

                    logBuffer.Add($"it:{iter} - " +
                                  $"rP/Q:[{max_dP}({maxPnode.Num})/{max_dQ}({maxQnode.Num})]; " +
                                  $"Umax/Umin:[{max_v}({max_node.Num})/{min_v}({min_node.Num})]; " +
                                  $"Delta:[{delta}({br.Start}-{br.End})]");
                }

#endregion

                //Power residual stop criteria
                if (dPQ.InfinityNorm() <= options.Accuracy)
                {
                    success = true;
                    break;
                }                

                // Next iteration
                iter++; 
            }

            // Iterations count limit
            if (iter == options.IterationsCount)
                success = false;

            return (U, success, iter);
        }

        /// <summary>
        /// Load evaluation based on <see cref="IStaticLoadModel"/> in <see cref="Grid.LoadModels"/> collection
        /// </summary>
        /// <param name="grid">input grid</param>
        private static void EvaluateLoadModel(Grid grid)
        {
            if(grid.LoadModels.Count == 0) 
                return;

            foreach (var item in grid.Nodes.Where(n => n.LoadModelNum.HasValue))
                grid.LoadModels[item.LoadModelNum!.Value].ApplyModel(item);
        }


        /// <summary>
        /// Logic on PV busses inspection for Q limits (switch-bus)
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> for calculus</param>
        /// <param name="U">Actual voltage <see cref="Complex[]"/></param>
        /// <param name="gensOnLimits"><see cref="List{int}"/> of PV buses on limits</param>
        /// <param name="crossLimits">PV bus switching flag</param>
        private static void InspectPV_Classic(Grid grid,
                                              ref Complex[] U,
                                              ref List<int> gensOnLimits,
                                              out bool crossLimits)
        {
            // Constraints flag
            crossLimits = false;

            for (int nodeNum = 0; nodeNum < grid.Nodes.Count; nodeNum++)
            {
                if (grid.Nodes[nodeNum].Type == NodeType.PV || gensOnLimits.Contains(grid.Nodes[nodeNum].Num))
                {
                    // Current node
                    var Node = grid.Nodes[nodeNum];

#region [Calc new Q value]

                    // New Q element
                    var Q_new = Node.S_calc.Imaginary;
                    // Build new Q element
                    //for (int j = 0; j < grid.Nodes.Count; j++)
                    //    Q_new -= U[nodeNum].Magnitude * U[j].Magnitude * grid.Y[nodeNum, j].Magnitude *
                    //             Math.Sin(grid.Y[nodeNum, j].Phase + U[j].Phase - U[nodeNum].Phase);

                    for (int j = grid.Ysp.RowPtr[nodeNum]; j < grid.Ysp.RowPtr[nodeNum + 1]; j++)
                    {
                        var col = grid.Ysp.ColIndex[j];
                        Q_new -= U[nodeNum].Magnitude * U[col].Magnitude * grid.Ysp.Values[j].Magnitude *
                                 Math.Sin(grid.Ysp.Values[j].Phase + U[col].Phase - U[nodeNum].Phase);
                    }

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
        /// <param name="gensOnLimits"><see cref="List{int}"/> of PV buses on limits</param>
        /// <param name="Node"><see cref="INode"/> object PV bus depends on</param>
        /// <param name="Qgen">Calculatad Qgen value</param>
        /// <param name="qmin">PV bus lower Q limit</param>
        /// <param name="qmax">PV bus upper Q limit</param>
        /// <returns></returns>
        private static bool ChangeQgen(List<int> gensOnLimits, INode Node, double Qgen, double? qmin, double? qmax)
        {
            var crossLimits = false;

            // Check limits conditions
            if (qmin.HasValue && Qgen <= qmin)
            {
                // If Qgen on LOWER limit but voltage BIGGER then Vpre 
                Node.S_gen = new Complex(Node.S_gen.Real, qmin.Value);
                Node.Type = NodeType.PQ;
                if (!gensOnLimits.Contains(Node.Num))
                {
                    gensOnLimits.Add(Node.Num);
                    crossLimits = true;
                }
            }
            else if (qmax.HasValue && Qgen >= qmax)
            {
                // If Qgen on UPPER limit but voltage LESS then Vpre                        
                Node.S_gen = new Complex(Node.S_gen.Real, qmax.Value);
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


        #region Dense

        ///// <summary>
        ///// Jacobian matrix calculation (Parallel) on each iteration
        ///// </summary>
        ///// <param name="grid">Input <see cref="Grid"/> for calculus</param>
        ///// <param name="U">Present <see cref="Complex[]"/> voltage value</param>
        ///// <returns>Jacobian matrix</returns>
        //private static double[,] Jacobian_Polar(Grid grid, Complex[] U)
        //{
        //    var dim = grid.PQ_Count + grid.PV_Count;

        //    var Um  = U.Map(u => u.Magnitude);
        //    var Uph = U.Map(u => u.Phase);

        //    var P_Delta = MatrixDouble.Create(dim, dim);
        //    var P_V     = MatrixDouble.Create(dim, grid.PQ_Count);
        //    var Q_Delta = MatrixDouble.Create(grid.PQ_Count, dim);
        //    var Q_V     = MatrixDouble.Create(grid.PQ_Count, grid.PQ_Count);

            
        //    Parallel.For(0, dim, (i) =>
        //    {
        //        //P_Delta
        //        for (int j = 0; j < dim; j++)
        //        {
        //            if (i != j)
        //            {
        //                P_Delta[i, j] = -Um[i] * Um[j] * grid.Y[i, j].Magnitude *
        //                                Math.Sin(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
        //            }
                        
        //            else
        //            {
        //                // Component to DELETE from sum (i==j)
        //                P_Delta[i, j] = -grid.Y[i, j].Magnitude *
        //                                Math.Pow(Um[i], 2) *
        //                                Math.Sin(grid.Y[i, j].Phase);

        //                // Basic sum (i==j)
        //                for (int k = 0; k < dim + grid.Slack_Count; k++)
        //                    P_Delta[i, j] += Um[i] * Um[k] * grid.Y[i, k].Magnitude *
        //                                     Math.Sin(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
        //            }
        //        }
        //        //P_V
        //        for (int j = 0; j < grid.PQ_Count; j++)
        //        {
        //            if (i != j)
        //                P_V[i, j] = Um[i] * grid.Y[i, j].Magnitude *
        //                            Math.Cos(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
        //            else
        //            {
        //                // Component with deleted part (one of two) (i==j)
        //                P_V[i, j] = Um[i] * grid.Y[i, j].Magnitude *
        //                            Math.Cos(grid.Y[i, j].Phase);

        //                // Basic sum (i==j)
        //                for (int k = 0; k < dim + grid.Slack_Count; k++)
        //                    P_V[i, j] += Um[k] * grid.Y[i, k].Magnitude *
        //                                 Math.Cos(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
        //            }
        //        }
        //    });


        //    Parallel.For(0, grid.PQ_Count, (i) =>
        //    {
        //        //Q_Delta
        //        for (int j = 0; j < dim; j++)
        //        {
        //            if (i != j)
        //                Q_Delta[i, j] = -Um[i] * Um[j] * grid.Y[i, j].Magnitude *
        //                                Math.Cos(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
        //            else
        //            {
        //                // Component to DELETE from sum (i==j)
        //                Q_Delta[i, j] = -grid.Y[i, j].Magnitude *
        //                                Math.Pow(Um[i], 2) *
        //                                Math.Cos(grid.Y[i, j].Phase);

        //                // Basic sum (i==j)
        //                for (int k = 0; k < dim + grid.Slack_Count; k++)
        //                    Q_Delta[i, j] += Um[i] * Um[k] * grid.Y[i, k].Magnitude *
        //                                     Math.Cos(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
        //            }
        //        }
        //        //Q_V
        //        for (int j = 0; j < grid.PQ_Count; j++)
        //        {
        //            if (i != j)
        //                Q_V[i, j] = -Um[i] * grid.Y[i, j].Magnitude *
        //                            Math.Sin(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
        //            else
        //            {
        //                // Component with deleted part (one of two) (i==j)
        //                Q_V[i, j] = -Um[i] * grid.Y[i, j].Magnitude *
        //                            Math.Sin(grid.Y[i, j].Phase);

        //                // Basic sum (i==j)
        //                for (int k = 0; k < dim + grid.Slack_Count; k++)
        //                    Q_V[i, j] -= Um[k] * grid.Y[i, k].Magnitude *
        //                                 Math.Sin(grid.Y[i, k].Phase + Uph[k] - Uph[i]);
        //            }
        //        }
        //    });

        //    // Form result matrix
        //    var res = MatrixDouble.CreateFromArray(new[,] { { P_Delta, P_V }, { Q_Delta, Q_V } });

        //    return res;
        //}

        ///// <summary>
        ///// Power residuals vector
        ///// </summary>
        ///// <param name="grid">Input <see cref="Grid"/> for calculus</param>
        ///// <param name="U">Present voltage value</param>
        ///// <returns> Power residuals vector</returns>
        //private static double[] Resuduals_Polar(Grid grid, Complex[] U)
        //{
        //    var dim = grid.PQ_Count + grid.PV_Count;

        //    var G = grid.Y.Real();
        //    var B = grid.Y.Imaginary();

        //    var Um = U.Map(u => u.Magnitude);
        //    var Uph = U.Map(u => u.Phase);

        //    var dP = VectorDouble.Create(dim);
        //    var dQ = VectorDouble.Create(grid.PQ_Count);

        //    //dP
        //    Parallel.For(0, dim, i =>
        //    {
        //        dP[i] = grid.S[i].Real;
        //        for (int j = 0; j < dim + grid.Slack_Count; j++)
        //            dP[i] -= Um[i] * Um[j] * grid.Y[i, j].Magnitude *
        //                     Math.Cos(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
        //    });

        //    //dQ
        //    Parallel.For(0, grid.PQ_Count, i =>
        //    {
        //        dQ[i] = grid.S[i].Imaginary;
        //        for (int j = 0; j < dim + grid.Slack_Count; j++)
        //            dQ[i] -= -Um[i] * Um[j] * grid.Y[i, j].Magnitude *
        //                      Math.Sin(grid.Y[i, j].Phase + Uph[j] - Uph[i]);
        //    });

        //    // Form result vector
        //    var res = VectorDouble.Create(dP.Concat(dQ));

        //    return res;
        //}

        #endregion

        #region Sparse

        private static (CSCMatrix, double[]) Sparse_J_and_dPQ_Polar_old(Grid grid, Complex[] U)
        {
            var dim = grid.PQ_Count + grid.PV_Count;

            var Um = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            var rows = new SparseVector[dim + grid.PQ_Count];

            var dP = VectorDouble.Create(dim);
            var dQ = VectorDouble.Create(grid.PQ_Count);

            // 0 -> dim
            Parallel.For(0, dim, (i) =>
            {
                //dP
                dP[i] = grid.Ssp[i].Real;
                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    dP[i] -= Um[i] * Um[col] * grid.Ysp.Values[j].Magnitude *
                             Math.Cos(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                }

                var P_Delta = new double[dim];
                var P_V = new double[grid.PQ_Count];
                var diagY = grid.Ysp[i];

                //P_Delta
                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    if (col != i && col < dim)
                    {
                        P_Delta[col] = -Um[i] * Um[col] * grid.Ysp.Values[j].Magnitude *
                                            Math.Sin(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                    }
                }
                P_Delta[i] = -diagY.Magnitude *
                                        Math.Pow(Um[i], 2) *
                                        Math.Sin(diagY.Phase);

                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    P_Delta[i] += Um[i] * Um[col] * grid.Ysp.Values[j].Magnitude *
                                     Math.Sin(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                }

                //P_V
                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    if (col != i && col < grid.PQ_Count)
                    {
                        P_V[col] = Um[i] * grid.Ysp.Values[j].Magnitude *
                                    Math.Cos(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                    }
                }

                if (i < grid.PQ_Count)
                {
                    P_V[i] = Um[i] * diagY.Magnitude *
                                    Math.Cos(diagY.Phase);

                    for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                    {
                        var col = grid.Ysp.ColIndex[j];
                        P_V[i] += Um[col] * grid.Ysp.Values[j].Magnitude *
                                             Math.Cos(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                    }
                }

                var v1 = new SparseVector(P_Delta);
                var v2 = new SparseVector(P_V);

                rows[i] = v1.Concat(v2);
            });

            // 0 -> grid.PQ_Count
            Parallel.For(0, grid.PQ_Count, (i) =>
            {
                //dQ
                dQ[i] = grid.Ssp[i].Imaginary;
                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    dQ[i] -= -Um[i] * Um[col] * grid.Ysp.Values[j].Magnitude *
                              Math.Sin(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                }

                var Q_Delta = new double[dim];
                var Q_V = new double[grid.PQ_Count];
                var diagY = grid.Ysp[i];

                //Q_Delta
                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    if (col != i && col < dim)
                    {
                        var c = grid.Ysp.ColIndex[j];
                        Q_Delta[col] = -Um[i] * Um[col] * grid.Ysp.Values[j].Magnitude *
                                        Math.Cos(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                    }
                }
                Q_Delta[i] = -diagY.Magnitude *
                                Math.Pow(Um[i], 2) *
                                Math.Cos(diagY.Phase);
                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    Q_Delta[i] += Um[i] * Um[col] * grid.Ysp.Values[j].Magnitude *
                                             Math.Cos(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                }
                //Q_V
                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    if (col != i && col < grid.PQ_Count)
                    {
                        var c = grid.Ysp.ColIndex[j];
                        Q_V[col] = -Um[i] * grid.Ysp.Values[j].Magnitude *
                                    Math.Sin(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                    }
                }
                Q_V[i] = -Um[i] * diagY.Magnitude *
                            Math.Sin(diagY.Phase);

                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    var col = grid.Ysp.ColIndex[j];
                    Q_V[i] -= Um[col] * grid.Ysp.Values[j].Magnitude *
                                 Math.Sin(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                }

                var v1 = new SparseVector(Q_Delta);
                var v2 = new SparseVector(Q_V);

                rows[dim + i] = v1.Concat(v2);
            });

            var resJ = CSRMatrix.CreateFromRows(rows).ToCSC();
            var resdPQ = VectorDouble.Create(dP.Concat(dQ));

            return (resJ, resdPQ);
        }

        private static (CSCMatrix, double[]) Sparse_J_and_dPQ_Polar(Grid grid, Complex[] U)
        {
            var dim = grid.PQ_Count + grid.PV_Count;

            var Um = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            var rows = new SparseVector[dim + grid.PQ_Count];

            var dP = VectorDouble.Create(dim);
            var dQ = VectorDouble.Create(grid.PQ_Count);

            // 0 -> dim
            Parallel.For(0, dim, (i) =>
            {                
                dP[i] = grid.Ssp[i].Real;

                var dimDiap = grid.Ysp.RowPtr[i + 1] - grid.Ysp.RowPtr[i];
                var pqDiap = grid.Ysp.ColIndex.Skip(grid.Ysp.RowPtr[i])
                                              .Take(grid.Ysp.RowPtr[i + 1] - grid.Ysp.RowPtr[i])
                                              .Where(ind => ind < grid.PQ_Count)
                                              .Count();

                Dictionary<int, double> P_Delta = new Dictionary<int, double>(grid.Ysp.RowPtr[i + 1] - grid.Ysp.RowPtr[i]);
                Dictionary<int, double> P_V = new Dictionary<int, double>(pqDiap);

                Dictionary<int, double> Q_Delta = new Dictionary<int, double>();
                Dictionary<int, double> Q_V = new Dictionary<int, double>();

                var diagY = grid.Ysp[i];

                //P_Delta
                P_Delta[i] = -diagY.Magnitude *
                               Math.Pow(Um[i], 2) *
                               Math.Sin(diagY.Phase);

                if (i < grid.PQ_Count)
                {
                    //P_V
                    P_V[i] = Um[i] * diagY.Magnitude *
                                    Math.Cos(diagY.Phase);

                    //dQ
                    dQ[i] = grid.Ssp[i].Imaginary;

                    Q_Delta = new Dictionary<int, double>(grid.Ysp.RowPtr[i + 1] - grid.Ysp.RowPtr[i]);
                    Q_V = new Dictionary<int, double>(pqDiap);

                    Q_Delta[i] = -diagY.Magnitude *
                                        Math.Pow(Um[i], 2) *
                                        Math.Cos(diagY.Phase);
                    //Q_V
                    Q_V[i] = -Um[i] * diagY.Magnitude *
                                    Math.Sin(diagY.Phase);
                }
                    

                for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
                {
                    // vars
                    var col = grid.Ysp.ColIndex[j];
                    var ymag = grid.Ysp.Values[j].Magnitude;
                    var cos = Math.Cos(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                    var sin = Math.Sin(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);


                    //dP
                    dP[i] -= Um[i] * Um[col] * ymag * cos;

                    //P_Delta
                    P_Delta[i] += Um[i] * Um[col] * ymag * sin;

                    if (col != i && col < dim)
                    {
                        //P_Delta
                        P_Delta[col] = -Um[i] * Um[col] * ymag * sin;

                        if (col < grid.PQ_Count)
                        {
                            //P_V
                            P_V[col] = Um[i] * ymag * cos;
                        }
                    }
                    if(i < grid.PQ_Count)
                    {
                        //P_V
                        P_V[i] += Um[col] * ymag * cos;

                        //dQ
                        dQ[i] -= -Um[i] * Um[col] * ymag * sin;

                        //Q_Delta
                        Q_Delta[i] += Um[i] * Um[col] * ymag * cos;

                        //Q_V
                        Q_V[i] -= Um[col] * ymag * sin;

                        if (col != i && col < dim)
                        {
                            //Q_Delta
                            Q_Delta[col] = -Um[i] * Um[col] * ymag * cos;

                            if(col < grid.PQ_Count)
                            {
                                //Q_V
                                Q_V[col] = -Um[i] * ymag * sin;
                            }
                        }

                    }
                }

                if (i < grid.PQ_Count)
                {
                    var v3 = new SparseVector(Q_Delta, dim);
                    var v4 = new SparseVector(Q_V, grid.PQ_Count);

                    rows[dim + i] = v3.Concat(v4);
                }

                var v1 = new SparseVector(P_Delta, dim);
                var v2 = new SparseVector(P_V, grid.PQ_Count);

                rows[i] = v1.Concat(v2);
            });

            var resJ = CSRMatrix.CreateFromRows(rows).ToCSC();
            var resdPQ = VectorDouble.Create(dP.Concat(dQ));

            return (resJ, resdPQ);
        }

        #endregion


        /// <summary>
        /// Find maximum residuals of P and Q with corresponded nodes
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <param name="dPQ">Residuals vector</param>
        /// <returns>(Max dP node, Max dP value, Max dQ node, Max dQ value)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (INode maxPnode, double max_dP, INode maxQnode, double max_dQ) GetMaximumResiduals(this Grid grid, double[] dPQ)
        {
            var dP = dPQ.SubVector(0, grid.PQ_Count + grid.PV_Count);
            var dQ = dPQ.SubVector(grid.PQ_Count + grid.PV_Count, grid.PQ_Count);

            var index_p  = dP.Map(r => Math.Abs(r)).MaximumIndex();
            var max_dP   = dP[index_p];
            var maxPnode = grid.Nodes[index_p];

            var index_q  = dQ.Map(r => Math.Abs(r)).MaximumIndex();
            var max_dQ   = dQ[index_q];
            var maxQnode = grid.Nodes[index_q];

            return (maxPnode, Math.Round(max_dP, 3), maxQnode, Math.Round(max_dQ, 3));
        }
    }
}
