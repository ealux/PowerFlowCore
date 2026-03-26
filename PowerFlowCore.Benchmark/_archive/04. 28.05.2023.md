# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **28.05.2023 - v.0.13.2** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

Grid samples list to be tested include (from [PowerFlowCore.Samples](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Samples) project):

* [Test_Ktr](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Test_Ktr.cs)
* [Nodes4_1PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes4_1PV.cs)
* [Nodes4_1PV_ZIP](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes4_1PV_ZIP.cs)
* [Nodes5_2Slack](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes5_2Slack.cs)
* [IEEE_14](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/IEEE-14.cs)
* [Nodes15_3PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes15_3PV.cs)
* [IEEE_57](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/IEEE-57.cs)
* [IEEE_118](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/IEEE-118.cs)
* [Nodes197_36PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes197_36PV.cs)
* [Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)
* [Nodes398_35PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes398_35PV.cs)
* [Nodes398_35PV_ZIP](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes398_35PV_ZIP.cs)
* [Nodes874_143PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes874_143PV.cs)
* [Nodes1350_250PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes1350_250PV.cs)
* [Nodes2628_50PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes2628_50PV.cs)

In addition to the well-known **IEEE** grids, configuration is displayed in grid name. So, **Node300_27PV** means 300 nodes where 27 is PV typed (generators). **Test_Ktr** include 4 nodes.

Tables legend:

-  **N**         : Value of the 'N' parameter
-  **Mean**      : Arithmetic mean of all measurements
-  **Error**     : Half of 99.9% confidence interval
-  **StdDev**    : Standard deviation of all measurements
-  **Gen0**      : GC Generation 0 collects per 1000 operations
-  **Gen1**      : GC Generation 1 collects per 1000 operations
-  **Gen2**      : GC Generation 2 collects per 1000 operations
-  **Allocated** : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
-  **1 us**      : 1 Microsecond (0.000001 sec)

## Environment ⌨️

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

## Create grid :hammer:

Performance tests on grid creation:

|            Method |         Mean |        Error |       StdDev |      Gen0 |      Gen1 |     Gen2 |     Allocated |
|------------------ |-------------:|-------------:|-------------:|----------:|----------:|---------:|--------------:|
|          Test_Ktr |     13.30 us |      0.26 us |      0.33 us |       2.5 |         - |        - |      10.48 KB |
|        Nodes4_1PV |     12.95 us |      0.15 us |      0.14 us |       2.5 |         - |        - |       10.4 KB |
|    Nodes4_1PV_ZIP |     55.02 us |      1.08 us |      1.01 us |       5.3 |         - |        - |      21.97 KB |
|     Nodes5_2Slack |     14.37 us |      0.20 us |      0.19 us |       2.7 |         - |        - |      11.38 KB |
|           IEEE_14 |     27.02 us |      0.51 us |      0.51 us |       5.7 |         - |        - |      23.45 KB |
|       Nodes15_3PV |     27.12 us |      0.22 us |      0.19 us |       5.7 |         - |        - |      23.76 KB |
|           IEEE_57 |     94.01 us |      1.28 us |      1.20 us |      25.5 |       0.1 |        - |     103.66 KB |
|          IEEE_118 |    312.44 us |      2.72 us |      2.41 us |      66.4 |      66.4 |     66.4 |     321.69 KB |
|     Nodes197_36PV |    653.75 us |      6.48 us |      6.06 us |     166.0 |     166.0 |    166.0 |     775.97 KB |
|     Nodes300_27PV |    981.07 us |     17.45 us |     16.33 us |     324.2 |     288.0 |    254.8 |     1671.2 KB |
|     Nodes398_35PV |  1,487.82 us |     13.83 us |     12.94 us |     417.9 |     386.7 |    337.8 |   2,776.36 KB |
| Nodes398_35PV_ZIP |  1,497.19 us |     10.91 us |     10.20 us |     416.0 |     386.7 |    333.9 |   2,784.22 KB |
|    Nodes874_143PV |  7,011.74 us |    139.02 us |    185.59 us |     703.1 |     617.1 |    546.8 |  12,678.17 KB |
|   Nodes1350_250PV | 13,488.99 us |    275.53 us |    812.42 us |    1156.2 |    1062.5 |    953.1 |  29,719.66 KB |
|    Nodes2628_50PV | 59,396.67 us |  1,116.85 us |  1,044.70 us |     888.8 |     666.6 |    666.6 | 109,942.15 KB |



## Calculate grid :triangular_ruler:

Grid calculations. Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|                    Method |         Mean |       Error |       StdDev |     Allocated |
|-------------------------- |-------------:|------------:|-------------:|--------------:|
|                  Test_Ktr |     395.1 us |     7.81 us |     11.20 us |     310.17 KB |
|                Nodes4_1PV |     267.3 us |     4.71 us |      5.79 us |     216.51 KB |
|            Nodes4_1PV_ZIP |     234.1 us |     2.61 us |      2.31 us |     174.43 KB |
|             Nodes5_2Slack |     234.0 us |     4.65 us |      4.57 us |     189.67 KB |
|                   IEEE_14 |     681.8 us |     8.71 us |      8.14 us |     533.23 KB |
|               Nodes15_3PV |   1,175.2 us |    16.42 us |     14.55 us |     953.04 KB |
|                   IEEE_57 |   1,641.9 us |     8.73 us |      8.16 us |   1,797.09 KB |
|                  IEEE_118 |   3,817.6 us |    75.01 us |     70.16 us |   3,791.17 KB |
|             Nodes197_36PV |   8,173.5 us |   111.74 us |     99.06 us |   7,952.82 KB |
|             Nodes300_27PV |  10,527.5 us |   204.79 us |    191.56 us |  14,949.68 KB |
|             Nodes398_35PV |  19,770.2 us |   394.81 us |    899.18 us |  20,009.07 KB |
|         Nodes398_35PV_ZIP |  20,372.0 us |   406.05 us |    916.53 us |  20,230.34 KB |
|            Nodes874_143PV |  47,848.1 us |   709.68 us |    629.11 us |  69,848.31 KB |
|           Nodes1350_250PV | 100,878.8 us | 1,218.64 us |  1,080.29 us | 148,715.54 KB |
|    Nodes2628_50PV (GS+NR) | 347,721.3 us | 6,920.34 us | 10,774.13 us | 468,412.63 KB |

## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|         Method |     Mean |   Error |   StdDev |  Gen0 |  Gen1 | Gen2 |   Allocated |
|--------------- |---------:|--------:|---------:|------:|------:|-----:|------------:|
| AllSampleGrids | 505.4 ms | 9.94 ms | 19.16 ms | 57000 | 22000 | 7000 | 1,011.37 MB |


## Large models collection :zap:

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|                       Method |    N |     Mean |    Error |   StdDev |    Gen0 |   Gen1 |  Gen2 | Allocated |
|----------------------------: |:----:|---------:|---------:|---------:|--------:|-------:|------:|----------:|
|          CalculateSequential | 1000 |  10.41 s |  0.047 s | 0.0421 s | 1883000 | 645000 | 44000 |  14.25 GB |
|  CalculateParallel_100_items |   10 |   5.67 s |  0.036 s | 0.0308 s | 1856000 | 482000 | 36000 |  14.22 GB |
| CalculateParallel_1000_items |    1 |   5.72 s |  0.079 s | 0.0745 s | 1837000 | 483000 | 37000 |  14.22 GB |

## Multiple solvers :dizzy:

Apply multiple solvers to calculate grid can provide different results within different model configuration.
Following tests describe:

* `NR`: use only Newton-Raphson solver;
* `GS+NR`: make 5 iteration with Gauss-Seidel solver at first then solve by Newton-Raphson.

State:

*  **conv** - converged
*  **div**  - diverged



|                    Method | State |         Mean |       Error |      StdDev |  Ratio |     Allocated |
|-------------------------: |:----: |-------------:|------------:|------------:|-------:|--------------:|
|              IEEE_14 (NR) |  conv |     674.5 us |     6.42 us |     6.01 us |   1.00 |     531.89 KB |
|           IEEE_14 (GS+NR) |  conv |   1,001.8 us |    10.93 us |    10.22 us |   1.49 |     686.55 KB |
|             IEEE_118 (NR) |  conv |   3,688.6 us |    66.22 us |    86.10 us |   5.50 |   3,790.91 KB |
|          IEEE_118 (GS+NR) |  conv |   4,064.1 us |    24.06 us |    21.33 us |   6.03 |   3,484.84 KB |
|        Nodes300_27PV (NR) |  conv |  10,405.8 us |   182.98 us |   171.16 us |  15.43 |  14,948.45 KB |
|     Nodes300_27PV (GS+NR) |  conv |  21,518.5 us |   307.22 us |   272.34 us |  31.94 |  27,922.02 KB |
|      Nodes1350_250PV (NR) |  conv |  99,562.9 us | 1,654.09 us | 1,547.23 us | 147.62 | 148,709.15 KB |
|   Nodes1350_250PV (GS+NR) |  conv | 103,599.7 us |   985.46 us |   822.91 us | 153.66 | 132,841.87 KB |
|       Nodes2628_50PV (NR) |  div  | 464,365.4 us | 9,186.43 us | 9,433.79 us | 689.65 | 746,114.87 KB |
|    Nodes2628_50PV (GS+NR) |  conv | 366,879.3 us | 7,193.64 us | 6,728.94 us | 543.97 | 468,967.52 KB |


## Connectivity :o:

Checks for graph connectivity:

|            Method |         Mean |      Error |     StdDev |     Gen0 |   Gen1 | Allocated |
|------------------ |-------------:|-----------:|-----------:|---------:|-------:|----------:|
|          Test_Ktr |      1.46 us |   0.004 us |   0.003 us |     0.51 |      - |   2.11 KB |
|        Nodes4_1PV |      1.43 us |   0.021 us |   0.020 us |     0.52 |      - |   2.13 KB |
|    Nodes4_1PV_ZIP |      1.46 us |   0.003 us |   0.002 us |     0.52 |      - |   2.13 KB |
|     Nodes5_2Slack |      1.70 us |   0.022 us |   0.021 us |     0.55 |      - |   2.27 KB |
|           IEEE_14 |      4.55 us |   0.008 us |   0.008 us |     1.01 |      - |   4.17 KB |
|       Nodes15_3PV |      4.49 us |   0.028 us |   0.026 us |     1.03 |      - |   4.23 KB |
|           IEEE_57 |     24.29 us |   0.105 us |   0.093 us |     3.20 |      - |  13.10 KB |
|          IEEE_118 |     62.51 us |   0.293 us |   0.228 us |     6.46 |      - |  26.79 KB |
|     Nodes197_36PV |     64.34 us |   0.741 us |   0.694 us |     9.27 |      - |  38.30 KB |
|     Nodes300_27PV |    129.69 us |   0.414 us |   0.387 us |    14.64 |      - |  60.22 KB |
|     Nodes398_35PV |    131.37 us |   1.286 us |   1.203 us |    17.33 |      - |  71.35 KB |
| Nodes398_35PV_ZIP |    129.07 us |   0.576 us |   0.481 us |    17.33 |      - |  71.35 KB |
|    Nodes874_143PV |    578.14 us |   1.057 us |   0.937 us |    41.99 |      - | 174.94 KB |
|   Nodes1350_250PV |    779.17 us |   6.365 us |   5.954 us |    69.33 |      - | 284.33 KB |
|    Nodes2628_50PV |  2,891.68 us |  32.027 us |  29.958 us |   121.09 |   7.81 | 497.96 KB |




