﻿using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Grid Nodes4_1PV_ZIP()
        {
            Logger.LogInfo("===========================================");
            Logger.LogInfo("4 nodes (ZIP): 1 - PV  2 - PQ  1 - Slack");
            Logger.LogInfo("===========================================");

            var nodes = new List<Node>()
            {
                new Node(){Num = 1, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10, 15), LoadModelNum = 1},
                new Node(){Num = 2, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10, 40), LoadModelNum = 1},    // with ZIP
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

            var SLM = new Dictionary<int, IStaticLoadModel>()
            {
                [1] = ZIP.Initialize("Test zip",
                                     p0: 0.6, p1: 0.2, p2: 0.2,
                                     q0: 0.6, q1: 0.2, q2: 0.2,
                                     umin: 0.965, umax: 0.974)
                          .AddModel(ZIP.Initialize("Test zip - Child 1",
                                                   p0: 0.7, p1: 0.15, p2: 0.15,
                                                   q0: 0.7, q1: 0.15, q2: 0.15,
                                                   umin: 0.975, umax: 0.987))
                          .AddModel(ZIP.Initialize("Test zip - Child 2",
                                                   p0: 0.8, p1: 0.1, p2: 0.1,
                                                   q0: 0.8, q1: 0.1, q2: 0.1,
                                                   umin: 0.988, umax: 1.1))
            };

            var grid = new Grid(nodes, branches);
            grid.LoadModels = SLM;

            return grid;
        }
    }
}
