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


    /// <summary>
    /// Type of node
    /// PQ -> Load node
    /// PV -> Generation node
    /// Slack -> Slack/Reference node
    /// </summary>
    public enum NodeType
    {
        PQ = 1,     //U - var;    P - const;  Q - const
        PV = 2,     //U - const;  P - const;  Q - var (constrained)
        Slack = 3   //U - const;  P - var;    Q - var
    }


    public class PFNode : INode
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
