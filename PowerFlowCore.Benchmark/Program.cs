using System;
using BenchmarkDotNet.Running;

namespace PowerFlowCore.Benchmark
{
    public class Program
    {
        public static void Main()
        {
            // Uncomment required benchmark

            //BenchmarkRunner.Run<CreateGrid>();
            BenchmarkRunner.Run<CalculateGrid>();
            //BenchmarkRunner.Run<CalculateGridParallel_AllSampleGrids>();
            //BenchmarkRunner.Run<CalculateGridParallel_LargeModel>();
            //BenchmarkRunner.Run<CalculateGrid_MultipleSolvers>();
            //BenchmarkRunner.Run<GridConnectivity>();

            Console.ReadKey();  
        }
    }
}