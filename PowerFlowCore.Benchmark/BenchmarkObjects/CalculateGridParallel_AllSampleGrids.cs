using BenchmarkDotNet.Attributes;
using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using System.Collections.Generic;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate in parallel sample grids several times for performance tests
    /// </summary>
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.Default]
    public class CalculateGridParallel_AllSampleGrids
    {
        [Params(1, 10)]
        public int N;

        private readonly List<Grid> grids = new List<Grid>();

        public CalculateGridParallel_AllSampleGrids()
        {
            grids.Add(SampleGrids.Test_Ktr());
            grids.Add(SampleGrids.Nodes4_1PV());
            grids.Add(SampleGrids.Nodes4_1PV_ZIP());
            grids.Add(SampleGrids.IEEE_14());
            grids.Add(SampleGrids.Nodes15_3PV());
            grids.Add(SampleGrids.IEEE_57());
            grids.Add(SampleGrids.IEEE_118());
            grids.Add(SampleGrids.Nodes197_36PV());
            grids.Add(SampleGrids.Nodes300_27PV());
        }

        [Benchmark]
        public void CalculateParallel_AllSampleGrids()
        {
            for (int i = 0; i < N; i++)
                Engine.CalculateParallel(grids);
        }
    }
}
