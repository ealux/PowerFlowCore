using System;
using System.Collections.Generic;
using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Tests
{
    class IEEE
    {
        public static Grid IEEE_14RTS()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack, Unom=Complex.FromPolarCoordinates(244, 0), U = 115,},
                new Node(){Num = 2,   Type = NodeType.PV, Unom=230, Vpre = 230, U = Complex.FromPolarCoordinates(115, 3.66*Math.PI/180), S_load = new Complex(0,0), S_gen = new Complex(40, 0), Q_min = -30, Q_max = 30},
                new Node(){Num = 3,   Type = NodeType.PV, Unom=230, Vpre = 230},
                new Node(){Num = 4,   Type = NodeType.PQ, Unom=230},
                new Node(){Num = 5,   Type = NodeType.PQ, Unom=230},
                new Node(){Num = 6,   Type = NodeType.PV, Unom=115, Vpre = 115},
                new Node(){Num = 7,   Type = NodeType.PQ, Unom=115},
                new Node(){Num = 8,   Type = NodeType.PV, Unom=115, Vpre = 115},
                new Node(){Num = 9,   Type = NodeType.PQ, Unom=115},
                new Node(){Num = 10,  Type = NodeType.PQ, Unom=115},
                new Node(){Num = 11,  Type = NodeType.PQ, Unom=115},
                new Node(){Num = 12,  Type = NodeType.PQ, Unom=115},
                new Node(){Num = 13,  Type = NodeType.PQ, Unom=115},
                new Node(){Num = 14,  Type = NodeType.PQ, Unom=115},
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
            };

            Grid net = new Grid(nodes, branches);

            return net;
        }
    }
}
