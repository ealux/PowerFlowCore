using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Complex;

using Complex = System.Numerics.Complex;

using PowerFlowCore.Data;
using PowerFlowCore.Extensions;
using System.Linq;

namespace PowerFlowCore
{
    /// <summary>
    /// 
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Flag for calculation necessity
        /// </summary>
        public bool NeedsToCalc { get; set; } = true;

        /// <summary>
        /// Grid item
        /// </summary>
        public Grid desc { get; private set; }

        /// <summary>
        /// Engine calc parameters
        /// </summary>
        public EngineOptions options { get; set; } = new EngineOptions();


        /// <summary>
        /// Initiate Engine object with special parameters
        /// </summary>
        public Engine(IConverter converter, EngineOptions options = null)
        {
            IEnumerable<INode> nodes = converter.Nodes;
            IEnumerable<IBranch> branches = converter.Branches;

            this.desc = new Grid(nodes, branches);
            //Set options
            if (options != null) this.options = options; 
        }


        //TO TEST  !!!
        public Engine(IEnumerable<INode> nodes, IEnumerable<IBranch> branches)
        {
            this.desc = new Grid(nodes, branches);
        }


        /// <summary>
        /// Steady state mode calculation
        /// </summary>
        public void Calculate()
        {
            try
            {
                if (desc.Nodes.Any(n => n.Type == NodeType.PV))
                {
                    this.desc.U_calc = desc.GaussSeidelSolver(initialGuess: desc.U_init,
                                                              accuracy: options.Accuracy,
                                                              iterations: options.IterationsCount,
                                                              voltageRatio: options.VotageRatio);
                    this.desc.CalculatePowerMatrix();
                    this.NeedsToCalc = false;
                }
                else
                {
                    this.desc.U_calc = desc.NewtonRaphsonSolver(initialGuess: desc.U_init,
                                                                accuracy: options.Accuracy,
                                                                voltageConvergence: options.VoltageConvergence,
                                                                iterations: options.IterationsCount,
                                                                voltageRatio: options.VotageRatio);
                    this.desc.CalculatePowerMatrix();
                    this.NeedsToCalc = false;
                }
            }
            catch (Exception e)
            {
                this.desc.U_calc = this.desc.U_init;
                this.NeedsToCalc = true;
                throw e;
            }            
        }       

    }


    public enum CalculationResult
    {

    }
}
