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
        /// <param name="grid">Input grid for calculus</param>
        /// <param name="U_initial">Initial voltage vector</param>
        /// <param name="options">Caclculation options</param>
        /// <returns>Grid with calculated voltages</returns>
        public static Grid SolverGS(this Grid grid, 
                                         Vector<Complex> U_initial,
                                         CalculationOptions options)
        {
            Grid gridReserve = grid.DeepCopy(); // Reserve initial grid

            var Um = U_initial.Map(x => x.Magnitude).ToArray(); // Input voltages magnitude vector
            var ph = U_initial.Map(x => x.Phase).ToArray();     // Input voltages phase vector

            var U    = Vector<Complex>.Build.Dense(U_initial.Count); // Vector for calc voltages
            var Uold = Vector<Complex>.Build.Dense(U_initial.Count); // Vector for voltages on previous iteration
            var dU   = Vector<Complex>.Build.Dense(U_initial.Count); // Voltage difference on iteration
            
            U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u1, u2) => Complex.FromPolarCoordinates(u1, u2))); // Fill U vector with initial values
            Uold = Vector<Complex>.Build.DenseOfEnumerable(U); // Fill U vector with initial values
            //Uold = U.Clone();    // Fiil previous voltage vector with inital values before iterations

            // Helper variables
            var sum           = new Complex();    // Local current suumator
            double difference = Double.MaxValue;  // Big difference value for accuracy comparison       
                

            // MAIN CYCLE
            for (int iteration = 0; iteration < options.IterationsCount; iteration++)
            {
                // Nodes iterator
                for (int i = 0; i < grid.Nodes.Count; i++)
                {
                    if (grid.Nodes[i].Type == NodeType.Slack) break;  //Take Slack nodes

                    #region [PQ nodes]
                    if (grid.Nodes[i].Type == NodeType.PQ)  //Take PQ nodes
                    {
                        sum = 0;    // Reset summator

                        for (int j = 0; j < grid.Nodes.Count; j++)
                            if (i != j)
                                sum += grid.Y[i, j] * U[j]; // Complete summator with non-self values

                        U[i] = (1 / grid.Y[i, i]) * ((grid.S[i].Conjugate() / U[i].Conjugate()) - sum); // Calculate new voltage value in node
                        
                        dU = U - Uold;  // Complete corresponded values in dU vector on voltage difference
                    }
                    
                    #endregion

                    #region [PV nodes]
                    if (grid.Nodes[i].Type == NodeType.PV)  // Take PV nodes
                    {
                        sum = 0;

                        for (int j = 0; j < grid.Nodes.Count; j++)
                            sum += grid.Y[i, j] * U[j];     // Complete summator with corresponded values

                        var Q_new = -(U[i].Conjugate() * sum).Imaginary; // Build new Q element
                        grid.Nodes[i].S_gen = new Complex(grid.Nodes[i].S_gen.Real, Q_new);
                        grid.S[i] = new Complex(grid.S[i].Real, grid.Nodes[i].S_gen.Imaginary - grid.Nodes[i].S_load.Imaginary);  // Build new S element

                        // Q conststraints
                        var qmin =  grid.Nodes[i].Q_min;
                        var qmax =  grid.Nodes[i].Q_max;

                        bool flag = false;

                        if (Q_new <= qmin)
                        {
                            grid.Nodes[i].S_gen = new Complex(grid.Nodes[i].S_gen.Real, qmin);
                            grid.S[i] = new Complex(grid.S[i].Real, grid.Nodes[i].S_gen.Imaginary - grid.Nodes[i].S_load.Imaginary);  // Build new S element
                            flag = true;

                        }
                        else if (Q_new >= qmax)
                        {
                            grid.Nodes[i].S_gen = new Complex(grid.Nodes[i].S_gen.Real, qmax);
                            grid.S[i] = new Complex(grid.S[i].Real, grid.Nodes[i].S_gen.Imaginary - grid.Nodes[i].S_load.Imaginary);  // Build new S element
                            flag = true;
                        }
                        //else
                        //{
                        //    grid.Nodes[i].S_gen = new Complex(grid.Nodes[i].S_gen.Real, Q_new);
                        //    grid.S[i] = new Complex(grid.S[i].Real, grid.Nodes[i].S_gen.Imaginary - grid.Nodes[i].S_load.Imaginary);  // Build new S element
                        //}

                        sum = 0;  // Reset summator

                        for (int j = 0; j < grid.Nodes.Count; j++)
                            if (i != j)
                                sum += grid.Y[i, j] * U[j];   // Recomplete summator with non-self values

                        var voltage = (1 / grid.Y[i, i]) * ((grid.S[i].Conjugate() / U[i].Conjugate()) - sum); // Calculate new voltage value

                        if (flag)
                        {
                            U[i] = voltage;
                        }
                        else
                        {
                            U[i] = Complex.FromPolarCoordinates(grid.Nodes[i].Vpre, voltage.Phase);             // Fix magnitude (Vpre), change phase
                        }
                    
                    }

                    //Refresh magnitude and phase vectors and go to another node
                    Um = U.Map(u => u.Magnitude).ToArray(); 
                    ph = U.Map(u => u.Phase).ToArray();
                    #endregion
                }

                Uold = U.Clone();                           // Set as previous calculated voltages  
                difference = dU.AbsoluteMaximum().Real;   // Find the bigest defference


                // TODO: Exception catcher on checks !!!

                // CHECKS
                // 1.Check Voltage level(difference between actual and nominal)
                //CheckVoltage(U_nominal: grid.Uinit,
                //             U: U,
                //             grid: grid,
                //             voltageRate: 0.99);


                // 2.Check on Accuracy (Voltage step value)
                if (difference <= options.Accuracy)
                {
                    // Inform finish by voltage convergence criteria
                    Console.WriteLine($"Gaus-Seidel iterations: {iteration} " + 
                                      $"of {options.IterationsCount}. Success (Voltage convergence criteria)\n");

                    //Update voltage levels
                    for (int n = 0; n < grid.Nodes.Count; n++) 
                        grid.Nodes[n].U = U[n];

                    grid.Ucalc = U.Clone();   // Set new values to grid

                    break;
                }

                // Inform finish by iteration criteria
                if(iteration == options.IterationsCount - 1)
                    Console.WriteLine($"Gaus-Seidel iterations: {iteration + 1} " +
                                      $"of {options.IterationsCount}. Success (Iteration count criteria)\n");
            }

            ////PV Q-power checks
            //if (grid.PV_Count != 0)
            //{
            //    bool flag = false; // Flag on PV nodes transformation

            //    // PV nodes iteration
            //    for (int pv = grid.PQ_Count; pv < (grid.PQ_Count + grid.PV_Count); pv++)   
            //    {
            //        // Q conststraints
            //        var qmin = grid.Nodes[pv].Q_min;
            //        var qmax = grid.Nodes[pv].Q_max;

            //        var q = - grid.Nodes[pv].S_load.Imaginary + grid.S[pv].Imaginary;   // 

            //        if (q <= qmin)
            //        {
            //            q = qmin;
            //            grid.Nodes[pv].S_gen = new Complex(grid.Nodes[pv].S_gen.Real, q);
            //            grid.Nodes[pv].Type = NodeType.PQ;
            //            flag = true;

            //        }
            //        else if (q >= qmax)
            //        {
            //            q = qmax;
            //            grid.Nodes[pv].S_gen = new Complex(grid.Nodes[pv].S_gen.Real, q);
            //            grid.Nodes[pv].Type = NodeType.PQ;
            //            flag = true;
            //        }
            //    }
            //    if (flag == true) // Start Calc with new PQ nodes
            //    {
            //        grid.InitParameters(grid.Nodes, grid.Branches);
            //        grid.SolverGS(U, options);
            //        return grid;
            //    }
            //}

            //Update voltage levels
            for (int n = 0; n < grid.Nodes.Count; n++) 
                grid.Nodes[n].U = U[n];
            U.CopyTo(grid.Ucalc);

            return grid;
        }
    }



    


}
