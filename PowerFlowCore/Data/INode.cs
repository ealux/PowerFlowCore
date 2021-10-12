using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Node contracts for calculation core
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Node number (initial)
        /// </summary>
        int Num { get; set; }

        /// <summary>
        /// Node number (for calculation)
        /// </summary>
        int Num_calc { get; set; }

        /// <summary>
        /// Node type
        /// </summary>
        NodeType Type { get; set; }

        /// <summary>
        /// Calculated voltage
        /// </summary>
        Complex U { get; set; }

        /// <summary>
        /// Preset voltage level (necessary for PV nodes)
        /// </summary>
        double Vpre { get; set; }

        /// <summary>
        /// Nominal voltage
        /// </summary>
        Complex Unom { get; set; }

        /// <summary>
        /// Consumption
        /// </summary>
        Complex S_load{ get; set; }

        /// <summary>
        /// Generation
        /// </summary>
        Complex S_gen { get; set; }

        /// <summary>
        /// Minimal Q generation constraint for PV nodes
        /// </summary>
        double Q_min { get; set; }

        /// <summary>
        /// Maximal Q generation constraint for PV nodes
        /// </summary>
        double Q_max { get; set; }

        /// <summary>
        /// Admittance (shunt)
        /// </summary>
        Complex Ysh { get; set; }
    }
}
