using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using Complex = System.Numerics.Complex;
using PowerFlowCore.Data;

namespace PowerFlowCore.Solvers
{
    internal static partial class Solvers
    {
        /// <summary>
        /// Gauss-Seidel solver
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> for calculus</param>
        /// <param name="U_initial">Initial voltage <see cref="Vector{Complex}"/></param>
        /// <param name="options"><see cref="CalculationOptions"/> for calculus</param>
        /// <returns><see cref="Grid"/> with calculated voltages</returns>
        internal static Grid SolverGS(this Grid grid, 
                                         Vector<Complex> U_initial,
                                         CalculationOptions options,
                                         out bool success)
        {          
            var U    = Vector<Complex>.Build.DenseOfEnumerable(U_initial);  // Vector for calc voltages
            var Uold = Vector<Complex>.Build.DenseOfEnumerable(U_initial);  // Vector for voltages on previous iteration
            var dU   = Vector<Complex>.Build.Dense(U_initial.Count);        // Voltage difference on iteration

            // Buffer area for log messages
            List<string> LogBuffer = new List<string>();

            // Result success
            success = false;
           
            // Helper variables
            double difference = Double.MaxValue;  // Big difference value for accuracy comparison      

            // Current iteration
            int iter = 0;

            // MAIN CYCLE
            while (iter < options.IterationsCount)
            {
                // Nodes iterator
                for (int i = 0; i < grid.Nodes.Count; i++)
                {                   
                    if (grid.Nodes[i].Type == NodeType.PQ) 
                        CalcNodeAsPQ(grid, i, ref U, ref Uold, ref dU, options.AccelerationRateGS);

                    if (grid.Nodes[i].Type == NodeType.PV) 
                        CaclNodeAsPV(grid, i, ref U, ref Uold, ref dU, options.AccelerationRateGS);

                    // TODO:
                    // 1. SHN
                    // 2. Ktr changes


                    if (grid.Nodes[i].Type == NodeType.Slack) 
                        continue;  //Take Slack nodes
                }

                Uold       = U.Clone();        // Set as previous calculated voltages  
                difference = dU.InfinityNorm();// Find the bigest defference

                // Set new value to Nodes
                for (int n = 0; n < grid.Nodes.Count; n++)
                    grid.Nodes[n].U = U[n];

                // Evaluate static load model
                EvaluateLoadModel(grid);

                #region [Logging on iteration]

                // Internal logging
                if (options.SolverInternalLogging)
                {
                    (INode max_node, double max_v)  = grid.MaxVoltageNode();            // Get node and max voltage
                    (INode min_node, double min_v)  = grid.MinVoltageNode();            // Get node and min voltage
                    (IBranch br, double delta)      = grid.MaxAngleBranch();            // Get branch and max delta (angle)
                    (INode maxUnode, double du)     = grid.GetMaxVoltageResidual(dU);   // Get max residuals

                    LogBuffer.Add($"it:{iter} - " +
                                  $"rU:[{du}({maxUnode.Num})]; " +
                                  $"Umax/Umin:[{max_v}({max_node.Num})/{min_v}({min_node.Num})]; " +
                                  $"Delta:[{delta}({br.Start}-{br.End})]");
                }

                #endregion

                // Stop criteria
                if (difference <= options.Accuracy)
                {
                    //Update voltage levels
                    for (int n = 0; n < grid.Nodes.Count; n++)
                        grid.Nodes[n].U = U[n];
                    success = true;
                    break;
                }

                iter++;
            }            

            //Update voltage levels
            for (int n = 0; n < grid.Nodes.Count; n++) 
                grid.Nodes[n].U = U[n];


            // Logging
            lock (Logger._lock)
            {
                for (int i = 0; i < LogBuffer.Count; i++)
                    Logger.LogInfo(LogBuffer[i], grid.Id);
                if (success)
                    Logger.LogSuccess($"Converged (G-S solver) in {iter} of {options.IterationsCount} iterations", grid.Id);
                else
                    Logger.LogCritical($"Not converged (G-S solver) in {iter} of {options.IterationsCount} iterations", grid.Id);
            }                

            return grid;
        }


        /// <summary>
        /// Calculate voltage for PQ nodes
        /// </summary>
        /// <param name="grid">Grid object</param>
        /// <param name="nodeNum">List number of calc node</param>
        /// <param name="U">Vector of actual U</param>
        /// <param name="Uold">Vector of old U</param>
        /// <param name="dU">Voltage difference vector</param>
        /// <param name="accRate">Acceleration ratio for GS procedure</param>
        private static void CalcNodeAsPQ(Grid grid,
                                         int nodeNum,
                                         ref Vector<Complex> U, 
                                         ref Vector<Complex> Uold, 
                                         ref Vector<Complex> dU,
                                         double accRate)
        {
            // Summator
            Complex sum = 0;    

            // Complete summator with non-self values
            for (int j = 0; j < grid.Nodes.Count; j++)
                if (nodeNum != j)
                    sum += grid.Y[nodeNum, j] * U[j]; 

            // Calculate new voltage value in node
            U[nodeNum] = (1 / grid.Y[nodeNum, nodeNum]) * 
                         ((grid.S[nodeNum].Conjugate() / U[nodeNum].Conjugate()) - sum); 

            // Apply acceleration rate
            U[nodeNum] = Uold[nodeNum] + accRate * (U[nodeNum] - Uold[nodeNum]);

            // Complete voltage difference vector
            dU = U - Uold;
        }



        /// <summary>
        /// Calculate voltage for PV nodes
        /// </summary>
        /// <param name="grid">Grid object</param>
        /// <param name="nodeNum">List number of calc node</param>
        /// <param name="U">Vector of actual U</param>
        /// <param name="Uold">Vector of old U</param>
        /// <param name="dU">Voltage difference vector</param>
        /// <param name="accRate">Acceleration ratio for GS procedure</param>
        private static void CaclNodeAsPV(Grid grid,
                                         int nodeNum,
                                         ref Vector<Complex> U,
                                         ref Vector<Complex> Uold,
                                         ref Vector<Complex> dU,
                                         double accRate)
        {
            // New Q element
            var Q_new = 0.0;

            // Build new Q element
            for (int j = 0; j < grid.Nodes.Count; j++)
                Q_new -= U[nodeNum].Magnitude * U[j].Magnitude * grid.Y[nodeNum, j].Magnitude * 
                         Math.Sin(grid.Y[nodeNum, j].Phase + U[j].Phase - U[nodeNum].Phase);

            Q_new += grid.Nodes[nodeNum].S_calc.Imaginary;

            // Q conststraints
            var qmin = grid.Nodes[nodeNum].Q_min;
            var qmax = grid.Nodes[nodeNum].Q_max;

            // Constraints flag
            bool isOutOfLimits = false;

            // Check limits conditions
            if (qmin.HasValue && Q_new <= qmin)
            {
                grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, qmin.Value);
                isOutOfLimits = true;

            }
            else if (qmax.HasValue && Q_new >= qmax)
            {
                grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, qmax.Value);
                isOutOfLimits = true;
            }
            else
            {
                grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, Q_new);
            }

            // Perfome analysis on conditons
            // Get PQ procedure
            if (isOutOfLimits == true)
            {
                CalcNodeAsPQ(grid, nodeNum, ref U, ref Uold, ref dU, accRate);
                return;
            }
            // Calc Angle only
            else
            {
                // Summator
                var sum = new Complex();

                for (int j = 0; j < grid.Nodes.Count; j++)
                    if (nodeNum != j)
                        sum += grid.Y[nodeNum, j] * U[j];   // Recomplete summator with non-self values

                // Calculate new voltage value
                var voltage = (1 / grid.Y[nodeNum, nodeNum]) * 
                              ((grid.S[nodeNum].Conjugate() / U[nodeNum].Conjugate()) - sum);

                // Fix magnitude (Vpre), change angle
                U[nodeNum] = Complex.FromPolarCoordinates(grid.Nodes[nodeNum].Vpre, voltage.Phase);
            }
        }


        /// <summary>
        /// Find maximum residuals of U and corresponded node
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <param name="U">Voltage vector on current iteration</param>
        /// <returns>(Max dU node, Max dU value)</returns>
        private static (INode node, double du) GetMaxVoltageResidual(this Grid grid, Vector<Complex> dU)
        {
            var index = dU.Map(v => Math.Abs(v.Magnitude)).MaximumIndex();
            var du    = dU[index].Magnitude;
            var node  = grid.Nodes[index];

            return (node, Math.Round(du, 3));
        }
    }
}
