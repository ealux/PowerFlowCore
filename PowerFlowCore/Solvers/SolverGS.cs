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
        /// Gauss-Seidel solver
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> for calculus</param>
        /// <param name="U_initial">Initial voltage <see cref="Vector{Complex}"/></param>
        /// <param name="options"><see cref="CalculationOptions"/> for calculus</param>
        /// <returns><see cref="Grid"/> with calculated voltages</returns>
        public static Grid SolverGS(this Grid grid, 
                                         Vector<Complex> U_initial,
                                         CalculationOptions options)
        {
            // Reserve initial grid
            Grid gridReserve = grid.DeepCopy(); 

            var U    = Vector<Complex>.Build.DenseOfEnumerable(U_initial);  // Vector for calc voltages
            var Uold = Vector<Complex>.Build.DenseOfEnumerable(U_initial);  // Vector for voltages on previous iteration
            var dU   = Vector<Complex>.Build.Dense(U_initial.Count);        // Voltage difference on iteration
           
            // Helper variables
            double difference = Double.MaxValue;  // Big difference value for accuracy comparison       
                
            // MAIN CYCLE
            for (int iteration = 0; iteration < options.IterationsCount; iteration++)
            {
                // Nodes iterator
                for (int i = 0; i < grid.Nodes.Count; i++)
                {                   
                    if (grid.Nodes[i].Type == NodeType.PQ) 
                        CalcNodeAsPQ(grid, i, ref U, ref Uold, ref dU, options.AccelerationRateGS);

                    if (grid.Nodes[i].Type == NodeType.PV) 
                        CaclNodeAsPV(grid, i, ref U, ref Uold, ref dU, options.AccelerationRateGS);

                    if (grid.Nodes[i].Type == NodeType.Slack) 
                        continue;  //Take Slack nodes
                }

                Uold = U.Clone();                           // Set as previous calculated voltages  
                difference = dU.AbsoluteMaximum().Real;   // Find the bigest defference


                // TODO: Exception catcher on checks !!!
                // CHECKS
                //1.Check Voltage level(difference between actual and nominal)
                CheckVoltage(U_nominal: grid.Uinit, U: U, grid: grid, voltageRate: options.VotageRate);


                // Stop criteria
                // Check on Voltage Convergence
                if (difference <= options.VoltageConvergence)
                {
                    // Inform finish by voltage convergence criteria
                    Console.WriteLine($"Gaus-Seidel iterations: {iteration} " +
                                      $"of {options.IterationsCount}. Success (Voltage convergence criteria)\n");

                    //Update voltage levels
                    for (int n = 0; n < grid.Nodes.Count; n++)
                        grid.Nodes[n].U = U[n];  

                    break;
                }
                // Inform finish by Iteration Criteria
                if (iteration == options.IterationsCount - 1)
                    Console.WriteLine($"Gaus-Seidel iterations: {iteration + 1} " +
                                      $"of {options.IterationsCount}. Success (Iteration count criteria)\n");
            }            

            //Update voltage levels
            for (int n = 0; n < grid.Nodes.Count; n++) 
                grid.Nodes[n].U = U[n];


            // !!!!!
            // TODO:
            // 1. (Not)Success logic
            // 2. Voltage restrictions
            // 3. Angle restrictions in branches
            // !!!!!

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

            Q_new = Q_new + grid.Nodes[nodeNum].S_load.Imaginary;

            // Q conststraints
            var qmin = grid.Nodes[nodeNum].Q_min;
            var qmax = grid.Nodes[nodeNum].Q_max;

            // Constraints flag
            bool isOutOfLimits = false;

            // Check limits conditions
            if (Q_new <= qmin)
            {
                grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, qmin);
                isOutOfLimits = true;

            }
            else if (Q_new >= qmax)
            {
                grid.Nodes[nodeNum].S_gen = new Complex(grid.Nodes[nodeNum].S_gen.Real, qmax);
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
    }
}
