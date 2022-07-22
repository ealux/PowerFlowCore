using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

using PowerFlowCore.Data;
using PowerFlowCore.Solvers;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore
{
    /// <summary>
    /// Encapsulate grid-solver interface
    /// </summary>
    public static class Engine
    {
        /// <summary>
        /// Engine calculations static options. May be use in calculations
        /// </summary>
        public static CalculationOptions Options { get; set; } = new CalculationOptions();

        #region Calc Grid default

        /// <summary>
        /// Calculate the grid with default <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <returns>Tuple with <see cref="Grid"/> object and <see cref="bool"/> calculation result</returns>
        public static (Grid Grid, bool Succsess) Calculate(this Grid grid)
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
        /// Calculate the grid with default <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <param name="success">Calcutaion result</param>
        /// <returns><see cref="Grid"/> object</returns>
        public static Grid Calculate(this Grid grid, out bool success)
        {
            (grid, success) = Calculate(grid);
            return grid;
        }

        /// <summary>
        /// Calculate grids collections in parallel with default <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids)
        {
            if(grids == null)
                throw new ArgumentNullException(nameof(grids));

            List<(Grid, bool)> list = grids.Select(g => (g, false)).ToList();

            list.AsParallel().ForAll( item =>
            {
                // Reserve initial grid
                Grid gridReserve = item.Item1.DeepCopy();

                // Calculate
                item.Item1.SolverNR(item.Item1.Uinit, new CalculationOptions(), out bool suc);

                if (suc)
                {
                    item.Item1.CalculatePowerMatrix();    // Calculate power flows
                    item.Item2 = true;
                }
                else
                    item.Item2 = false;
            });           

            return list.AsEnumerable();
        }

        /// <summary>
        /// Calculate grids collections in parallel with default <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <param name="success">Calcutaion result. False if any grid calculation is failed</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids, out bool success)
        {
            var res = new List<(Grid, bool)>();
            success = true;

            res = Calculate(grids).ToList();

            if(res.Any(v => v.Item2 == false))
                success = false;

            return res.AsEnumerable();
        }

        #endregion

        #region Calc Grid optional

        /// <summary>
        /// Calculate the grid
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied</param>
        /// <returns>Tuple with Grid object and bool calculation result</returns>
        public static (Grid Grid, bool Succsess) Calculate(this Grid grid, CalculationOptions options)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (options == null)
                options = new CalculationOptions();

            // Reserve initial grid
            Grid gridReserve = grid.DeepCopy();

            // Calculate
            grid.SolverNR(grid.Uinit, options, out bool suc);

            if (suc)
            {
                grid.CalculatePowerMatrix();    // Calculate power flows
                return (grid, suc);             // On success
            }
            else
                return (gridReserve, false);    // On fault 
        }

        /// <summary>
        /// Calculate the grid
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied</param>
        /// <param name="success">Calcutaion result</param>
        /// <returns>Tuple with Grid object and bool calculation result</returns>
        public static Grid Calculate(this Grid grid, CalculationOptions options, out bool success)
        {
            (grid, success) = Calculate(grid, options);
            return grid;
        }

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids, CalculationOptions options)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));
            if(options == null)
                options = new CalculationOptions();

            List<(Grid, bool)> list = grids.Select(g => (g, false)).ToList();

            list.AsParallel().ForAll(item =>
            {
                // Reserve initial grid
                Grid gridReserve = item.Item1.DeepCopy();

                // Calculate
                item.Item1.SolverNR(item.Item1.Uinit, options, out bool suc);

                if (suc)
                {
                    item.Item1.CalculatePowerMatrix();    // Calculate power flows
                    item.Item2 = true;
                }
                else
                    item.Item2 = false;
            });

            return list.AsEnumerable();
        }

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied</param>
        /// <param name="success">Calcutaion result. False if any grid calculation is failed</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids, CalculationOptions options, out bool success)
        {
            var res = new List<(Grid, bool)>();
            success = true;

            res = Calculate(grids, options).ToList();

            if (res.Any(v => v.Item2 == false))
                success = false;

            return res.AsEnumerable();
        }

        #endregion

        #region Calc SolvableGrid

        /// <summary>
        /// Calculate grid
        /// </summary>
        /// <param name="grid">Input <see cref="SolvableGrid"/></param>
        /// <returns>Tuple with <see cref="Grid"/> object and <see cref="bool"/> calculation result</returns>
        public static (Grid Grid, bool Succsess) Calculate(this SolvableGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            // Reserve initial grid
            Grid gridReserve = grid.Grid.DeepCopy();

            // Voltage vector to initialialize calculations for each solver
            Vector<Complex> Uinitial = grid.Grid.Uinit;

            // Solver aplication result
            var success = false;

            foreach (var solver in grid.Solvers)
            {
                if (success)
                    continue;

                // Calculate
                if(solver.Item1 == SolverType.GaussSeidel)
                    grid.Grid.SolverGS(Uinitial, solver.Item2, out success);
                else if(solver.Item1 == SolverType.NewtonRaphson)
                    grid.Grid.SolverNR(Uinitial, solver.Item2, out success);
                // Set next initialization
                Uinitial = grid.Grid.Ucalc;
            }

            if (success)
            {
                grid.Grid.CalculatePowerMatrix(); // Calculate power flows
                return (grid.Grid, true);         // On success
            }
            else
                return (gridReserve, false);      // On fault 
        }

        /// <summary>
        /// Calculate grid
        /// </summary>
        /// <param name="grid">Input <see cref="SolvableGrid"/></param>
        /// <param name="success">Calcutaion result</param>
        /// <returns><see cref="Grid"/> object</returns>
        public static Grid Calculate(this SolvableGrid grid, out bool success)
        {
            (grid.Grid, success) = Calculate(grid);
            return grid.Grid;
        }

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="grids">Input <see cref="SolvableGrid"/> collection</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<SolvableGrid> grids)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));

            List<(SolvableGrid, bool)> list = grids.Select(g => (g, false)).ToList();

            list.AsParallel().ForAll(item =>
            {
                // Reserve initial grid
                Grid gridReserve = item.Item1.Grid.DeepCopy();

                // Voltage vector to initialialize calculations for each solver
                Vector<Complex> Uinitial = item.Item1.Grid.Uinit;

                // Solver aplication result
                var success = false;

                foreach (var solver in item.Item1.Solvers)
                {
                    if (success)
                        continue;

                    // Calculate
                    if (solver.Item1 == SolverType.GaussSeidel)
                        item.Item1.Grid.SolverGS(Uinitial, solver.Item2, out success);
                    else if (solver.Item1 == SolverType.NewtonRaphson)
                        item.Item1.Grid.SolverNR(Uinitial, solver.Item2, out success);
                    // Set next initialization
                    Uinitial = item.Item1.Grid.Ucalc;
                }

                if (success)
                {
                    item.Item1.Grid.CalculatePowerMatrix();    // Calculate power flows
                    item.Item2 = true;
                }
                else
                    item.Item2 = false;
            });

            return list.Select(item => (item.Item1.Grid, item.Item2)).AsEnumerable();
        }

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="grids">Input <see cref="SolvableGrid"/> collection</param>
        /// <param name="success">Calcutaion result. False if any grid calculation is failed</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<SolvableGrid> grids, out bool success)
        {
            var res = new List<(Grid, bool)>();
            success = true;

            res = Calculate(grids).ToList();

            if (res.Any(v => v.Item2 == false))
                success = false;

            return res.AsEnumerable();
        }

        #endregion

        #region Solver appliers

        /// <summary>
        /// Creates new <see cref="SolvableGrid"/> object with selected solver
        /// </summary>
        /// <param name="grid">Grid to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <returns><see cref="SolvableGrid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grid"/> is null</exception>
        public static SolvableGrid ApplySolver(this Grid grid, SolverType solver)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            return new SolvableGrid(grid, solver, new CalculationOptions());
        }

        /// <summary>
        /// Creates new <see cref="SolvableGrid"/> object with selected solver and <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grid">Grid to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied. Apply default if parameter is null</param>
        /// <returns><see cref="SolvableGrid"/> with selected solver and options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grid"/> is null</exception>
        public static SolvableGrid ApplySolver(this Grid grid, SolverType solver, CalculationOptions options)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (options == null)
                options = new CalculationOptions();

            return new SolvableGrid(grid, solver, options);
        }

        /// <summary>
        /// Creates new <see cref="IEnumerable{SolvableGrid}"/> collection with selected solver
        /// </summary>
        /// <param name="grids">Collection of <see cref="Grid"/> to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <returns>Collection of <see cref="SolvableGrid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grids"/> is null</exception>
        public static IEnumerable<SolvableGrid> ApplySolver(this IEnumerable<Grid> grids, SolverType solver)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));

            var output = new List<SolvableGrid>(grids.Count());

            foreach (var item in grids)
                output.Add(new SolvableGrid(item, solver, new CalculationOptions()));

            return output.AsEnumerable();
        }

        /// <summary>
        /// Creates new <see cref="IEnumerable{SolvableGrid}"/> collection with selected solver
        /// </summary>
        /// <param name="grids">Collection of <see cref="Grid"/> to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied. Apply default if parameter is null</param>
        /// <returns>Collection of <see cref="SolvableGrid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grids"/> is null</exception>
        public static IEnumerable<SolvableGrid> ApplySolver(this IEnumerable<Grid> grids, SolverType solver, CalculationOptions options)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));
            if (options == null)
                options = new CalculationOptions();

            var output = new List<SolvableGrid>(grids.Count());

            foreach (var item in grids)
                output.Add(new SolvableGrid(item, solver, options));

            return output.AsEnumerable();
        }


        /// <summary>
        /// Creates new <see cref="SolvableGrid"/> object with selected solver
        /// </summary>
        /// <param name="grid">Grid to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <returns><see cref="SolvableGrid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grid"/> is null</exception>
        public static SolvableGrid ApplySolver(this SolvableGrid grid, SolverType solver)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            grid.Solvers.Enqueue((solver, new CalculationOptions()));

            return grid;
        }

        /// <summary>
        /// Creates new <see cref="SolvableGrid"/> object with selected solver and <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grid">Grid to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied. Apply default if parameter is null</param>
        /// <returns><see cref="SolvableGrid"/> with selected solver and options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grid"/> is null</exception>
        public static SolvableGrid ApplySolver(this SolvableGrid grid, SolverType solver, CalculationOptions options)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (options == null)
                options = new CalculationOptions();

            grid.Solvers.Enqueue((solver, options));

            return grid;
        }

        /// <summary>
        /// Creates new <see cref="IEnumerable{SolvableGrid}"/> collection with selected solver
        /// </summary>
        /// <param name="grids">Collection of <see cref="SolvableGrid"/> to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <returns>Collection of <see cref="SolvableGrid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grids"/> is null</exception>
        public static IEnumerable<SolvableGrid> ApplySolver(this IEnumerable<SolvableGrid> grids, SolverType solver)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));

            foreach (var item in grids)
                item.Solvers.Enqueue((solver, new CalculationOptions()));

            return grids;
        }

        /// <summary>
        /// Creates new <see cref="IEnumerable{SolvableGrid}"/> collection with selected solver
        /// </summary>
        /// <param name="grids">Collection of <see cref="SolvableGrid"/> to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied. Apply default if parameter is null</param>
        /// <returns>Collection of <see cref="SolvableGrid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grids"/> is null</exception>
        public static IEnumerable<SolvableGrid> ApplySolver(this IEnumerable<SolvableGrid> grids, SolverType solver, CalculationOptions options)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));
            if (options == null)
                options = new CalculationOptions();

            foreach (var item in grids)
                item.Solvers.Enqueue((solver, options));

            return grids;
        }

        #endregion
    }
}
