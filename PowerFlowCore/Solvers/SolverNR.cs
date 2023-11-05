using PowerFlowCore.Algebra;
using PowerFlowCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Complex = System.Numerics.Complex;
#if (NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)
using System.Buffers;
#endif

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
                    grid.InitParameters(grid.Nodes, grid.Branches, false);

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
            grid.InitParameters(grid.Nodes, grid.Branches, false);

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

                (J, dPQ) = J_and_dPQ_Polar(grid, U);

                // Calculation of increments
                double[] dx = CSCMatrixSolver.Solve(J, dPQ);

                // Update angles
                for (int j = 0; j < (grid.PQ_Count + grid.PV_Count); j++)
                    ph[j] += dx[j];

                // Update magnitudes
                for (int j = 0; j < grid.PQ_Count; j++)
                    Um[j] += dx[grid.PQ_Count + grid.PV_Count + j];

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
            if(grid.LoadModels.Count == 0 || !grid.Nodes.Any(n => n.LoadModelNum.HasValue))
                return;
            
            // Apply models 
            foreach (var item in grid.Nodes.Where(n => n.LoadModelNum.HasValue))
                grid.LoadModels[item.LoadModelNum!.Value].ApplyModel(item);

            // Recalc power injections
            grid.Ssp = new SparseVectorComplex(grid.S);
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

        /// <summary>
        /// Calculate Jacobian matrix and Residuals vector
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> to calculate</param>
        /// <param name="U">Voltage vector at current state</param>
        private static (CSCMatrix J, double[] dPQ) J_and_dPQ_Polar(Grid grid, Complex[] U)
        {
            var dim = grid.PQ_Count + grid.PV_Count;

            var rows = new SparseVector[dim + grid.PQ_Count];

            var dP = new double[dim];
            var dQ = new double[grid.PQ_Count];

            // calcs
            if(dim <= 100)
            {
                var Um = U.Map(u => u.Magnitude);
                var Uph = U.Map(u => u.Phase);
                for (int i = 0; i < dim; i++)
                    InternalRowJob(grid, i, dim, Um, Uph, rows, dP, dQ);
            }
            else
            {
#if (NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)               

                var intPool = ArrayPool<int>.Shared;
                var doublePool = ArrayPool<double>.Shared;

                Parallel.For(0, dim, (i) =>
                {
                    InternalRowJob(grid, i, dim, U, rows, dP, dQ, 
                                    intPool, doublePool);
                });
                
#else                
                var Um = U.Map(u => u.Magnitude);
                var Uph = U.Map(u => u.Phase);
                Parallel.For(0, dim, (i) => InternalRowJob(grid, i, dim, Um, Uph, rows, dP, dQ));
#endif
            }

            var resJ = CSRMatrix.CreateFromRows(rows).ToCSC();
            var resdPQ = VectorDouble.Create(dP.Concat(dQ));

            return (resJ, resdPQ);
        }

#if (NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)
        /// <summary>
        /// Calculate Jacobian matrix and residuals vector (intrnal job)
        /// </summary>
        /// <param name="grid">Input grid object</param>
        /// <param name="i">iteration number</param>
        /// <param name="dim">PQ + PV nodes count</param>
        /// <param name="U">Voltage vector</param>
        /// <param name="rows">Massive to put in calculataed rows of Jacobian matrix</param>
        /// <param name="dP">Vector of active powers in residuals vector</param>
        /// <param name="dQ">Vector of reactive powers in residuals vector</param>
        /// <param name="intPool">Pool of int arrays</param>
        /// <param name="doublePool">Pool of double arrays</param>
        private static void InternalRowJob(Grid grid, int i, int dim,
                                           Complex[] U, SparseVector[] rows, double[] dP, double[] dQ,
                                           ArrayPool<int> intPool, ArrayPool<double> doublePool)
        {           
            dP[i] = grid.Ssp[i].Real;

            var dimDiap = grid.Ysp.RowPtr[i + 1] - grid.Ysp.RowPtr[i];
            var pqDiap = 0;
            for (int k = grid.Ysp.RowPtr[i]; k < grid.Ysp.RowPtr[i + 1]; k++)
            {
                if (grid.Ysp.ColIndex[k] < grid.PQ_Count)
                    pqDiap++;
            }

            //P_Delta
            var pdi = intPool.Rent(dimDiap);
            var pdv = doublePool.Rent(dimDiap);
            var P_Delta_inds = pdi.AsSpan().Slice(0, dimDiap);
            var P_Delta_vals = pdv.AsSpan().Slice(0, dimDiap);
            //P_V
            var pvi = intPool.Rent(pqDiap);
            var pvv = doublePool.Rent(pqDiap);
            var P_V_inds = pvi.AsSpan().Slice(0, pqDiap);
            var P_V_vals = pvv.AsSpan().Slice(0, pqDiap);

            //Q_Delta
            var qdi = intPool.Rent(dimDiap);
            var qdv = doublePool.Rent(dimDiap);
            var Q_Delta_inds = qdi.AsSpan().Slice(0, dimDiap);
            var Q_Delta_vals = qdv.AsSpan().Slice(0, dimDiap);
            //Q_V
            var qvi = intPool.Rent(pqDiap);
            var qvv = doublePool.Rent(pqDiap);
            var Q_V_inds = qvi.AsSpan().Slice(0, pqDiap);
            var Q_V_vals = qvv.AsSpan().Slice(0, pqDiap);

            var diagY = grid.Ysp[i];
            var diagYsin = Math.Sin(diagY.Phase);
            var diagYcos = Math.Cos(diagY.Phase);
            var umi = U[i].Magnitude;

            int p1 = 0, p2 = 0;
            for (int k = grid.Ysp.RowPtr[i]; k < grid.Ysp.RowPtr[i + 1]; k++)
            {
                P_Delta_inds[p1++] = grid.Ysp.ColIndex[k];
                if (grid.Ysp.ColIndex[k] < grid.PQ_Count)
                    P_V_inds[p2++] = grid.Ysp.ColIndex[k];
            }

            var pd_i_ind = P_Delta_inds.IndexOf(i);
            var pv_i_ind = P_V_inds.IndexOf(i);
            var qd_i_ind = -1;
            var qv_i_ind = -1;

            //P_Delta
            P_Delta_vals[pd_i_ind] = -diagY.Magnitude * Math.Pow(umi, 2) * diagYsin;

            if (i < grid.PQ_Count)
            {
                //P_V
                P_V_vals[pv_i_ind] = umi * diagY.Magnitude * diagYcos;

                //dQ
                dQ[i] = grid.Ssp[i].Imaginary;                

                p1 = 0; p2 = 0;
                for (int k = grid.Ysp.RowPtr[i]; k < grid.Ysp.RowPtr[i + 1]; k++)
                {
                    Q_Delta_inds[p1++] = grid.Ysp.ColIndex[k];
                    if (grid.Ysp.ColIndex[k] < grid.PQ_Count)
                        Q_V_inds[p2++] = grid.Ysp.ColIndex[k];
                }

                qd_i_ind = Q_Delta_inds.IndexOf(i);
                qv_i_ind = Q_V_inds.IndexOf(i);

                //Q_Delta
                Q_Delta_vals[qd_i_ind] = -diagY.Magnitude * Math.Pow(umi, 2) * diagYcos;

                //Q_V
                Q_V_vals[qv_i_ind] = -umi * diagY.Magnitude * diagYsin;
            }


            for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
            {
                // vars
                var col = grid.Ysp.ColIndex[j];
                var ymag = grid.Ysp.Values[j].Magnitude;
                var cos = Math.Cos(grid.Ysp.Values[j].Phase + U[col].Phase - U[i].Phase);
                var sin = Math.Sin(grid.Ysp.Values[j].Phase + U[col].Phase - U[i].Phase);                
                var umcol = U[col].Magnitude; 

                var pd_col_ind = P_Delta_inds.IndexOf(col);
                var pv_col_ind = P_V_inds.IndexOf(col);

                //dP
                dP[i] -= umi * umcol * ymag * cos;

                //P_Delta
                P_Delta_vals[pd_i_ind] += umi * umcol * ymag * sin;

                if (col != i && col < dim)
                {
                    //P_Delta
                    P_Delta_vals[pd_col_ind] = -umi * umcol * ymag * sin;

                    if (col < grid.PQ_Count)
                    {
                        //P_V
                        P_V_vals[pv_col_ind] = umi * ymag * cos;
                    }
                }
                if (i < grid.PQ_Count)
                {
                    var qd_col_ind = Q_Delta_inds.IndexOf(col);
                    var qv_col_ind = Q_V_inds.IndexOf(col);

                    //P_V
                    P_V_vals[pv_i_ind] += umcol * ymag * cos;

                    //dQ
                    dQ[i] -= -umi * umcol * ymag * sin;

                    //Q_Delta
                    Q_Delta_vals[qd_i_ind] += umi * umcol * ymag * cos;

                    //Q_V
                    Q_V_vals[qv_i_ind] -= umcol * ymag * sin;

                    if (col != i && col < dim)
                    {
                        //Q_Delta
                        Q_Delta_vals[qd_col_ind] = -umi * umcol * ymag * cos;

                        if (col < grid.PQ_Count)
                        {
                            //Q_V
                            Q_V_vals[qv_col_ind] = -umi * ymag * sin;
                        }
                    }

                }
            }
                        
            rows[i] = new SparseVector(P_Delta_inds,
                                       P_Delta_vals, dim,
                                       P_V_inds,
                                       P_V_vals, grid.PQ_Count);

            if (i < grid.PQ_Count)
            {
                rows[dim + i] = new SparseVector(Q_Delta_inds,
                                                 Q_Delta_vals, dim,
                                                 Q_V_inds,
                                                 Q_V_vals, grid.PQ_Count);
            }

            intPool.Return(pdi, true);
            doublePool.Return(pdv, true);
            intPool.Return(pvi, true);
            doublePool.Return(pvv, true);
            intPool.Return(qdi, true);
            doublePool.Return(qdv, true);
            intPool.Return(qvi, true);
            doublePool.Return(qvv, true);
        }

#endif
        /// <summary>
        /// Calculate Jacobian matrix and residuals vector (intrnal job)
        /// </summary>
        /// <param name="grid">Input grid object</param>
        /// <param name="i">iteration number</param>
        /// <param name="dim">PQ + PV nodes count</param>
        /// <param name="Um">Voltage magnitudes vector</param>
        /// <param name="Uph">Voltage phases vector</param>
        /// <param name="rows">Massive to put in calculataed rows of Jacobian matrix</param>
        /// <param name="dP">Vector of active powers in residuals vector</param>
        /// <param name="dQ">Vector of reactive powers in residuals vector</param>
        private static void InternalRowJob(Grid grid, int i, int dim, double[] Um, double[] Uph, SparseVector[] rows, double[] dP, double[] dQ)
        {
            dP[i] = grid.Ssp[i].Real;

            var dimDiap = grid.Ysp.RowPtr[i + 1] - grid.Ysp.RowPtr[i];
            var pqDiap = 0;
            for (int k = grid.Ysp.RowPtr[i]; k < grid.Ysp.RowPtr[i + 1]; k++)
            {
                if (grid.Ysp.ColIndex[k] < grid.PQ_Count)
                    pqDiap++;
            }

            //P_Delta
            int[] P_Delta_inds = new int[dimDiap];
            double[] P_Delta_vals = new double[dimDiap];
            //P_V
            int[] P_V_inds = new int[pqDiap];
            double[] P_V_vals = new double[pqDiap];
            //Q_Delta
            int[] Q_Delta_inds = null!;
            double[] Q_Delta_vals = null!;
            //Q_V
            int[] Q_V_inds = null!;
            double[] Q_V_vals = null!;

            var diagY = grid.Ysp[i];
            var diagYsin = Math.Sin(diagY.Phase);
            var diagYcos = Math.Cos(diagY.Phase);


            int p1 = 0, p2 = 0;
            for (int k = grid.Ysp.RowPtr[i]; k < grid.Ysp.RowPtr[i + 1]; k++)
            {
                P_Delta_inds[p1++] = grid.Ysp.ColIndex[k];
                if (grid.Ysp.ColIndex[k] < grid.PQ_Count)
                    P_V_inds[p2++] = grid.Ysp.ColIndex[k];
            }

            var pd_i_ind = Array.IndexOf(P_Delta_inds, i);
            var pv_i_ind = Array.IndexOf(P_V_inds, i);
            var qd_i_ind = -1;
            var qv_i_ind = -1;

            //P_Delta
            P_Delta_vals[pd_i_ind] = -diagY.Magnitude * Math.Pow(Um[i], 2) * diagYsin;

            if (i < grid.PQ_Count)
            {
                //P_V
                P_V_vals[pv_i_ind] = Um[i] * diagY.Magnitude * diagYcos;

                //dQ
                dQ[i] = grid.Ssp[i].Imaginary;

                Q_Delta_inds = new int[dimDiap];
                Q_Delta_vals = new double[dimDiap];

                Q_V_inds = new int[pqDiap];
                Q_V_vals = new double[pqDiap];

                p1 = 0; p2 = 0;
                for (int k = grid.Ysp.RowPtr[i]; k < grid.Ysp.RowPtr[i + 1]; k++)
                {
                    Q_Delta_inds[p1++] = grid.Ysp.ColIndex[k];
                    if (grid.Ysp.ColIndex[k] < grid.PQ_Count)
                        Q_V_inds[p2++] = grid.Ysp.ColIndex[k];
                }

                qd_i_ind = Array.IndexOf(Q_Delta_inds, i);
                qv_i_ind = Array.IndexOf(Q_V_inds, i);

                //Q_Delta
                Q_Delta_vals[qd_i_ind] = -diagY.Magnitude * Math.Pow(Um[i], 2) * diagYcos;

                //Q_V
                Q_V_vals[qv_i_ind] = -Um[i] * diagY.Magnitude * diagYsin;
            }

            for (int j = grid.Ysp.RowPtr[i]; j < grid.Ysp.RowPtr[i + 1]; j++)
            {
                // vars
                var col = grid.Ysp.ColIndex[j];
                var ymag = grid.Ysp.Values[j].Magnitude;
                var cos = Math.Cos(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);
                var sin = Math.Sin(grid.Ysp.Values[j].Phase + Uph[col] - Uph[i]);

                var pd_col_ind = Array.IndexOf(P_Delta_inds, col);
                var pv_col_ind = Array.IndexOf(P_V_inds, col);

                //dP
                dP[i] -= Um[i] * Um[col] * ymag * cos;

                //P_Delta
                P_Delta_vals[pd_i_ind] += Um[i] * Um[col] * ymag * sin;

                if (col != i && col < dim)
                {
                    //P_Delta
                    P_Delta_vals[pd_col_ind] = -Um[i] * Um[col] * ymag * sin;

                    if (col < grid.PQ_Count)
                    {
                        //P_V
                        P_V_vals[pv_col_ind] = Um[i] * ymag * cos;
                    }
                }
                if (i < grid.PQ_Count)
                {
                    var qd_col_ind = Array.IndexOf(Q_Delta_inds, col);
                    var qv_col_ind = Array.IndexOf(Q_V_inds, col);

                    //P_V
                    P_V_vals[pv_i_ind] += Um[col] * ymag * cos;

                    //dQ
                    dQ[i] -= -Um[i] * Um[col] * ymag * sin;

                    //Q_Delta
                    Q_Delta_vals[qd_i_ind] += Um[i] * Um[col] * ymag * cos;

                    //Q_V
                    Q_V_vals[qv_i_ind] -= Um[col] * ymag * sin;

                    if (col != i && col < dim)
                    {
                        //Q_Delta
                        Q_Delta_vals[qd_col_ind] = -Um[i] * Um[col] * ymag * cos;

                        if (col < grid.PQ_Count)
                        {
                            //Q_V
                            Q_V_vals[qv_col_ind] = -Um[i] * ymag * sin;
                        }
                    }

                }
            }

            rows[i] = new SparseVector(P_Delta_inds, P_Delta_vals, dim, P_V_inds, P_V_vals, grid.PQ_Count);

            if (i < grid.PQ_Count)
            {
                rows[dim + i] = new SparseVector(Q_Delta_inds, Q_Delta_vals, dim, Q_V_inds, Q_V_vals, grid.PQ_Count);
            }
        }


        /// <summary>
        /// Find maximum residuals of P and Q with corresponded nodes
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <param name="dPQ">Residuals vector</param>
        /// <returns>(Max dP node, Max dP value, Max dQ node, Max dQ value)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (INode maxPnode, double max_dP, INode maxQnode, double max_dQ) GetMaximumResiduals(this Grid grid, double[] dPQ)
        {
            //var dP = dPQ.SubVector(0, grid.PQ_Count + grid.PV_Count);
            var dP = dPQ.SubVector(grid.PQ_Count + grid.PV_Count);
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
