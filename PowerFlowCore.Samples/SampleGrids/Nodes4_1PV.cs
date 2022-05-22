using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Engine Nodes4_1PV()
        {
            Logger.LogInfo("===========================================");
            Logger.LogInfo("4 nodes: 1 - PV  2 - PQ  1 - Slack");
            Logger.LogInfo("===========================================");

            var nodes = new List<Node>()
            {
                new Node(){Num = 1, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10, 15)},
                new Node(){Num = 2, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10, 40)},
                new Node(){Num = 3, Type = NodeType.PV,     Unom=110, Vpre = 110, S_load = new Complex(10, 25), S_gen = new Complex(25, 0), Q_min=15, Q_max=35},
                new Node(){Num = 4, Type = NodeType.Slack,  Unom=115, Vpre = 115}
            };

            var branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2, Y=1/(new Complex(10, 2)), Ktr=1, Ysh = new Complex(0, 0)},
                new Branch(){Start=1, End=3, Y=1/(new Complex(10, 20)), Ktr=1,Ysh = new Complex(0, 0)},
                new Branch(){Start=1, End=4, Y=1/(new Complex(8, 15)), Ktr=1, Ysh = new Complex(0, 0)},
                new Branch(){Start=2, End=4, Y=1/(new Complex(20, 40)), Ktr=1,}
            };

            var options = new CalculationOptions();
            var engine  = new Engine(nodes, branches, options);

            return engine;
        }
    }
}
