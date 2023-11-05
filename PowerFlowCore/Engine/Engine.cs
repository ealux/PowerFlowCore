using PowerFlowCore.Algebra;
using PowerFlowCore.Data;
using PowerFlowCore.Solvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        #region [Individual]


        /// <summary>
        /// Calculate the grid
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <returns>Tuple with Grid object and bool calculation result</returns>
        public static (Grid Grid, bool Succsess) Calculate(this Grid grid, Complex[] uinit = null!, CalculationOptions options = null!)
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

            // Voltage vector to initialialize calculations for each solver
            Complex[] Uinitial = uinit != null ?
                                    uinit.Length == calc.Nodes.Count ?
                                        uinit :
                                        calc.Uinit.Copy()
                                    : calc.Uinit.Copy();


            // Calculate
            if (grid.Solvers.Count == 0)
            {
                try
                {
                    calc.SolverNR(Uinitial, options, out success);
                }
                catch (Exception)
                {
                    Logger.LogCritical("Internal N-R solver error! Check inputs!");
                    success = false;
                }
            }
            else
            {
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
                    {
                        try
                        {
                            calc.SolverGS(Uinitial, solver.Item2, out success);
                        }
                        catch (Exception)
                        {
                            Logger.LogCritical("Internal G-S solver error! Check inputs!");
                            success = false;
                        }
                    }

                    else if (solver.Item1 == SolverType.NewtonRaphson)
                    {
                        try
                        {
                            calc.SolverNR(Uinitial, solver.Item2, out success);
                        }
                        catch (Exception)
                        {
                            Logger.LogCritical("Internal N-R solver error! Check inputs!");
                            success = false;
                        }
                    }


                    // Set next initialization
                    Uinitial = calc.Ucalc;

                    // Set constraints back
                    solver.Item2.UseVoltageConstraint = constrTemp;
                }

                grid.Solvers.Clear();
            }

            if (success)
            {
                grid = calc;
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
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <param name="success">Calcutaion result</param>
        /// <returns>Tuple with Grid object and bool calculation result</returns>
        public static Grid Calculate(this Grid grid, out bool success, Complex[] uinit = null!, CalculationOptions options = null!)
        {
            (grid, success) = Calculate(grid, uinit, options);
            return grid;
        }

        #endregion


        #region [Parallel]

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids, CalculationOptions options = null!)
        {
            _ = grids ?? throw new ArgumentNullException(nameof(grids));
            if (options == null)
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
                    try
                    {
                        calc.SolverNR(calc.Uinit, options, out success);
                    }
                    catch (Exception)
                    {
                        Logger.LogCritical("Internal N-R solver error! Check inputs!");
                        success = false;
                    }
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
                        {
                            try
                            {
                                calc.SolverGS(Uinitial, solver.Item2, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal G-S solver error! Check inputs!");
                                success = false;
                            }
                        }
                        else if (solver.Item1 == SolverType.NewtonRaphson)
                        {
                            try
                            {
                                calc.SolverNR(Uinitial, solver.Item2, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal N-R solver error! Check inputs!");
                                success = false;
                            }
                        }

                        // Set next initialization
                        Uinitial = calc.Ucalc;

                        // Set constraints back
                        solver.Item2.UseVoltageConstraint = constrTemp;
                    }

                    item.grid.Solvers.Clear();
                }

                if (success)
                {
                    item.Item1 = calc;
                    item.Item1.CalculatePowerMatrix();    // Calculate power flows
                    item.Item2 = true;
                }
                else
                    item.Item2 = false;
            });

            return list;
        }

        /// <summary>
        /// Calculate grids collections in parallel (async)
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static async Task<IEnumerable<(Grid Grid, bool Succsess)>> CalculateAsync(this IEnumerable<Grid> grids, CalculationOptions options = null!)
        {
            _ = grids ?? throw new ArgumentNullException(nameof(grids));
            if (options == null)
                options = Options ?? new CalculationOptions();

            var list = grids.Select(g => new Func<Task<(Grid grid, bool success)>>(() => Job((g,false)))).ToList();
            List<(Grid grid, bool success)> listout = new List<(Grid grid, bool success)>(list.Count);

            // Internal job on calculations
            async Task<(Grid grid, bool success)> Job((Grid grid, bool success) item)
            {
                // Validate grid
                if (!item.Item1.Validate())
                {
                    item.Item2 = false;
                    return await Task.FromResult(item);
                }

                // Calc grid
                Grid calc = item.Item1.DeepCopy().WithId(item.Item1.Id);

                // Set breaker impedance for calculus
                if (options.UseBreakerImpedance)
                {
                    if (!calc.SetBreakers())
                    {
                        item.Item2 = false;
                        return await Task.FromResult(item);
                    }
                    calc.InitParameters(calc.Nodes, calc.Branches);
                }

                bool success = false;

                // Calculate
                if (item.grid.Solvers.Count == 0)
                {
                    try
                    {
                        calc.SolverNR(calc.Uinit, options, out success);
                    }
                    catch (Exception)
                    {
                        Logger.LogCritical("Internal N-R solver error! Check inputs!");
                        success = false;
                    }
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
                        {
                            try
                            {
                                calc.SolverGS(Uinitial, solver.Item2, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal G-S solver error! Check inputs!");
                                success = false;
                            }
                        }
                        else if (solver.Item1 == SolverType.NewtonRaphson)
                        {
                            try
                            {
                                calc.SolverNR(Uinitial, solver.Item2, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal N-R solver error! Check inputs!");
                                success = false;
                            }
                        }

                        // Set next initialization
                        Uinitial = calc.Ucalc;

                        // Set constraints back
                        solver.Item2.UseVoltageConstraint = constrTemp;
                    }

                    item.grid.Solvers.Clear();
                }

                if (success)
                {
                    item.Item1 = calc;
                    item.Item1.CalculatePowerMatrix();    // Calculate power flows
                    item.Item2 = true;
                }
                else
                    item.Item2 = false;

                return await Task.FromResult(item);
            }

            // Main async iterator
            await ParallelProcessingAsync(list, async job => listout.Add(await job())).ConfigureAwait(false);

            return listout;
        }

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="grids">Input <see cref="Grid"/> collection</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <param name="success">Calcutaion result. False if any grid calculation is failed</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<Grid> grids, out bool success, CalculationOptions options = null!)
        {
            IEnumerable<(Grid, bool)> res;
            success = true;

            res = Calculate(grids, options);

            if (res.Any(v => v.Item2 == false))
                success = false;

            return res;
        }

        #endregion


        #region [Parallel with Uinitial]

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="gridsWithUinit">Input tuple of <see cref="Grid"/> and <see cref="Complex[]"/> voltage inital vector</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<(Grid grid, Complex[] uinit)> gridsWithUinit, CalculationOptions options = null!)
        {
            _ = gridsWithUinit ?? throw new ArgumentNullException(nameof(gridsWithUinit));
            if (options == null)
                options = Options ?? new CalculationOptions();

            IEnumerable<(Grid grid, bool success, Complex[] uinit)> list = gridsWithUinit.Select(g => (g.grid, false, g.uinit));
            IEnumerable<(Grid grid, bool success)> calcs;

            list.AsParallel().ForAll(item =>
            {
                // Validate grid
                if (!item.grid.Validate())
                {
                    item.success = false;
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
                // Voltage vector to initialialize calculations for each solver
                Complex[] Uinitial = item.uinit != null ?
                                        item.uinit.Length == calc.Nodes.Count ?
                                            item.uinit :
                                            calc.Uinit.Copy()
                                        : calc.Uinit.Copy();

                // Calculate
                if (item.grid.Solvers.Count == 0)
                {
                    try
                    {
                        calc.SolverNR(Uinitial, options, out success);
                    }
                    catch (Exception)
                    {
                        Logger.LogCritical("Internal N-R solver error! Check inputs!");
                        success = false;
                    }
                }
                else
                {
                    foreach (var solver in item.grid.Solvers)
                    {
                        if (success)
                            continue;

                        // Turn off constraints on all solvers except the last
                        var constrTemp = solver.Options.UseVoltageConstraint;
                        solver.Options.UseVoltageConstraint = false;
                        if (solver == item.grid.Solvers.Last())
                            solver.Options.UseVoltageConstraint = constrTemp;

                        // Calculate
                        if (solver.Name == SolverType.GaussSeidel)
                        {
                            try
                            {
                                calc.SolverGS(Uinitial, solver.Options, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal G-S solver error! Check inputs!");
                                success = false;
                            }
                        }
                        else if (solver.Name == SolverType.NewtonRaphson)
                        {
                            try
                            {
                                calc.SolverNR(Uinitial, solver.Options, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal N-R solver error! Check inputs!");
                                success = false;
                            }
                        }

                        // Set next initialization
                        Uinitial = calc.Ucalc;

                        // Set constraints back
                        solver.Options.UseVoltageConstraint = constrTemp;
                    }

                    item.grid.Solvers.Clear();
                }

                if (success)
                {
                    item.grid = calc;
                    item.grid.CalculatePowerMatrix();    // Calculate power flows
                    item.success = true;
                }
                else
                    item.success = false;
            });

            calcs = list.Select(g => (g.grid, g.success));

            return calcs;
        }

        /// <summary>
        /// Calculate grids collections in parallel async
        /// </summary>
        /// <param name="gridsWithUinit">Input tuple of <see cref="Grid"/> and <see cref="Complex[]"/> voltage inital vector</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static async Task<IEnumerable<(Grid Grid, bool Succsess)>> CalculateAsync(this IEnumerable<(Grid grid, Complex[] uinit)> gridsWithUinit, CalculationOptions options = null!)
        {
            _ = gridsWithUinit ?? throw new ArgumentNullException(nameof(gridsWithUinit));
            if (options == null)
                options = Options ?? new CalculationOptions();

            var list = gridsWithUinit.Select(g => new Func<Task<(Grid grid, bool success)>>(() => Job((g.grid, false, g.uinit)))).ToList();
            List<(Grid grid, bool success)> listout = new List<(Grid grid, bool success)>(list.Count);

            // Internal job on calculations
            async Task<(Grid grid, bool success)> Job((Grid grid, bool success, Complex[] uinit) item)
            {
                // Validate grid
                if (!item.grid.Validate())
                {
                    item.success = false;
                    return await Task.FromResult((item.grid, item.success));
                }

                // Calc grid
                Grid calc = item.Item1.DeepCopy().WithId(item.Item1.Id);

                // Set breaker impedance for calculus
                if (options.UseBreakerImpedance)
                {
                    if (!calc.SetBreakers())
                    {
                        item.Item2 = false;
                        return await Task.FromResult((item.grid, item.success));
                    }
                    calc.InitParameters(calc.Nodes, calc.Branches);
                }

                bool success = false;
                // Voltage vector to initialialize calculations for each solver
                Complex[] Uinitial = item.uinit != null ?
                                        item.uinit.Length == calc.Nodes.Count ?
                                            item.uinit :
                                            calc.Uinit.Copy()
                                        : calc.Uinit.Copy();

                // Calculate
                if (item.grid.Solvers.Count == 0)
                {
                    try
                    {
                        calc.SolverNR(Uinitial, options, out success);
                    }
                    catch (Exception)
                    {
                        Logger.LogCritical("Internal N-R solver error! Check inputs!");
                        success = false;
                    }
                }
                else
                {
                    foreach (var solver in item.grid.Solvers)
                    {
                        if (success)
                            continue;

                        // Turn off constraints on all solvers except the last
                        var constrTemp = solver.Options.UseVoltageConstraint;
                        solver.Options.UseVoltageConstraint = false;
                        if (solver == item.grid.Solvers.Last())
                            solver.Options.UseVoltageConstraint = constrTemp;

                        // Calculate
                        if (solver.Name == SolverType.GaussSeidel)
                        {
                            try
                            {
                                calc.SolverGS(Uinitial, solver.Options, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal G-S solver error! Check inputs!");
                                success = false;
                            }
                        }
                        else if (solver.Name == SolverType.NewtonRaphson)
                        {
                            try
                            {
                                calc.SolverNR(Uinitial, solver.Options, out success);
                            }
                            catch (Exception)
                            {
                                Logger.LogCritical("Internal N-R solver error! Check inputs!");
                                success = false;
                            }
                        }

                        // Set next initialization
                        Uinitial = calc.Ucalc;

                        // Set constraints back
                        solver.Options.UseVoltageConstraint = constrTemp;
                    }

                    item.grid.Solvers.Clear();
                }

                if (success)
                {
                    item.grid = calc;
                    item.grid.CalculatePowerMatrix();    // Calculate power flows
                    item.success = true;
                }
                else
                    item.success = false;

                return await Task.FromResult((item.grid, item.success));
            }

            // Main async iterator
            await ParallelProcessingAsync(list, async job => listout.Add(await job())).ConfigureAwait(false);

            return listout;
        }

        /// <summary>
        /// Calculate grids collections in parallel
        /// </summary>
        /// <param name="gridsWithUinit">Input tuple of <see cref="Grid"/> and <see cref="Complex[]"/> voltage inital vector</param>
        /// <param name="options"><see cref="CalculationOptions"/> to be applied (optional)</param>
        /// <param name="success">Calcutaion result. False if any grid calculation is failed</param>
        /// <returns>Collection of Grid object and bool calculation result pairs</returns>
        public static IEnumerable<(Grid Grid, bool Succsess)> Calculate(this IEnumerable<(Grid grid, Complex[] uinit)> gridsWithUinit, out bool success, CalculationOptions options = null!)
        {
            IEnumerable<(Grid, bool)> res;
            success = true;

            res = Calculate(gridsWithUinit, options);

            if (res.Any(v => v.Item2 == false))
                success = false;

            return res;
        }

        #endregion


        #region [Private methods]

        /// <summary>
        /// Create partitioner and awaits tasks with setted jobs
        /// </summary>
        /// <param name="source">Enumerable set of grids to calculate</param>
        /// <param name="body">Func to apply</param>
        /// <returns></returns>
        private static Task ParallelProcessingAsync<T>(IEnumerable<T> source, Func<T, Task> body)
        {
            async Task AwaitPartitioner(IEnumerator<T> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    {
                        await body(partition.Current);
                    }
                }
            }

            return Task.WhenAll(
                          Partitioner
                              .Create(source)
                              .GetPartitions(Environment.ProcessorCount)
                              .AsParallel()
                              .Select(AwaitPartitioner));
        }

        #endregion


        #region [Solver appliers]

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

        #endregion [Solver appliers]
    }
}
