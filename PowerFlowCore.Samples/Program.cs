using System;
using System.Diagnostics;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            // IEEE-14
            CalculateAndShow(SampleGrids.IEEE_14());
            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            //Restart timer
            timer.Restart();
            // Nodes15_3PV
            CalculateAndShow(SampleGrids.Nodes15_3PV());
            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            //Restart timer
            timer.Restart();
            // IEEE-57
            CalculateAndShow(SampleGrids.IEEE_57());
            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            //Restart timer
            timer.Restart();
            // IEEE-118
            CalculateAndShow(SampleGrids.IEEE_118());
            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            ////Restart timer
            //timer.Restart();
            //// IEEE-300
            //CalculateAndShow(SampleGrids.IEEE_300());
            //Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result


            //// Test Complex Ktr
            //CalculateAndShow(SampleGrids.Test_Ktr());
            //Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            Console.ReadKey();
        }


        /// <summary>
        /// Make calculus and print calculated params
        /// </summary>
        /// <param name="e"><see cref="Engine"/> object to be calculated</param>
        private static void CalculateAndShow(Engine e)
        {
            e.Calculate();                                                       //Performe calculations
            var calc = e.Grid.Ucalc;                                             //Take calculated U values
            for (int i = 0; i < e.Grid.Nodes.Count; i++)
                Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
                                  $" {e.Grid.Nodes[i].Type}" +
                                  " \tV: " + Math.Round(e.Grid.Ucalc[i].Magnitude, 5) +
                                  "\tAngle: " + Math.Round(e.Grid.Ucalc[i].Phase * 180 / Math.PI, 5));
            Console.WriteLine("Powers");
            for (int i = 0; i < e.Grid.Nodes.Count; i++)
                Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
                                  $" {e.Grid.Nodes[i].Type}" +                                  
                                  "\tSload: " + e.Grid.Nodes[i].S_load.ToString() +
                                  "    \tSgen: "  + e.Grid.Nodes[i].S_gen.ToString());
        }
    }
}
