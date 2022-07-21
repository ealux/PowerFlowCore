using PowerFlowCore.Samples;
using BenchmarkDotNet.Attributes;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Create sample grids several times for performance tests
    /// </summary>
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.Default]
    public class CreateGrids
    {
        [Params(1, 10, 100, 1000)]
        public int N;


        [Benchmark]
        public void Test_Ktr()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.Test_Ktr();
        }

        [Benchmark]
        public void Nodes4_1PV()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.Nodes4_1PV();
        }

        [Benchmark]
        public void Nodes4_1PV_ZIP()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.Nodes4_1PV_ZIP();
        }

        [Benchmark]
        public void IEEE_14()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.IEEE_14();
        }

        [Benchmark]
        public void Nodes15_3PV()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.Nodes15_3PV();
        }

        [Benchmark]
        public void IEEE_57()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.IEEE_57();
        }

        [Benchmark]
        public void IEEE_118()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.IEEE_118();
        }

        [Benchmark]
        public void Nodes197_36PV()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.Nodes197_36PV();
        }

        [Benchmark]
        public void Nodes300_27PV()
        {
            for (int i = 0; i < N; i++)
                SampleGrids.Nodes300_27PV();
        }
    }
}
