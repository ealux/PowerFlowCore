using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;


namespace PowerFlowCore.Samples
{
    partial class Program
    {
        static void Main()
        {
            var timer_global = Stopwatch.StartNew();

            //Logger.AddConsoleMode();
            //Logger.AddCustomMode(new CustomLoggerListener()); // Test custom listener
            ////Logger.AddDebugMode();
            //Logger.LogInfo("Calculation started");

            //var timer = Stopwatch.StartNew();

            //CalculateAndShow(SampleGrids.Nodes4_1PV());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV

            //timer.Restart();
            //CalculateAndShow(SampleGrids.Nodes4_1PV_ZIP());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV_ZIP

            //timer.Restart();
            //CalculateAndShow(SampleGrids.IEEE_14());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-14

            //timer.Restart();
            //CalculateAndShow(SampleGrids.Nodes15_3PV());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes15_3PV

            //timer.Restart();
            //CalculateAndShow(SampleGrids.IEEE_57());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-57

            //timer.Restart();
            //CalculateAndShow(SampleGrids.IEEE_118());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-118

            //timer.Restart();
            //CalculateAndShow(SampleGrids.Test_Ktr());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Complex Ktr test

            //timer.Restart();
            //CalculateAndShow(SampleGrids.Nodes197_36PV());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes197_36PV

            //timer.Restart();
            //CalculateAndShow(SampleGrids.Nodes300_27PV());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes300_27PV


            // --- Parallel calc default ----
            Logger.LogBroadcast += Logger_OnLogBroadcast; // Logger event listener
            var grids = new List<Grid>();
            grids.Add(SampleGrids.Test_Ktr());
            grids.Add(SampleGrids.Nodes4_1PV());
            grids.Add(SampleGrids.Nodes4_1PV_ZIP());
            grids.Add(SampleGrids.IEEE_14());
            grids.Add(SampleGrids.Nodes15_3PV());
            grids.Add(SampleGrids.IEEE_57());
            grids.Add(SampleGrids.IEEE_118());
            grids.Add(SampleGrids.Nodes197_36PV());
            grids.Add(SampleGrids.Nodes300_27PV());
            var result = Engine.CalculateDefaultParallel(grids);


            //// ---- Parallel calc ----
            //Logger.LogBroadcast += Logger_OnLogBroadcast; // Logger event listener
            //Parallel.Invoke(
            //    () => CalculateAndShow(SampleGrids.Test_Ktr()),
            //    () => CalculateAndShow(SampleGrids.Nodes4_1PV()),
            //    () => CalculateAndShow(SampleGrids.Nodes4_1PV_ZIP()),
            //    () => CalculateAndShow(SampleGrids.IEEE_14()),
            //    () => CalculateAndShow(SampleGrids.Nodes15_3PV()),
            //    () => CalculateAndShow(SampleGrids.IEEE_57()),
            //    () => CalculateAndShow(SampleGrids.IEEE_118()),
            //    () => CalculateAndShow(SampleGrids.Nodes197_36PV()),
            //    () => CalculateAndShow(SampleGrids.Nodes300_27PV())
            //);

            Logger.LogInfo("Calculation finished with: " + timer_global.ElapsedMilliseconds + " ms");

            Console.ReadKey();
        }


        // Logger event handling and print message
        private static void Logger_OnLogBroadcast(string senderID, LoggerMessage message) => Console.WriteLine(message.Message);


        /// <summary>
        /// Make calculus and print calculated params
        /// </summary>
        /// <param name="e"><see cref="Engine"/> object to be calculated</param>
        private static void CalculateAndShow(Grid grid)
        {
            Engine.CalculateDefault(grid);  //Performe calculations

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
    }
}
