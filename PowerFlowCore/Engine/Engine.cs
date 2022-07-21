using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

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


        
        public Engine(IConverter converter, CalculationOptions? options = default)
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
            //this.Grid.SolverGS(this.Grid.Uinit, this.Options, out success);

            //this.Grid.SolverGS(this.Grid.Uinit, new CalculationOptions() { IterationsCount = 5 }, out success).SolverNR(this.Grid.Ucalc, this.Options, out success);
            this.Grid.SolverNR(this.Grid.Uinit, this.Options, out bool success);


            this.Grid.CalculatePowerMatrix();
            this.NeedsToCalc = false;
        }
        

        /// <summary>
        /// Calculate the grid with default <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <returns>Tuple with Grid object and bool calculation result</returns>
        public static (Grid result, bool success) CalculateDefault(Grid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            // Reserve initial grid
            Grid gridReserve = grid.DeepCopy();

            // Calculate
            grid.SolverNR(grid.Uinit, new CalculationOptions(), out bool suc);

            if (suc)
            {
                grid.CalculatePowerMatrix();    // Calculate power flows
                return (grid, suc);             // On success
            }                
            else
                return (gridReserve, false);    // On fault 
        }


        /// <summary>
        /// Calculate grids collections in parallel with default <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <returns>Dictionary with Grid objects and bool calculation results</returns>
        public static Dictionary<Grid, bool> CalculateDefaultParallel(IEnumerable<Grid> grids)
        {
            if(grids == null)
                throw new ArgumentNullException(nameof(grids));

            var jobs = new List<Task>();
            var dict = new ConcurrentDictionary<Grid, bool>();

            // Set jobs
            foreach (var grid in grids)
            {
                jobs.Add(Task.Run(() =>
                {
                    // Reserve initial grid
                    Grid gridReserve = grid.DeepCopy();

                    // Calculate
                    grid.SolverNR(grid.Uinit, new CalculationOptions(), out bool suc);

                    if (suc)
                    {
                        grid.CalculatePowerMatrix();    // Calculate power flows
                        dict.TryAdd(grid, suc);
                    }
                    else
                        dict.TryAdd(gridReserve, false);
                }));
            }

            // Grouping tasks
            Task t = Task.WhenAll(jobs);

            // Run all
            try { t.Wait(); }
            catch { }

            // Save results
            var output = new Dictionary<Grid, bool>(dict);

            if (t.Status == TaskStatus.Faulted)
                Logger.LogCritical("Something went wrong! One or several calculations can be failed. " +
                                    $"Try to check input grids. Inner exception message: \n {t.Exception.Message}");

            return output;
        }
    }
}
