using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Complex = System.Numerics.Complex;

using PowerFlowCore.Data;
using PowerFlowCore.Algebra;
using PowerFlowCore.Solvers;



namespace PowerFlowCore.Samples
{
    partial class Program
    {
        static void Main()
        {
            /* Uncomment required group */

            var timer_global = Stopwatch.StartNew();

            Logger.AddConsoleMode();                          // Log to Console
            Logger.AddCustomMode(new CustomLoggerListener()); // Test custom listener (to Debug)
            //Logger.AddDebugMode();                          // Log to Debug
            //Logger.LogBroadcast += Logger_OnLogBroadcast;   // Logger event listener

            Logger.LogInfo("Calculation started");

            var timer = Stopwatch.StartNew();

            #region Creation

            //timer.Restart();
            //_ = SampleGrids.Test_Ktr();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Complex Ktr test

            //timer.Restart();
            //_ = SampleGrids.BreakersScheme();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Breakers Scheme

            //timer.Restart();
            //_ = SampleGrids.Nodes4_1PV();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV

            //timer.Restart();
            //_ = SampleGrids.Nodes4_1PV_ZIP();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV_ZIP

            //timer.Restart();
            //_ = SampleGrids.Nodes5_2Slack();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes5_2Slack

            //timer.Restart();
            //_ = SampleGrids.IEEE_14();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-14

            //timer.Restart();
            //_ = SampleGrids.Nodes15_3PV();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes15_3PV

            //timer.Restart();
            //_ = SampleGrids.IEEE_57();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-57

            //timer.Restart();
            //_ = SampleGrids.IEEE_118();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-118            

            //timer.Restart();
            //_ = SampleGrids.Nodes197_36PV();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes197_36PV

            //timer.Restart();
            //_ = SampleGrids.Nodes300_27PV();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes300_27PV

            //timer.Restart();
            //_ = SampleGrids.Nodes398_35PV();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes398_35PV

            //timer.Restart();
            //_ = SampleGrids.Nodes398_35PV_ZIP();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes398_35PV_ZIP

            ////for (int i = 0; i < 10; i++)
            ////{
            //timer.Restart();
            //_ = SampleGrids.Nodes874_143PV();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes874_143PV           
            //                                                                        //}

            ////for (int i = 0; i < 10; i++)
            ////{
            //timer.Restart();
            //_ = SampleGrids.Nodes1350_250PV();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes1350_250PV
            //                                                                        //}           

            ////for (int i = 0; i < 25; i++)
            ////{
            //    timer.Restart();
            //    _ = SampleGrids.Nodes2628_50PV();
            //    Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes2628_50PV
            ////}

            #endregion Creation

            #region Individual calcs     

            timer.Restart();
            CalculateAndShow(SampleGrids.Test_Ktr());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Complex Ktr test

            timer.Restart();
            CalculateAndShow(SampleGrids.BreakersScheme());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Breakers Scheme

            CalculateAndShow(SampleGrids.Nodes4_1PV());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes4_1PV_ZIP());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV_ZIP

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes5_2Slack());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes5_2Slack

            timer.Restart();
            CalculateAndShow(SampleGrids.IEEE_14());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-14

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes15_3PV());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes15_3PV

            timer.Restart();
            CalculateAndShow(SampleGrids.IEEE_57());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-57

            timer.Restart();
            CalculateAndShow(SampleGrids.IEEE_118());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-118            

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes197_36PV());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes197_36PV

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes300_27PV());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes300_27PV

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes398_35PV());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes398_35PV

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes398_35PV_ZIP());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes398_35PV_ZIP

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes874_143PV());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes874_143PV  

            timer.Restart();
            CalculateAndShow(SampleGrids.Nodes1350_250PV());
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes1350_250PV

            timer.Restart();
            SampleGrids.Nodes2628_50PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions { IterationsCount = 4 })
                                        .ApplySolver(SolverType.NewtonRaphson)
                                        .Calculate();
            Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes2628_50PV

            #endregion

            #region Parallel calcs            

            // ----Parallel calc from box----

            //timer.Restart();
            //var list = CreateSamplesList();    // Sample Grids collection
            //Engine.Calculate(list);       // Parallel colection
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");

            //timer.Restart();
            //var list = Create398NodesGridList(50);    // 398nodes Grids collection (25 items)
            //list.Calculate();       // Parallel colection
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");

            #endregion

            #region Multiple solvers calcs

            //// ---- Multiple solvers calcs ----
            //timer.Restart();
            //var list = CreateSamplesList();    // Sample Grids collection
            //list.ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 1 })   // Parallel with multi solver
            //    .ApplySolver(SolverType.NewtonRaphson)
            //    .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");

            //timer.Restart();
            //SampleGrids.Nodes4_1PV_ZIP().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 3 })
            //                            .ApplySolver(SolverType.NewtonRaphson)
            //                            .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV_ZIP

            //timer.Restart();
            //SampleGrids.IEEE_14().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
            //                            .ApplySolver(SolverType.NewtonRaphson)
            //                            .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV_ZIP

            //timer.Restart();
            //SampleGrids.IEEE_118().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
            //                            .ApplySolver(SolverType.NewtonRaphson)
            //                            .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV_ZIP

            //timer.Restart();
            //SampleGrids.Nodes197_36PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
            //                           .ApplySolver(SolverType.NewtonRaphson)
            //                           .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes197_36PV

            //timer.Restart();
            //SampleGrids.Nodes300_27PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
            //                           .ApplySolver(SolverType.NewtonRaphson)
            //                           .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes300_27PV

            //timer.Restart();
            //SampleGrids.Nodes398_35PV_ZIP().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 1 })
            //                               .ApplySolver(SolverType.NewtonRaphson)
            //                               .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes398_35PV_ZIP

            //timer.Restart();
            //SampleGrids.Nodes874_143PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 3 })
            //                              .ApplySolver(SolverType.NewtonRaphson)
            //                              .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes874_143PV

            //timer.Restart();
            //SampleGrids.Nodes1350_250PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
            //                             .ApplySolver(SolverType.NewtonRaphson)
            //                             .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes1350_250PV

            //timer.Restart();
            //SampleGrids.Nodes2628_50PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
            //                            .ApplySolver(SolverType.NewtonRaphson)
            //                            .Calculate();
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes1350_250PV


            #endregion

            #region Graph inspects

            //// ---- IsConnected checks ----
            //var list = CreateSamplesList();    // Sample Grid collection
            //foreach (var item in list)
            //    Console.WriteLine(item.Nodes.Count + " nodes - " + item.IsConnected());    // Check connectivity

            #endregion


            Logger.LogInfo("Calculation finished with: " + timer_global.ElapsedMilliseconds + " ms");

            Console.Read();
        }


        #region Helper methods       

        /// <summary>
        /// Make calculus and print calculated params
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object to be calculated</param>
        private static void CalculateAndShow(Grid grid)
        {
            grid.Calculate();  //Performe calculations

            ////Voltage and angle
            //for (int i = 0; i < grid.Nodes.Count; i++)
            //    Console.WriteLine("Node: " + grid.Nodes[i].Num +
            //                      $" {grid.Nodes[i].Type}" +
            //                      " \tV: " + Math.Round(grid.Ucalc[i].Magnitude, 5) +
            //                      "\tAngle: " + Math.Round(grid.Ucalc[i].Phase * 180 / Math.PI, 5));

            //// Load and gen in Nodes
            //Console.WriteLine("\nPowers");
            //for (int i = 0; i < grid.Nodes.Count; i++)
            //    Console.WriteLine("Node: " + grid.Nodes[i].Num +
            //                      $" {grid.Nodes[i].Type}" +
            //                      "\tSload: " + grid.Nodes[i].S_load.ToString() +
            //                      "\tScalc: " + grid.Nodes[i].S_calc.ToString("#.###") +
            //                      "    \tSgen: " + grid.Nodes[i].S_gen.ToString());

            //// Powers in branches
            //Console.WriteLine("\nPower flows");
            //foreach (var item in grid.Branches)
            //    Console.WriteLine("Branch " + item.Start + "-" + item.End +
            //                        "\tStart: " + item.S_start.ToString() +
            //                        "\tEnd: " + item.S_end.ToString());

            //// Currents in branches
            //Console.WriteLine("\nCurrents");
            //foreach (var item in grid.Branches)
            //    Console.WriteLine("Branch " + item.Start + "-" + item.End +
            //                        "\tStart: " + item.I_start.ToString() +
            //                        "\tEnd: " + item.I_end.ToString());


            //Console.WriteLine(grid.GetVoltageDifference().ToVectorString());        // Show voltage differecne in nodes
            //Console.WriteLine(grid.GetAngleAbsoluteDifference().ToVectorString());  // Show angle differecne in branches
        }


        /// <summary>
        /// Create IEnumerable grid collection
        /// </summary>
        public static IEnumerable<Grid> CreateSamplesList()
        {
            List<Grid> grids = new List<Grid>();

            grids.Add(SampleGrids.Test_Ktr());
            grids.Add(SampleGrids.Nodes4_1PV());
            grids.Add(SampleGrids.Nodes4_1PV_ZIP());
            grids.Add(SampleGrids.Nodes5_2Slack());
            grids.Add(SampleGrids.IEEE_14());
            grids.Add(SampleGrids.Nodes15_3PV());
            grids.Add(SampleGrids.IEEE_57());
            grids.Add(SampleGrids.IEEE_118());
            grids.Add(SampleGrids.Nodes197_36PV());
            grids.Add(SampleGrids.Nodes300_27PV());
            grids.Add(SampleGrids.Nodes398_35PV());
            grids.Add(SampleGrids.Nodes398_35PV_ZIP());
            grids.Add(SampleGrids.Nodes874_143PV());
            grids.Add(SampleGrids.Nodes1350_250PV());
            grids.Add(SampleGrids.Nodes2628_50PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions { IterationsCount = 4 })
                                                  .ApplySolver(SolverType.NewtonRaphson));

            return grids;
        }


        /// <summary>
        /// Create IEnumerable grid collection (398nodes)
        /// </summary>
        public static IEnumerable<Grid> Create398NodesGridList(int items = 25)
        {
            List<Grid> grids = new List<Grid>();

            for (int i = 0; i < items; i++)
                grids.Add(SampleGrids.Nodes398_35PV_ZIP());

            return grids;
        }


        /// <summary>
        /// Logger event handling and print message
        /// </summary>
        private static void Logger_OnLogBroadcast(string senderID, LoggerMessage message) => Console.WriteLine(message.Message);

        #endregion
    }
}
