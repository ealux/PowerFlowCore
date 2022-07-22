using BenchmarkDotNet.Attributes;
using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using System.Collections.Generic;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate in parallel list of 100 grids with 300 nodes several times for performance tests
    /// </summary>
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.Default]
    public class CalculateDefaultParallel_100grids_of_300nodes
    {
        [Params(1, 10)]
        public int N;

        private readonly List<Grid> grids = new List<Grid>(100);

        public CalculateDefaultParallel_100grids_of_300nodes()
        {
            for (int i = 0; i < 100; i++)
            {
                grids.Add(SampleGrids.Nodes300_27PV());
            }
        }

        [Benchmark]
        public void CalculateParallel_100grids_of_300nodes()
        {
            for (int i = 0; i < N; i++)
                Engine.CalculateDefaultParallel(grids);
        }
    }
}
