using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate grids with single and multiple solvers
    /// </summary>
    //[SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net60)]
    //[SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser(true)]
    public class CalculateGrid_MultipleSolvers
    {
        [Params(1)]
        public int N;

        private readonly Grid _Nodes300_27PV;
        private readonly Grid _IEEE_118;
        private readonly Grid _IEEE_14;
        private readonly Grid _Nodes1350_250PV;
        private readonly Grid _Nodes2628_50PV;

        public CalculateGrid_MultipleSolvers()
        {
            _Nodes300_27PV  = SampleGrids.Nodes300_27PV();
            _IEEE_118       = SampleGrids.IEEE_118();
            _IEEE_14        = SampleGrids.IEEE_14();
            _Nodes1350_250PV = SampleGrids.Nodes1350_250PV();
            _Nodes2628_50PV = SampleGrids.Nodes2628_50PV();
        }

        
        [Benchmark(Baseline=true)]
        public void NR_IEEE_14()
        {
            for (int i = 0; i < N; i++)
                _IEEE_14.Calculate();
        }

        [Benchmark]
        public void GSandNR_IEEE_14()
        {
            for (int i = 0; i < N; i++)
                _IEEE_14.ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                         .ApplySolver(SolverType.NewtonRaphson)
                         .Calculate();
        }

        [Benchmark]
        public void NR_IEEE_118()
        {
            for (int i = 0; i < N; i++)
                _IEEE_118.Calculate();
        }

        [Benchmark]
        public void GSandNR_IEEE_118()
        {
            for (int i = 0; i < N; i++)
                _IEEE_118.ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                         .ApplySolver(SolverType.NewtonRaphson)
                         .Calculate();
        }

        [Benchmark]
        public void NR_Nodes300_27PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes300_27PV.Calculate();
        }

        [Benchmark]
        public void GSandNR_Nodes300_27PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes300_27PV.ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                              .ApplySolver(SolverType.NewtonRaphson)
                              .Calculate();
        }

        [Benchmark]
        public void NR_Nodes1350_250PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes1350_250PV.Calculate();
        }

        [Benchmark]
        public void GSandNR_Nodes1350_250PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes1350_250PV.ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                              .ApplySolver(SolverType.NewtonRaphson)
                              .Calculate();
        }

        [Benchmark]
        public void NR_Nodes2628_50PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes2628_50PV.Calculate();
        }

        [Benchmark]
        public void GSandNR_Nodes2628_50PV()
        {
            for (int i = 0; i < N; i++)
                _Nodes2628_50PV.ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 5 })
                               .ApplySolver(SolverType.NewtonRaphson)
                               .Calculate();
        }
    }
}
