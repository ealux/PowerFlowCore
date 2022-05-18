using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Engine IEEE_14()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("IEEE-14: 4 - PV  9 - PQ  1 - Slack");
            Console.WriteLine("===========================================");

            List<INode> nodes = new List<INode>()
            {
                new Node(){Num = 1,  Type = NodeType.Slack, Unom=Complex.FromPolarCoordinates(243.8, 0)},
                new Node(){Num = 2,  Type = NodeType.PV,    Unom=230, Vpre = 240.35,   S_load = new Complex(21.7, 12.7),  S_gen = new Complex(40, 0), Q_min = -40, Q_max = 50},
                new Node(){Num = 3,  Type = NodeType.PV,    Unom=230, Vpre = 232.30,   S_load = new Complex(94.2, 19.0),  S_gen = new Complex(0, 0),  Q_min = 0.0, Q_max = 40},
                new Node(){Num = 4,  Type = NodeType.PQ,    Unom=230, S_load = new Complex(47.8, -3.9)},
                new Node(){Num = 5,  Type = NodeType.PQ,    Unom=230, S_load = new Complex(7.6, 1.6)},
                new Node(){Num = 6,  Type = NodeType.PV,    Unom=115, Vpre = 123.05,   S_load = new Complex(11.2, 7.5),  S_gen = new Complex(0, 0),  Q_min = -6, Q_max = 24},
                new Node(){Num = 7,  Type = NodeType.PQ,    Unom=115},
                new Node(){Num = 8,  Type = NodeType.PV,    Unom=115, Vpre = 125.35,   S_load = new Complex(0, 0),  S_gen = new Complex(0, 0),  Q_min = -6, Q_max = 24},
                new Node(){Num = 9,  Type = NodeType.PQ,    Unom=115, S_load = new Complex(29.5, 16.6), Ysh = new Complex(0, 1436.67e-6)},
                new Node(){Num = 10, Type = NodeType.PQ,    Unom=115, S_load = new Complex(9.0, 5.8)},
                new Node(){Num = 11, Type = NodeType.PQ,    Unom=115, S_load = new Complex(3.5, 1.8)},
                new Node(){Num = 12, Type = NodeType.PQ,    Unom=115, S_load = new Complex(6.1, 1.6)},
                new Node(){Num = 13, Type = NodeType.PQ,    Unom=115, S_load = new Complex(13.5, 5.8)},
                new Node(){Num = 14, Type = NodeType.PQ,    Unom=115, S_load = new Complex(14.9, 5.0)},
            };

            List<IBranch> branches = new List<IBranch>()
            {
                new Branch(){Start=1, End=2,   Y=1/(new Complex(10.252, 31.3009)),   Ktr=1,     Ysh=new Complex(0, 99.8e-6)},
                new Branch(){Start=1, End=5,   Y=1/(new Complex(28.5819, 117.9882)), Ktr=1,     Ysh=new Complex(0, 93.0e-6)},
                new Branch(){Start=2, End=3,   Y=1/(new Complex(24.8577, 104.7261)), Ktr=1,     Ysh=new Complex(0, 82.8e-6)},
                new Branch(){Start=2, End=4,   Y=1/(new Complex(30.7402, 93.2733)),  Ktr=1,     Ysh=new Complex(0, 64.3e-6)},
                new Branch(){Start=2, End=5,   Y=1/(new Complex(30.1266, 91.9825)),  Ktr=1,     Ysh=new Complex(0, 65.4e-6)},
                new Branch(){Start=3, End=4,   Y=1/(new Complex(35.4483, 90.4749)),  Ktr=1,     Ysh=new Complex(0, 24.2e-6)},
                new Branch(){Start=4, End=5,   Y=1/(new Complex(7.0622, 22.2762)),   Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=4, End=7,   Y=1/(new Complex(0, 105.8105)),  Ktr=0.511247,   Ysh=new Complex(0, 0)},
                new Branch(){Start=4, End=9,   Y=1/(new Complex(0, 276.2604)),  Ktr=0.515996,   Ysh=new Complex(0, 0)},
                new Branch(){Start=5, End=6,   Y=1/(new Complex(0, 115.8037)),  Ktr=0.536481,   Ysh=new Complex(0, 0)},
                new Branch(){Start=6, End=11,  Y=1/(new Complex(12.5611, 26.3045)),  Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=6, End=12,  Y=1/(new Complex(16.2548, 33.8309)),  Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=6, End=13,  Y=1/(new Complex(8.7483, 17.2282)),   Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=7, End=8,   Y=1/(new Complex(0, 23.2958)),        Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=7, End=9,   Y=1/(new Complex(0, 14.5488)),        Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=9, End=10,  Y=1/(new Complex(4.2069, 11.1751)),   Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=9, End=14,  Y=1/(new Complex(16.8103, 35.7578)),  Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=10, End=11, Y=1/(new Complex(10.8511, 25.4013)),  Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=12, End=13, Y=1/(new Complex(29.2167, 26.4341)),  Ktr=1,     Ysh=new Complex(0, 0)},
                new Branch(){Start=13, End=14, Y=1/(new Complex(22.6055, 46.0256)),  Ktr=1,     Ysh=new Complex(0, 0)}
            };

            var options = new CalculationOptions();             //Create options
            var engine = new Engine(nodes, branches, options);  //Create engine

            return engine;
        }
    }
}
