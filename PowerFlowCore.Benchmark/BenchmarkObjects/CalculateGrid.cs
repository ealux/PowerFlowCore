using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PowerFlowCore.Solvers;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate sample grids several times for performance tests
    /// </summary>
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser(false)]
    public class CalculateGrid
    {
        [Params(1)]//, 10, 100)]
        public int N;

        private readonly Grid _Test_Ktr;
        private readonly Grid _Nodes4_1PV;
        private readonly Grid _Nodes4_1PV_ZIP;
        private readonly Grid _Nodes5_2Slack;
        private readonly Grid _IEEE_14;
        private readonly Grid _Nodes15_3PV;
        private readonly Grid _IEEE_57;
        private readonly Grid _IEEE_118;
        private readonly Grid _Nodes197_36PV;
        private readonly Grid _Nodes300_27PV;
        private readonly Grid _Nodes398_35PV;
        private readonly Grid _Nodes398_35PV_ZIP;
        private readonly Grid _Nodes874_143PV;
        private readonly Grid _Nodes1350_250PV;
        private readonly Grid _Nodes2628_50PV;

        public CalculateGrid()
        {
            _Test_Ktr          = SampleGrids.Test_Ktr();
            _Nodes4_1PV        = SampleGrids.Nodes4_1PV();
            _Nodes4_1PV_ZIP    = SampleGrids.Nodes4_1PV_ZIP();
            _Nodes5_2Slack     = SampleGrids.Nodes5_2Slack();
            _IEEE_14           = SampleGrids.IEEE_14();
            _Nodes15_3PV       = SampleGrids.Nodes15_3PV();
            _IEEE_57           = SampleGrids.IEEE_57();
            _IEEE_118          = SampleGrids.IEEE_118();
            _Nodes197_36PV     = SampleGrids.Nodes197_36PV();
            _Nodes300_27PV     = SampleGrids.Nodes300_27PV();
            _Nodes398_35PV     = SampleGrids.Nodes398_35PV();
            _Nodes398_35PV_ZIP = SampleGrids.Nodes398_35PV_ZIP();
            _Nodes874_143PV    = SampleGrids.Nodes874_143PV();
            _Nodes1350_250PV = SampleGrids.Nodes1350_250PV();
            _Nodes2628_50PV = SampleGrids.Nodes2628_50PV();
        }

        [Benchmark]
        public void Test_Ktr()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Test_Ktr);
        }

        [Benchmark]
        public void Nodes4_1PV()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes4_1PV);
        }

        [Benchmark]
        public void Nodes4_1PV_ZIP()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes4_1PV_ZIP);
        }

        [Benchmark]
        public void Nodes5_2Slack()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes5_2Slack);
        }

        [Benchmark]
        public void IEEE_14()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_IEEE_14);
        }

        [Benchmark]
        public void Nodes15_3PV()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes15_3PV);
        }

        [Benchmark]
        public void IEEE_57()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_IEEE_57);
        }

        [Benchmark]
        public void IEEE_118()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_IEEE_118);
        }

        [Benchmark]
        public void Nodes197_36PV()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes197_36PV);
        }

        [Benchmark]
        public void Nodes300_27PV()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes300_27PV);
        }

        [Benchmark]
        public void Nodes398_35PV()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes398_35PV);
        }

        [Benchmark]
        public void Nodes398_35PV_ZIP()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes398_35PV_ZIP);
        }

        [Benchmark]
        public void Nodes874_143PV()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes874_143PV);
        }

        [Benchmark]
        public void Nodes1350_250PV()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes1350_250PV);
        }

        [Benchmark]
        public void Nodes2628_50PV__GS_and_NR()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(_Nodes2628_50PV.ApplySolver(SolverType.GaussSeidel, new CalculationOptions { IterationsCount = 4 })
                                                .ApplySolver(SolverType.NewtonRaphson));
        }
    }
}
