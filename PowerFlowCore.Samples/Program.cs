﻿using System;
using System.Collections.Generic;
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
            CalculateAnShow(SampleGrids.IEEE_14());
            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result


            //Restart timer
            timer.Restart();                

            // Nodes15_3PV
            CalculateAnShow(SampleGrids.Nodes15_3PV());
            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            //Restart timer
            timer.Restart();

            // IEEE-57
            CalculateAnShow(SampleGrids.IEEE_57());
            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            Console.ReadKey();
        }


        /// <summary>
        /// Make calculus and print calculated params
        /// </summary>
        /// <param name="e"><seealso cref="Engine"/> object to be calculated</param>
        private static void CalculateAnShow(Engine e)
        {
            e.Calculate();                                                        //Performe calculations
            var calc = e.Grid.Ucalc;                                             //Take calculated U values
            for (int i = 0; i < e.Grid.Nodes.Count; i++)
                Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
                                  $" {e.Grid.Nodes[i].Type}" +
                                  " \tV: " + Math.Round(e.Grid.Ucalc[i].Magnitude, 6) +
                                  "\tAngle: " + Math.Round(e.Grid.Ucalc[i].Phase * 180 / Math.PI, 6));
            Console.WriteLine(e.Grid.S);                                          //Show vector of calculated S
            e.Grid.Nodes.ForEach(n => Console.WriteLine(n.S_gen.ToString()));     //Show Nodes numbers
        }
    }
}
