using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PowerFlowCore.Data;
using PowerFlowCore.Samples;
using PowerFlowCore.Solvers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerFlowCore.Benchmark
{
    /// <summary>
    /// Calculate in parallel sample grids several times for performance tests
    /// </summary>
    //[SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net60)]
    //[SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser(true)]
    public class CalculateGridParallel_AllSampleGrids
    {
        [Params(1)]
        public int N;

        private readonly List<Grid> grids = new List<Grid>();

        public CalculateGridParallel_AllSampleGrids()
        {
            grids.Add(SampleGrids.Test_Ktr());
            grids.Add(SampleGrids.Nodes4_1PV());
            grids.Add(SampleGrids.Nodes4_1PV_ZIP());
            grids.Add(SampleGrids.Nodes5_2Slack());
            grids.Add(SampleGrids.IEEE_14());
            grids.Add(SampleGrids.Nodes15_3PV());
            grids.Add(SampleGrids.IEEE_57());
            grids.Add(SampleGrids.IEEE_118());
            grids.Add(SampleGrids.Nodes197_36PV());
            grids.Add(SampleGrids.Nodes300_27PV());
            grids.Add(SampleGrids.Nodes398_35PV());
            grids.Add(SampleGrids.Nodes398_35PV_ZIP());
            grids.Add(SampleGrids.Nodes874_143PV());
            grids.Add(SampleGrids.Nodes1350_250PV());
            grids.Add(SampleGrids.Nodes2628_50PV().ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 4})
                                                  .ApplySolver(SolverType.NewtonRaphson));
        }

        [Benchmark(Baseline=true)]
        public void CalculateSeq_AllSampleGrids()
        {
            foreach (var item in grids)
            {
                Engine.Calculate(item);
            }
                
        }

        [Benchmark]
        public void CalculateParallel_AllSampleGrids()
        {
            for (int i = 0; i < N; i++)
                Engine.Calculate(grids);
        }

        [Benchmark]
        public async Task CalculateParallel_AllSampleGrids_Async()
        {
            for (int i = 0; i < N; i++)
                _ = await Engine.CalculateAsync(grids);
            
        }
    }
}
