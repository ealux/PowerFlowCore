using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Tests
{
    class Node : INode
    {
        public int Num { get; set; }
        public int Num_calc { get; set; }
        public NodeType Type { get; set; }
        public Complex U { get; set; }
        public Complex Unom { get; set; }
        public Complex Ysh { get; set; }
        public Complex S_load { get; set; }
        public Complex S_gen { get; set; }
        public double Q_min { get; set; }
        public double Q_max { get; set; }
        public double Vpre { get; set; }
    }
}
