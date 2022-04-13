using System;
using System.Collections.Generic;
using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace UnitTest
{
    public static class TwoNodesGrid
    {
        public static Grid PV_110()
        {        
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,
                    Unom=Complex.FromPolarCoordinates(115, 0), U = 115,},
                new Node(){Num = 2,   Type = NodeType.PV,  
                    Unom=115, Vpre = 115, U = Complex.FromPolarCoordinates(115, 3.66*Math.PI/180),
                    S_load = new Complex(0,0), S_gen = new Complex(40, 0), Q_min = -30, Q_max = 30},
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
            };

            Grid net = new Grid(nodes, branches);

            return net;
        }

        public static Grid PQ_110()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,
                    Unom=115, U = 115,},
                new Node(){Num = 2,   Type = NodeType.PQ,   
                    Unom=115,     U = Complex.FromPolarCoordinates(109.9779508833, -3.2496304386*Math.PI/180),
                    S_load = new Complex(40, 20) },
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
            };

            Grid net = new Grid(nodes, branches);

            return net;
        }


        public static Grid Trans_PV_110()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,
                    Unom=Complex.FromPolarCoordinates(115, 0), U = 115,},
                new Node(){Num = 2,   Type = NodeType.PV,
                    Unom=10, Vpre = 11, U = Complex.FromPolarCoordinates(11, 4.502025177995*Math.PI/180),
                    S_load = new Complex(0,0), S_gen = new Complex(40, 0), Q_min = -50, Q_max = 50},
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=1/(new Complex(1, 55)),  Ktr=0.0913,      Ysh=new Complex(0, -50e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(1, 55)),  Ktr=0.0913,      Ysh=new Complex(0, -50e-6)},
            };

            Grid net = new Grid(nodes, branches);

            return net;
        }

        public static Grid Trans_PQ_110()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,
                    Unom=115, U = 115,},
                new Node(){Num = 2,   Type = NodeType.PQ,
                    Unom=10.5,     U = Complex.FromPolarCoordinates(9.984097951438, -4.972297034073*Math.PI/180),
                    S_load = new Complex(40, 20) },
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=1/(new Complex(1, 55)),  Ktr=0.0913,      Ysh=new Complex(0, -50e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(1, 55)),  Ktr=0.0913,      Ysh=new Complex(0, -50e-6)},
            };

            Grid net = new Grid(nodes, branches);

            return net;
        }

        public static Grid PV_PQmax_load_110()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,
                    Unom=115, U = 115,},
                new Node(){Num = 2,   Type = NodeType.PV,
                    Unom=115,     U = Complex.FromPolarCoordinates(111.575672776827, 3.720055112106*Math.PI/180),
                    S_load = new Complex(5, 60), S_gen= new Complex(40, 30), Q_min = -30, Q_max = 30},

            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
            };

            Grid net = new Grid(nodes, branches);

            return net;
        }

        public static Grid PV_PQmin_load_110()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,
                    Unom=115, U = 115,},
                new Node(){Num = 2,   Type = NodeType.PV,
                    Unom=115,     U = Complex.FromPolarCoordinates(115.610144279568, 3.109643165109*Math.PI/180),
                    S_load = new Complex(5, 3), S_gen= new Complex(40, -5), Q_min = -5, Q_max = 30},

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