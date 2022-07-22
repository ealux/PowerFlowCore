using PowerFlowCore.Data;
using System;
using System.Collections.Generic;

namespace PowerFlowCore.Solvers
{
    /// <summary>
    /// Grid with chain collection of <see cref="SolverType"/> and <see cref="CalculationOptions"/>
    /// </summary>
    public class SolvableGrid
    {
        /// <summary>
        /// <see cref="Data.Grid"/> to be resolved
        /// </summary>
        public Grid Grid { get; set; }

        /// <summary>
        /// Collection of  <see cref="SolverType"/> and corresponded <see cref="CalculationOptions"/>
        /// </summary>
        public Queue<(SolverType, CalculationOptions)> Solvers { get; set; } = new Queue<(SolverType, CalculationOptions)> ();

        // Private default ctor
        private SolvableGrid() { }

        /// <summary>
        /// Create SolvableGrid object. Encapsulate transferred <see cref="Data.Grid"/>, 
        /// first <see cref="SolverType"/> in chain collection and <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grid"><see cref="Data.Grid"/> to work with</param>
        /// <param name="type">First solver in chain</param>
        /// <param name="options">Options for the first solver. Create default options if parameter is null</param>
        public SolvableGrid(Grid grid, SolverType type, CalculationOptions options)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (options == null)
                options = new CalculationOptions();
            
            Grid = grid;
            Solvers.Enqueue((type, options));
        }
    }
}
