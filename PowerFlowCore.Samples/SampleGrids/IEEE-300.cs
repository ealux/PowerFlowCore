using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Engine IEEE_300()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("IEEE-300");
            Console.WriteLine("===========================================");

            List<INode> nodes = new List<INode>()
            {

            };

            List<IBranch> branches = new List<IBranch>()
            {
                
            };

            var options = new CalculationOptions();             //Create options
            var engine = new Engine(nodes, branches, options);  //Create engine

            return engine;
        }
    }
}
