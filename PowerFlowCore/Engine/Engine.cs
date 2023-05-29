using PowerFlowCore.Algebra;
using PowerFlowCore.Data;
using PowerFlowCore.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore
{
    /// <summary>
    /// Encapsulate grid-solver interface
    /// </summary>
    public static class Engine
    {
        /// <summary>
        /// Engine calculations static options. May be used in calculations
        /// </summary>
        public static CalculationOptions Options { get; set; } = new CalculationOptions();

        #region Calc Grid default

        /// <summary>
        /// Calculate the grid with engine static <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <returns>Tuple with <see cref="Grid"/> object and <see cref="bool"/> calculation result</returns>
        public static (Grid Grid, bool Succsess) Calculate(this Grid grid)
        {
            _ = grid ?? throw new ArgumentNullException(nameof(grid));

            // Validate grid
            if (!grid.Validate())
                return (grid, false);

            // Calc grid
            Grid calc = grid.DeepCopy().WithId(grid.Id);

            // Set breaker impedance for calculus. Checks
            if (!calc.SetBreakers())
                return (grid, false);
            calc.InitParameters(calc.Nodes, calc.Branches);

            bool success = false;

            // Calculate
            if (grid.Solvers.Count == 0)
            {
                calc.SolverNR(calc.Uinit, Options, out success);
            }
            else
            {
                // Voltage vector to initialialize calculations for each solver
                Complex[] Uinitial = calc.Uinit.Copy();

                foreach (var solver in grid.Solvers)
                {
                    if (success)
                        continue;

                    // Turn off constraints on all solvers except the last
                    var constrTemp = solver.Item2.UseVoltageConstraint;
                    solver.Item2.UseVoltageConstraint = false;
                    if (solver == grid.Solvers.Last())
                        solver.Item2.UseVoltageConstraint = constrTemp;

                    // Calculate
                    if (solver.Item1 == SolverType.GaussSeidel)
                        calc.SolverGS(Uinitial, solver.Item2, out success);
                    else if (solver.Item1 == SolverType.NewtonRaphson)
                        calc.SolverNR(Uinitial, solver.Item2, out success);

                    // Set next initialization
                    Uinitial = calc.Ucalc;

                    // Set constraints back
                    solver.Item2.UseVoltageConstraint = constrTemp;
                }

                grid.Solvers.Clear();
            }            

            if (success)
            {
                grid.Nodes = calc.DeepCopyNodes();
                grid.CalculatePowerMatrix();    // Calculate power flows
                return (grid, success);             // On success
            }
            else
                return (grid, false);    // On fault
        }

        /// <summary>
        /// Calculate the grid with engine static <see cref="CalculationOptions"/>
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
        /// Calculate grids collections in parallel with engine static <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids)
        {
            _ = grids ?? throw new ArgumentNullException(nameof(grids));

            IEnumerable<(Grid grid, bool success)> list = grids.Select(g => (g, false));

            list.AsParallel().ForAll( item =>
            {
                // Validate grid
                if (!item.Item1.Validate())
                {
                    item.Item2 = false;
                    return;
                }                 

                // Calc grid
                Grid calc = item.Item1.DeepCopy().WithId(item.Item1.Id);

                // Set breaker impedance for calculus
                if (!calc.SetBreakers())
                {
                    item.Item2 = false;
                    return;
                }              
                calc.InitParameters(calc.Nodes, calc.Branches);

                bool success = false;

                // Calculate
                if (item.grid.Solvers.Count == 0)
                {
                    calc.SolverNR(calc.Uinit, Options, out success);
                }
                else
                {
                    // Voltage vector to initialialize calculations for each solver
                    Complex[] Uinitial = calc.Uinit.Copy();

                    foreach (var solver in item.grid.Solvers)
                    {
                        if (success)
                            continue;

                        // Turn off constraints on all solvers except the last
                        var constrTemp = solver.Item2.UseVoltageConstraint;
                        solver.Item2.UseVoltageConstraint = false;
                        if (solver == item.grid.Solvers.Last())
                            solver.Item2.UseVoltageConstraint = constrTemp;

                        // Calculate
                        if (solver.Item1 == SolverType.GaussSeidel)
                            calc.SolverGS(Uinitial, solver.Item2, out success);
                        else if (solver.Item1 == SolverType.NewtonRaphson)
                            calc.SolverNR(Uinitial, solver.Item2, out success);

                        // Set next initialization
                        Uinitial = calc.Ucalc;

                        // Set constraints back
                        solver.Item2.UseVoltageConstraint = constrTemp;
                    }

                    item.grid.Solvers.Clear();
                }

                if (success)
                {
                    item.Item1.Nodes = calc.DeepCopyNodes();
                    item.Item1.CalculatePowerMatrix();    // Calculate power flows
                    item.Item2 = true;
                }
                else
                    item.Item2 = false;
            });           

            return list; 
        }

        /// <summary>
        /// Calculate grids collections in parallel with engine static <see cref="CalculationOptions"/>
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <param name="success">Calcutaion result. False if any grid calculation is failed</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids, out bool success)
        {
            IEnumerable<(Grid, bool)> res;
            success = true;

            res = Calculate(grids);

            if(res.Any(v => v.Item2 == false))
                success = false;

            return res;
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
            _ = grid ?? throw new ArgumentNullException(nameof(grid));
            if (options == null)
                options = Options ?? new CalculationOptions();

            // Validate grid
            if (!grid.Validate())
                return (grid, false);

            // Calc grid
            Grid calc = grid.DeepCopy().WithId(grid.Id);

            // Set breaker impedance for calculus
            if (options.UseBreakerImpedance)
            {
                if (!calc.SetBreakers())
                    return (grid, false);
                calc.InitParameters(calc.Nodes, calc.Branches);
            }

            bool success = false;

            // Calculate
            if (grid.Solvers.Count == 0)
            {
                calc.SolverNR(calc.Uinit, options, out success);
            }
            else
            {
                // Voltage vector to initialialize calculations for each solver
                Complex[] Uinitial = calc.Uinit.Copy();

                foreach (var solver in grid.Solvers)
                {
                    if (success)
                        continue;

                    // Turn off constraints on all solvers except the last
                    var constrTemp = solver.Item2.UseVoltageConstraint;
                    solver.Item2.UseVoltageConstraint = false;
                    if (solver == grid.Solvers.Last())
                        solver.Item2.UseVoltageConstraint = constrTemp;

                    // Calculate
                    if (solver.Item1 == SolverType.GaussSeidel)
                        calc.SolverGS(Uinitial, solver.Item2, out success);
                    else if (solver.Item1 == SolverType.NewtonRaphson)
                        calc.SolverNR(Uinitial, solver.Item2, out success);

                    // Set next initialization
                    Uinitial = calc.Ucalc;

                    // Set constraints back
                    solver.Item2.UseVoltageConstraint = constrTemp;
                }

                grid.Solvers.Clear();
            }

            if (success)
            {
                grid.Nodes = calc.DeepCopyNodes();
                grid.CalculatePowerMatrix();    // Calculate power flows
                return (grid, success);             // On success
            }
            else
                return (grid, false);    // On fault 

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
            _ = grids ?? throw new ArgumentNullException(nameof(grids));
            if(options == null)
                options = Options ?? new CalculationOptions();

            IEnumerable<(Grid grid, bool success)> list = grids.Select(g => (g, false));

            list.AsParallel().ForAll(item =>
            {
                // Validate grid
                if (!item.Item1.Validate())
                {
                    item.Item2 = false;
                    return;
                }

                // Calc grid
                Grid calc = item.Item1.DeepCopy().WithId(item.Item1.Id);

                // Set breaker impedance for calculus
                if (options.UseBreakerImpedance)
                {
                    if (!calc.SetBreakers())
                    {
                        item.Item2 = false;
                        return;
                    }
                    calc.InitParameters(calc.Nodes, calc.Branches);
                }

                bool success = false;

                // Calculate
                if (item.grid.Solvers.Count == 0)
                {
                    calc.SolverNR(calc.Uinit, options, out success);
                }
                else
                {
                    // Voltage vector to initialialize calculations for each solver
                    Complex[] Uinitial = calc.Uinit.Copy();

                    foreach (var solver in item.grid.Solvers)
                    {
                        if (success)
                            continue;

                        // Turn off constraints on all solvers except the last
                        var constrTemp = solver.Item2.UseVoltageConstraint;
                        solver.Item2.UseVoltageConstraint = false;
                        if (solver == item.grid.Solvers.Last())
                            solver.Item2.UseVoltageConstraint = constrTemp;

                        // Calculate
                        if (solver.Item1 == SolverType.GaussSeidel)
                            calc.SolverGS(Uinitial, solver.Item2, out success);
                        else if (solver.Item1 == SolverType.NewtonRaphson)
                            calc.SolverNR(Uinitial, solver.Item2, out success);

                        // Set next initialization
                        Uinitial = calc.Ucalc;

                        // Set constraints back
                        solver.Item2.UseVoltageConstraint = constrTemp;
                    }

                    item.grid.Solvers.Clear();
                }

                if (success)
                {
                    item.Item1.Nodes = calc.DeepCopyNodes();
                    item.Item1.CalculatePowerMatrix();    // Calculate power flows
                    item.Item2 = true;
                }
                else
                    item.Item2 = false;
            });

            return list;
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
            IEnumerable<(Grid, bool)> res;
            success = true;

            res = Calculate(grids, options);

            if (res.Any(v => v.Item2 == false))
                success = false;

            return res;
        }

        #endregion

        #region Solver appliers

        /// <summary>
        /// Add selected solver to grid solvers queue
        /// </summary>
        /// <param name="grid">Grid to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <returns><see cref="Grid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grid"/> is null</exception>
        public static Grid ApplySolver(this Grid grid, SolverType solver)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            grid.Solvers.Enqueue((solver, Options ?? new CalculationOptions()));

            return grid;
        }

        /// <summary>
        /// Add selected solver and <see cref="CalculationOptions"/> to grid solvers queue
        /// </summary>
        /// <param name="grid">Grid to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied. Apply default if parameter is null</param>
        /// <returns><see cref="Grid"/> with selected solver and options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grid"/> is null</exception>
        public static Grid ApplySolver(this Grid grid, SolverType solver, CalculationOptions options)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (options == null)
                options = Options ?? new CalculationOptions();

            grid.Solvers.Enqueue((solver, options));

            return grid;
        }

        /// <summary>
        /// Creates new <see cref="IEnumerable{Grid}"/> collection with added selected solver to grid solvers queue
        /// </summary>
        /// <param name="grids">Collection of <see cref="Grid"/> to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <returns>Collection of <see cref="Grid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grids"/> is null</exception>
        public static IEnumerable<Grid> ApplySolver(this IEnumerable<Grid> grids, SolverType solver)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));

            var output = new List<Grid>(grids.Count());

            foreach (var item in grids)
            {
                item.Solvers.Enqueue((solver, Options ?? new CalculationOptions()));
                output.Add(item);
            }

            return output.AsEnumerable();
        }

        /// <summary>
        /// Creates new <see cref="IEnumerable{SolvableGrid}"/> collection with added selected solver to grid solvers queue
        /// </summary>
        /// <param name="grids">Collection of <see cref="Grid"/> to apply solver</param>
        /// <param name="solver">Solver to be applied</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied. Apply default if parameter is null</param>
        /// <returns>Collection of <see cref="Grid"/> with selected solver and default options</returns>
        /// <exception cref="ArgumentNullException">Return exception if <paramref name="grids"/> is null</exception>
        public static IEnumerable<Grid> ApplySolver(this IEnumerable<Grid> grids, SolverType solver, CalculationOptions options)
        {
            if (grids == null)
                throw new ArgumentNullException(nameof(grids));
            if (options == null)
                options = Options ?? new CalculationOptions();

            var output = new List<Grid>(grids.Count());

            foreach (var item in grids)
            {
                item.Solvers.Enqueue((solver, options));
                output.Add(item);
            }

            return output.AsEnumerable();
        }

        #endregion
    }
}
