using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using BenchmarkDotNet.Attributes;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate grids with single and multiple solvers
    /// </summary>
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.Default]
    public class CalculateGrid_MultipleSolvers
    {
        [Params(1, 10)]
        public int N;

        private readonly Grid _Nodes300_27PV;
        private readonly Grid _IEEE_118;
        private readonly Grid _IEEE_14;

        public CalculateGrid_MultipleSolvers()
        {
            _Nodes300_27PV  = SampleGrids.Nodes300_27PV();
            _IEEE_118       = SampleGrids.IEEE_118();
            _IEEE_14        = SampleGrids.IEEE_14();
        }

        
        [Benchmark(Baseline=true)]
        public void NewtonRaphsonOnly_IEEE_14()
        {
            for (int i = 0; i < N; i++)
                _IEEE_14.Calculate();
        }

        [Benchmark]
        public void GaussThenNewtonRaphson_IEEE_14()
        {
            for (int i = 0; i < N; i++)
                _IEEE_14.ApplySolver(Solvers.SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                         .ApplySolver(Solvers.SolverType.NewtonRaphson)
                         .Calculate();
        }

        [Benchmark]
        public void NewtonRaphsonOnly_IEEE_118()
        {
            for (int i = 0; i < N; i++)
                _IEEE_118.Calculate();
        }

        [Benchmark]
        public void GaussThenNewtonRaphson_IEEE_118()
        {
            for (int i = 0; i < N; i++)
                _IEEE_118.ApplySolver(Solvers.SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                         .ApplySolver(Solvers.SolverType.NewtonRaphson)
                         .Calculate();
        }

        [Benchmark]
        public void NewtonRaphsonOnly_Nodes300_27PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes300_27PV.Calculate();
        }

        [Benchmark]
        public void GaussThenNewtonRaphson_Nodes300_27PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes300_27PV.ApplySolver(Solvers.SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                              .ApplySolver(Solvers.SolverType.NewtonRaphson)
                              .Calculate();
        }
    }
}
