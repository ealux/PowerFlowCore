using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Grid Nodes22_ShuntLoad()
        {
            Logger.LogInfo(" =========================================== ");
            Logger.LogInfo("Nodes22_ShuntLoad: 0 - PV  21 - PQ  1 - Slack");
            Logger.LogInfo(" =========================================== ");
            var nodes = new List<INode>()
            {            
                new Node(){Num = 100101,  Type = NodeType.PQ,    Unom=230},
                new Node(){Num = 100102,  Type = NodeType.PQ,    Unom=230},
                new Node(){Num = 100201,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100202,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100203,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100204,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100205,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100206,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100207,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100208,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100209,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100210,  Type = NodeType.PQ,    Unom=35},
                new Node(){Num = 100301,  Type = NodeType.PQ,    Unom=10},
                new Node(){Num = 100302,  Type = NodeType.PQ,    Unom=10},
                new Node(){Num = 100303,  Type = NodeType.PQ,    Unom=10},
                new Node(){Num = 100304,  Type = NodeType.PQ,    Unom=10},
                new Node(){Num = 100401,  Type = NodeType.PQ,    Unom=1.15, Ysh = new Complex(51_700_000e-6, 32_500_000e-6)},
                new Node(){Num = 100501,  Type = NodeType.PQ,    Unom=0.4},
                new Node(){Num = 110101,  Type = NodeType.PQ,    Unom=230},
                new Node(){Num = 110102,  Type = NodeType.PQ,    Unom=230},
                new Node(){Num = 120101,  Type = NodeType.Slack, Unom=Complex.FromPolarCoordinates(224, 74 * (Math.PI/180.0))},
                new Node(){Num = 120102,  Type = NodeType.PQ,    Unom=230},
            };


            var branches = new List<IBranch>()
            {            
                new Branch(){Start = 100101, End = 100201, Ktr=0.16739, Y=1/new Complex(0.63, 45.362), Ysh = new Complex(2.025e-6, -7.259e-6)},
                new Branch(){Start = 100101, End = 100102, Ktr=1, Y=1/new Complex(2.81, 80.077), Ysh = new Complex(0.577e-6, -0.756e-6)},
                new Branch(){Start = 100102, End = 100301, Ktr=0.047826, Y=1/new Complex(5.62, 160.15)},
                new Branch(){Start = 100102, End = 100301, Ktr=0.047826, Y=1/new Complex(5.62, 160.15)},
                new Branch(){Start = 100201, End = 100202, Ktr=1, Y=1/new Complex(0.008, 0.041)},
                new Branch(){Start = 100202, End = 100208, Ktr=1, Y=1/new Complex(0.005, 0.006), Ysh = new Complex(0.444e-6, 19.305e-6)},
                new Branch(){Start = 100202, End = 100205, Ktr=1, Y=1/new Complex(0.002, 0.004), Ysh = new Complex(0.434e-6, 17.46e-6)},
                new Branch(){Start = 100202, End = 100203, Ktr=1, Y=1/new Complex(0.006, 0.008), Ysh = new Complex(0.233e-6, 10.14e-6)},
                new Branch(){Start = 100202, End = 100204, Ktr=1, Y=1/new Complex(0.006, 0.008), Ysh = new Complex(0.233e-6, -10.14e-6)},
                new Branch(){Start = 100205, End = 100207, Ktr=1, Y=1/(new Complex(0, 0.001))},
                new Branch(){Start = 100205, End = 100206, Ktr=1, Y=1/(new Complex(0, 0.001))},
                new Branch(){Start = 100208, End = 100209, Ktr=1, Y=1/new Complex(0.013, 1.11)},
                new Branch(){Start = 100209, End = 100210, Ktr=1, Y=1/new Complex(0.013, 0.017), Ysh = new Complex(1.183e-6, 51.48e-6)},
                new Branch(){Start = 100210, End = 100401, Ktr=0.03285, Y=1/new Complex(0.002, 0.739), Ysh = new Complex(118.204e-6, -341.224e-6)},
                new Branch(){Start = 100301, End = 100302, Ktr=1, Y=1/new Complex(0.014, 0.067)},
                new Branch(){Start = 100302, End = 100303, Ktr=1, Y=1/new Complex(0.041, 0.019), Ysh = new Complex(12.79e-6, 1752e-6)},
                new Branch(){Start = 100302, End = 100304, Ktr=1, Y=1/new Complex(0.004, 0.002), Ysh = new Complex(0.307e-6, 42.06e-6)},
                new Branch(){Start = 100303, End = 100501, Ktr=0.04, Y=1/new Complex(0.04, 0.345), Ysh = new Complex(215.1e-6, -760e-6)},
                new Branch(){Start = 110101, End = 110102, Ktr=1, Y=1/(new Complex(0, 0.001))},
                new Branch(){Start = 110101, End = 100101, Ktr=1, Y=1/new Complex(4.8, 27.64), Ysh = new Complex(2.044e-6, 177.7e-6)},
                new Branch(){Start = 110101, End = 120101, Ktr=1, Y=1/new Complex(2.36, 12.8), Ysh = new Complex(0.754e-6, 82.9e-6)},
                new Branch(){Start = 110102, End = 120102, Ktr=1, Y=1/new Complex(2.1, 11.05), Ysh = new Complex(0.754e-6, 74.3e-6)},
                new Branch(){Start = 120101, End = 120102, Ktr=1, Y=1/(new Complex(0, 0.001))},
            };


            var grid = new Grid(nodes, branches);


            return grid;


        }
    }
}
