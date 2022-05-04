using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Complex;

using Complex = System.Numerics.Complex;

using PowerFlowCore.Data;
using PowerFlowCore.Solvers;
using PowerFlowCore.Extensions;
using System.Linq;

namespace PowerFlowCore
{
    /// <summary>
    /// Encupsulate grid-solver interface
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
        public Grid Grid { get; private set; }

        /// <summary>
        /// Engine calculus options
        /// </summary>
        public CalculationOptions Options { get; set; } = new CalculationOptions();


        
        public Engine(IConverter converter, CalculationOptions options = null)
        {
            IEnumerable<INode> nodes = converter.Nodes;
            IEnumerable<IBranch> branches = converter.Branches;

            this.Grid = new Grid(nodes, branches);
            
            if (options != null) this.Options = options; 
        }


        /// <summary>
        /// Initiate Engine object with special parameters
        /// </summary>
        /// <param name="nodes">INode object collection</param>
        /// <param name="branches">IBranch object collection</param>
        /// <param name="options">Calculation options</param>
        public Engine(IEnumerable<INode> nodes, 
                      IEnumerable<IBranch> branches,
                      CalculationOptions options)
        {
            this.Grid    = new Grid(nodes, branches);   // Create Grid
            this.Options = options;                     // Set options
        }


        /// <summary>
        /// Steady state mode calculus
        /// </summary>
        public void Calculate()
        {
            //this.Grid.SolverGS(this.Grid.Uinit, this.Options);

            //this.Grid.SolverGS(this.Grid.Uinit, new CalculationOptions() { IterationsCount = 10 }).SolverNR(this.Grid.Ucalc, this.Options);
            this.Grid.SolverNR(this.Grid.Uinit, this.Options);

            //this.Grid.SolverNR2(this.Grid.Uinit, this.Options);
            //this.Grid.SolverGS(this.Grid.Uinit, new CalculationOptions() { IterationsCount = 5}).SolverNR2(this.Grid.Ucalc, this.Options);


            this.Grid.CalculatePowerMatrix();
            this.NeedsToCalc = false;
        }       
    }
}
