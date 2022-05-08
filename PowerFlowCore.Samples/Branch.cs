using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    class Branch : IBranch
    {
        public int Start { get; set; }
        public int Start_calc { get; set; }
        public int End { get; set; }
        public int End_calc { get; set; }
        public Complex Y { get; set; }
        public Complex Ysh { get; set; }
        public Complex Ktr { get; set; }
        public int Count { get; set; }


        public Complex S_start { get; set; }
        public Complex S_end { get; set; }
        public Complex I_start { get; set; }
        public Complex I_end { get; set; }
    }
}
