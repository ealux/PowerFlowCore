using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Grid Nodes15_3PV_Decoupled()
        {
            Logger.LogInfo("================================================");
            Logger.LogInfo("15 nodes (Decoupled): 3 - PV  11 - PQ  1 - Slack");
            Logger.LogInfo("================================================");

            var nodes = new List<INode>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,  Unom=Complex.FromPolarCoordinates(121, -5*Math.PI/180)},
                new Node(){Num = 2,   Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15,3)},
                new Node(){Num = 3,   Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15,3)},
                new Node(){Num = 5,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 6,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 7,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 21,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(16, 5)},
                new Node(){Num = 31,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(24, 10.28)},
                new Node(){Num = 51,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(12, 8)},
                new Node(){Num = 61,  Type = NodeType.Slack,  Unom=10.5, Vpre = 0,      S_load = new Complex(10, 10.02)},
                new Node(){Num = 71,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(16, 13)},
                new Node(){Num = 200, Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15, 14.58)},
                new Node(){Num = 201, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 0.0),   S_gen = new Complex(20, 0),  Q_min = 10, Q_max = 15},
                new Node(){Num = 202, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 0.0),   S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15},
                new Node(){Num = 203, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 12.5),  S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15}
            };

            var branches = new List<IBranch>()
            {
                new Branch(){Start=1, End=3,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=1, End=3,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=1, End=5,    Y=1/(new Complex(10.69, 22.68)),Ktr=1,      Ysh=new Complex(0, 145.8e-6)},
                new Branch(){Start=3, End=7,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=3, End=7,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=5, End=7,    Y=1/(new Complex(11.95, 20.5)), Ktr=1,      Ysh=new Complex(0, 127.7e-6)},
                new Branch(){Start=2, End=7,    Y=1/(new Complex(11.95, 20.5)), Ktr=1,      Ysh=new Complex(0, 127.7e-6)},
                new Branch(){Start=2, End=7,    Y=1/(new Complex(11.95, 20.5)), Ktr=1,      Ysh=new Complex(0, 127.7e-6)},
                //new Branch(){Start=2, End=200,  Y=1/(new Complex(2.16, 7.29)),  Ktr=1,      Ysh=new Complex(0, 50.6e-6)},
                //new Branch(){Start=2, End=6,    Y=1/(new Complex(7.47, 12.81)), Ktr=1,      Ysh=new Complex(0, 79.8e-6)},
                new Branch(){Start=6, End=200,  Y=1/(new Complex(10.46, 17.93)),Ktr=1,      Ysh=new Complex(0, 117.7e-6)},
                new Branch(){Start=200, End=201,Y=1/(new Complex(1.46, 38.4)),  Ktr=0.087,  Ysh=new Complex(3.4e-6, -17.8e-6)},
                new Branch(){Start=200, End=202,Y=1/(new Complex(1.46, 38.4)),  Ktr=0.087,  Ysh=new Complex(3.4e-6, -17.8e-6)},
                new Branch(){Start=200, End=203,Y=1/(new Complex(1.46, 38.4)),  Ktr=0.087,  Ysh=new Complex(3.4e-6, -17.8e-6)},
                new Branch(){Start=2, End=21,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=2, End=21,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=3, End=31,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=3, End=31,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=5, End=51,   Y=1/(new Complex(7.95, 139)),   Ktr=0.096,  Ysh=new Complex(1.06e-6, -5.03e-6)},
                new Branch(){Start=5, End=51,   Y=1/(new Complex(7.95, 139)),   Ktr=0.096,  Ysh=new Complex(1.06e-6, -5.03e-6)},
                new Branch(){Start=6, End=61,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=6, End=61,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=7, End=71,   Y=1/(new Complex(1.4, 34.7)),   Ktr=0.091,  Ysh=new Complex(2.72e-6, -19.66e-6)},
                new Branch(){Start=7, End=71,   Y=1/(new Complex(1.4, 34.7)),   Ktr=0.091,  Ysh=new Complex(2.72e-6, -19.66e-6)}
            };

            var grid = new Grid(nodes, branches);

            return grid;
        }
    }
}
