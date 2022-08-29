using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Grid Nodes5_2Slack()
        {
			Logger.LogInfo(" =========================================== ");
			Logger.LogInfo("5 nodes: 0 - PV  3 - PQ  2 - Slack");
			Logger.LogInfo(" =========================================== ");
			var nodes = new List<INode>()
			{
				new Node(){ Num = 1,Type = NodeType.Slack, Unom=Complex.FromPolarCoordinates(110, 0), S_gen = new Complex(25.072, 35.754)},
				new Node(){Num = 2,  Type = NodeType.PQ,    Unom=110, S_load = new Complex(10, 15)},
				new Node(){Num = 3,  Type = NodeType.PQ,    Unom=110, S_load = new Complex(7.5, 13)},
				new Node(){ Num = 4,Type = NodeType.Slack, Unom=Complex.FromPolarCoordinates(110, 0), S_gen = new Complex(2.488, 4.413)},
				new Node(){Num = 5,  Type = NodeType.PQ,    Unom=110, S_load = new Complex(10, 12)},
			};

			var branches = new List<IBranch>()
			{
				new Branch(){Start = 1, End = 2, Ktr=1, Y=1/new Complex(0.2, 0.45)},
				new Branch(){Start = 2, End = 5, Ktr=1, Y=1/new Complex(1.5, 2.6)},
				new Branch(){Start = 2, End = 3, Ktr=1, Y=1/new Complex(0.15, 2.6)},
				new Branch(){Start = 3, End = 5, Ktr=1, Y=1/new Complex(1.25, 2.7)},
				new Branch(){Start = 3, End = 4, Ktr=1, Y=1/new Complex(2.1, 10.5)},
			};

			var grid = new Grid(nodes, branches);

			return grid;
		}
    }
}
