using System.Collections.Generic;

using PowerFlowCore.Data;
using PowerFlowCore.Solvers;

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
            // Reserve initial grid
            Grid gridReserve = Grid.DeepCopy();

            //this.Grid.SolverGS(this.Grid.Uinit, this.Options);

            //this.Grid.SolverGS(this.Grid.Uinit, new CalculationOptions() { IterationsCount = 15 }).SolverNR(this.Grid.Ucalc, this.Options);
            this.Grid.SolverNR(this.Grid.Uinit, this.Options);


            this.Grid.CalculatePowerMatrix();
            this.NeedsToCalc = false;
        }       
    }
}
