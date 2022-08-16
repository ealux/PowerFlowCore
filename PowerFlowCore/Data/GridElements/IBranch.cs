using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        /// Start node num
        /// </summary>
        int Start { get; set; }

        /// <summary>
        /// Start node num (for calculation)
        /// </summary>
        int Start_calc { get; set; }

        /// <summary>
        /// End node num
        /// </summary>
        int End { get; set; }

        /// <summary>
        /// End node num (for calculation)
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
        Complex Ktr { get; set; }

        /// <summary>
        /// Power flow on branch start side
        /// </summary>
        Complex S_start { get; set; }

        /// <summary>
        /// Power flow on branch end side
        /// </summary>
        Complex S_end { get; set; }

        /// <summary>
        /// Ccurrent on branch start side
        /// </summary>
        Complex I_start { get; set; }

        /// <summary>
        /// Current on branch end side
        /// </summary>
        Complex I_end { get; set; }
    }
}
