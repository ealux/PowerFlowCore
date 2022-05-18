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
                                  //"\tPload: " + Math.Round(e.Grid.Nodes[i].S_load.Real, 3) +
                                  //"\tQload: " + Math.Round(e.Grid.Nodes[i].S_load.Imaginary, 3) +
                                  //"\tPgen: " + Math.Round(e.Grid.Nodes[i].S_gen.Real, 3) +
                                  //"Qgen: "  + Math.Round(e.Grid.Nodes[i].S_gen.Imaginary, 3));
            Console.WriteLine("Powers");
            for (int i = 0; i < e.Grid.Nodes.Count; i++)
                Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
                                  $" {e.Grid.Nodes[i].Type}" +                                  
                                  "\tPload: " + Math.Round(e.Grid.Nodes[i].S_load.Real, 3) +
                                  "\tQload: " + Math.Round(e.Grid.Nodes[i].S_load.Imaginary, 3) +
                                  "\tPgen: " + Math.Round(e.Grid.Nodes[i].S_gen.Real, 3) +
                                  "\t\tQgen: " + Math.Round(e.Grid.Nodes[i].S_gen.Imaginary, 3));
            //Console.WriteLine(e.Grid.S);                                         //Show vector of calculated S
            //e.Grid.Nodes.ForEach(n => Console.WriteLine(n.S_gen.ToString()));    //Show Nodes numbers
        }
    }
}
