using BenchmarkDotNet.Attributes;
using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate 1000 items 300 nodes grid collection in different ways for performance tests
    /// </summary>
    [MemoryDiagnoser]
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

        // sequental
        [Benchmark]
        public void CalculateSequential()
        {
            for (int i = 0; i < 1000; i++)
            {
                Engine.Calculate(model);
            }
        }

        // parallel

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


        // async parallel


        [Benchmark]
        public async Task CalculateParallel_10_items_Async()
        {
            for (int i = 0; i < 100; i++)
            {
                _ = await Engine.CalculateAsync(grids_10);
            }
        }


        [Benchmark]
        public async Task CalculateParallel_100_items_Async()
        {
            for (int i = 0; i < 10; i++)
            {
                _ = await Engine.CalculateAsync(grids_100);
            }
        }


        [Benchmark]
        public async Task CalculateParallel_1000_items_Async()
        {
            _ = await Engine.CalculateAsync(grids_1000);
        }
    }
}
