using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Financial;
using MathNet.Numerics.LinearAlgebra;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Branch contracts for calculation core
    /// </summary>
    public interface IBranch
    {
        /// <summary>
        /// Start node (initial)
        /// </summary>
        int Start { get; set; }

        /// <summary>
        /// Start node (for calculation)
        /// </summary>
        int Start_calc { get; set; }


        /// <summary>
        /// End node
        /// </summary>
        int End { get; set; }

        /// <summary>
        /// End node (for calculation)
        /// </summary>
        int End_calc { get; set; }


        /// <summary>
        /// Admittance
        /// </summary>
        Complex Y { get; set; }

        /// <summary>
        /// Shunt admittance
        /// </summary>
        Complex Ysh { get; set; }


        /// <summary>
        /// Count of parralel branches
        /// </summary>
        int Count { get; set; }


        /// <summary>
        /// Transformation ratio
        /// </summary>
        double Ktr { get; set; }


        /// <summary>
        /// Power on branch start
        /// </summary>
        Complex S_start { get; set; }

        /// <summary>
        /// Power on branch end
        /// </summary>
        Complex S_end { get; set; }

        /// <summary>
        /// Ccurrent on branch start
        /// </summary>
        Complex I_start { get; set; }

        /// <summary>
        /// Current on branch end
        /// </summary>
        Complex I_end { get; set; }
    }

    public class PFBranch : IBranch
    {
        public int Start { get; set; }
        public int Start_calc { get; set; }
        public int End { get; set; }
        public int End_calc { get; set; }
        public Complex Y { get; set; }
        public Complex Ysh { get; set; }
        public double Ktr { get; set; }
        public int Count { get; set; }


        public Complex S_start { get; set; }
        public Complex S_end { get; set; }
        public Complex I_start { get; set; }
        public Complex I_end { get; set; }
    }
}
