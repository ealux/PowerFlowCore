using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        /// <summary>
        /// Scheme with 3 breakers only
        /// </summary>
        /// <returns></returns>
        public static Grid BreakersScheme()
        {
            Logger.LogInfo(" ================================================ ");
            Logger.LogInfo("4 nodes (Breakers only): 0 - PV  3 - PQ  1 - Slack");
            Logger.LogInfo(" ================================================ ");

            var nodes = new List<Node>()
            {
                new Node(){ Num = 1,Type = NodeType.Slack, Unom=Complex.FromPolarCoordinates(110, 0), S_gen = new Complex(109.979, 150.369)},
                new Node(){Num = 2,  Type = NodeType.PQ,    Unom=110},
                new Node(){Num = 3,  Type = NodeType.PQ,    Unom=110},
                new Node(){Num = 4,  Type = NodeType.PQ,    Unom=110, S_load = new Complex(110, 150)},
            };

            var branches = new List<Branch>()
            {

                new Branch(){Start = 1, End = 2, Ktr=1 },
                new Branch(){Start = 2, End = 3, Ktr=1 },
                new Branch(){Start = 3, End = 4, Ktr=1 },
            };

            var grid = new Grid(nodes, branches);


            return grid;
        }
    }
}
