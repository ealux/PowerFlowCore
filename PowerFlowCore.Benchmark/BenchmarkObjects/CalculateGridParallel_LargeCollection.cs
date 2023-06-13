using BenchmarkDotNet.Attributes;
using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using System.Collections.Generic;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate 1000 items 300 nodes grid collection in different ways for performance tests
    /// </summary>
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.Default]
    public class CalculateGridParallel_LargeModel
    {
        private readonly List<Grid> grids_1000 = new List<Grid>(1000);
        private readonly List<Grid> grids_100 = new List<Grid>(100);
        private readonly List<Grid> grids_10 = new List<Grid>(10);
        private readonly Grid model;


        public CalculateGridParallel_LargeModel()
        {
            model = SampleGrids.Nodes300_27PV();
            for (int i = 0; i < 10; i++)
                grids_10.Add(SampleGrids.Nodes300_27PV());
            for (int i = 0; i < 100; i++)
                grids_100.Add(SampleGrids.Nodes300_27PV());            
            for (int i = 0; i < 1000; i++)
                grids_1000.Add(SampleGrids.Nodes300_27PV());
        }

        [Benchmark]
        public void CalculateSequential()
        {
            for (int i = 0; i < 1000; i++)
            {
                Engine.Calculate(model);
            }
        }

        [Benchmark]
        public void CalculateParallel_10_items()
        {
            for (int i = 0; i < 100; i++)
            {
                Engine.Calculate(grids_10);
            }
        }


        [Benchmark]
        public void CalculateParallel_100_items()
        {
            for (int i = 0; i < 10; i++)
            {
                Engine.Calculate(grids_100);
            }
        }
        

        [Benchmark]
        public void CalculateParallel_1000_items()
        {
            Engine.Calculate(grids_1000);
        }
    }
}
