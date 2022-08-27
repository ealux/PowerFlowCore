using PowerFlowCore.Data;
using System;
using System.Collections.Generic;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Tests
{
    public static class SmallNodes
    {
        public static Grid PV_110()
        {        
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,
                    Unom=Complex.FromPolarCoordinates(115, 0), U = 115,},
                new Node(){Num = 2,   Type = NodeType.PV,  
                    Unom=115, Vpre = 115, U = Complex.FromPolarCoordinates(115, -0.091037899895*Math.PI/180),
                    S_load = new Complex(0,0), S_gen = new Complex(40, 0), Q_min = -30, Q_max = 30},
                new Node(){Num = 3,   Type = NodeType.PQ,
                    Unom=115, U = Complex.FromPolarCoordinates(108.036734805267, -3.165691440915*Math.PI/180),
                    S_load = new Complex(40,30)},
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=3, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=3, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
            };

            Grid net = new Grid(nodes, branches);

            return net;
        }

        public static Complex[,] PV_110_AdmittanceMatrix()
        {
            Complex[,] Y = new Complex[3,3];
            //Y*10^3
            Y[0, 0] = new Complex(-11.76471, 46.7782);
            Y[1, 0] = new Complex(11.76471, -46.7782);
            Y[2, 0] = new Complex(0, 0);
            Y[0, 1] = new Complex(11.76471, -46.7782);
            Y[1, 1] = new Complex(-23.52941, 93.55565);
            Y[2, 1] = new Complex(11.76471, -46.7782);
            Y[0, 2] = new Complex(0, 0);
            Y[1, 2] = new Complex(11.76471, -46.7782);
            Y[2, 2] = new Complex(-11.76471, 46.7782);

            for (int i = 0; i < Y.GetLength(0); i++)
            {
                for (int j = 0; j < Y.GetLength(1); j++)
                {
                    Y[i,j] = -Y[i,j] * 0.001;
                }
            }

            return Y;
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
                new Node(){Num = 1,   Type = NodeType.PQ,
                    Unom=Complex.FromPolarCoordinates(115, 0), U = Complex.FromPolarCoordinates(113.140964174093, -4.502098177134*Math.PI/180),},
                new Node(){Num = 3,   Type = NodeType.PV,
                    Unom=10, Vpre = 10.5, U = Complex.FromPolarCoordinates(10.5, -4.374196802339*Math.PI/180),
                    S_load = new Complex(0,0), S_gen = new Complex(40, 0), Q_min = -50, Q_max = 50},
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=2, End=3,    Y=1/(new Complex(1, 55)),  Ktr=0.0913,      Ysh=new Complex(0, -50e-6)},
                new Branch(){Start=3, End=3,    Y=1/(new Complex(1, 55)),  Ktr=0.0913,      Ysh=new Complex(0, -50e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
                new Branch(){Start=1, End=2,    Y=1/(new Complex(10, 40)),  Ktr=1,      Ysh=new Complex(0, 281e-6)},
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
                new Node(){Num = 2,   Type = NodeType.PQ,
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
                new Node(){Num = 2,   Type = NodeType.PQ,
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