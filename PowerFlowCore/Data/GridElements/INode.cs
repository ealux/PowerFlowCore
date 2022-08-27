using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Node contracts for calculation core
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Node number
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
        /// Load
        /// </summary>
        Complex S_load{ get; set; }

        /// <summary>
        /// Number of Load model in <see cref="Grid.LoadModels"/>
        /// </summary>
        public int? LoadModelNum { get; set; }

        /// <summary>
        /// Load, calculated by <see cref="IStaticLoadModel"/>
        /// </summary>
        Complex S_calc { get; set; }

        /// <summary>
        /// Generation
        /// </summary>
        Complex S_gen { get; set; }

        /// <summary>
        /// Minimal Q generation constraint for PV nodes
        /// </summary>
        double? Q_min { get; set; }

        /// <summary>
        /// Maximal Q generation constraint for PV nodes
        /// </summary>
        double? Q_max { get; set; }

        /// <summary>
        /// Admittance (shunt)
        /// </summary>
        Complex Ysh { get; set; }
                
    }
}
