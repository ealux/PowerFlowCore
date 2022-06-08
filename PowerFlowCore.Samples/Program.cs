using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;


namespace PowerFlowCore.Samples
{
    class Program
    {
        static void Main()
        {
            var timer_global = Stopwatch.StartNew();

            //Logger.AddConsoleMode();
            //Logger.AddDebugMode();
            //Logger.LogInfo("Calculation started");

            //var timer = Stopwatch.StartNew();

            //// Nodes4_1PV
            //CalculateAndShow(SampleGrids.Nodes4_1PV());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes4_1PV

            //timer.Restart();
            //// IEEE-14
            //CalculateAndShow(SampleGrids.IEEE_14());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-14

            //timer.Restart();
            //// Nodes15_3PV
            //CalculateAndShow(SampleGrids.Nodes15_3PV());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Nodes15_3PV

            //timer.Restart();
            //// IEEE-57
            //CalculateAndShow(SampleGrids.IEEE_57());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-57

            //timer.Restart();
            //// IEEE-118
            //CalculateAndShow(SampleGrids.IEEE_118());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // IEEE-118

            //timer.Restart();
            //// Test Complex Ktr
            //CalculateAndShow(SampleGrids.Test_Ktr());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  // Complex Ktr test


            //timer.Restart();
            //// IEEE-300
            //CalculateAndShow(SampleGrids.IEEE_300());
            //Logger.LogInfo("Calc End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result



            // Parallel calc
            Logger.LogBroadcast += Logger_OnLogBroadcast; // Logger event listener
            Parallel.Invoke(
                () => CalculateAndShow(SampleGrids.IEEE_14()),
                () => CalculateAndShow(SampleGrids.Nodes15_3PV()),
                () => CalculateAndShow(SampleGrids.IEEE_57()),
                () => CalculateAndShow(SampleGrids.IEEE_118()),
                () => CalculateAndShow(SampleGrids.Test_Ktr())
            );

            Logger.LogInfo("Calculation finished with: " + timer_global.ElapsedMilliseconds + " ms");

            Console.ReadKey();
        }


        // Logger event handling and print message
        private static void Logger_OnLogBroadcast(string senderID, string message) => Console.WriteLine(message);



        /// <summary>
        /// Make calculus and print calculated params
        /// </summary>
        /// <param name="e"><see cref="Engine"/> object to be calculated</param>
        private static void CalculateAndShow(Engine e)
        {
            e.Calculate();                                                       //Performe calculations
            var calc = e.Grid.Ucalc;                                             //Take calculated U values

            // Voltage and angle
            //for (int i = 0; i < e.Grid.Nodes.Count; i++)
            //    Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
            //                      $" {e.Grid.Nodes[i].Type}" +
            //                      " \tV: " + Math.Round(e.Grid.Ucalc[i].Magnitude, 5) +
            //                      "\tAngle: " + Math.Round(e.Grid.Ucalc[i].Phase * 180 / Math.PI, 5));

            //// Load and gen in Nodes
            //Console.WriteLine("\nPowers");
            //for (int i = 0; i < e.Grid.Nodes.Count; i++)
            //    Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
            //                      $" {e.Grid.Nodes[i].Type}" +
            //                      "\tSload: " + e.Grid.Nodes[i].S_load.ToString() +
            //                      "    \tSgen: " + e.Grid.Nodes[i].S_gen.ToString());

            //// Powers in branches
            //Console.WriteLine("\nPower flows");
            //foreach (var item in e.Grid.Branches)
            //    Console.WriteLine("Branch " + item.Start + "-" + item.End +
            //                        "\tStart: " + item.S_start.ToString() +
            //                        "\tEnd: " + item.S_end.ToString());

            //// Currents in branches
            //Console.WriteLine("\nCurrents");
            //foreach (var item in e.Grid.Branches)
            //    Console.WriteLine("Branch " + item.Start + "-" + item.End +
            //                        "\tStart: " + item.I_start.ToString() +
            //                        "\tEnd: " + item.I_end.ToString());


            //Console.WriteLine(e.Grid.GetVoltageDifference().ToVectorString());        // Show voltage differecne in nodes
            //Console.WriteLine(e.Grid.GetAngleAbsoluteDifference().ToVectorString());  // Show angle differecne in branches
        }
    }
}
