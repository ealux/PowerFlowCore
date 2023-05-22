# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **23.05.2023** (previous results in ***_archive*** folder)</u>

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
.NET SDK=7.0.203
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

## Create grid :hammer:

Performance tests on grid creation:

|            Method | N |         Mean |      Error |     StdDev |     Gen0 |     Gen1 |     Gen2 |  Allocated |
|------------------ |-- |-------------:|-----------:|-----------:|---------:|---------:|---------:|-----------:|
|          Test_Ktr | 1 |     11.20 us |   0.13  us |   0.15 us  |   2.2    |        - |        - |    9.05 KB |
|        Nodes4_1PV | 1 |     10.85 us |   0.16  us |   0.16 us  |   2.1    |        - |        - |    8.92 KB |
|    Nodes4_1PV_ZIP | 1 |     47.97 us |   0.82  us |   0.73 us  |   5.0    |        - |        - |   20.61 KB |
|     Nodes5_2Slack | 1 |     12.26 us |   0.14  us |   0.17 us  |   2.4    |        - |        - |   10.01 KB |
|           IEEE_14 | 1 |     43.23 us |   0.46  us |   0.32 us  |   5.6    |        - |        - |   22.97 KB |
|       Nodes15_3PV | 1 |     52.80 us |   0.57  us |   0.55 us  |   5.9    |        - |        - |   24.44 KB |
|           IEEE_57 | 1 |    431.55 us |   1.59  us |   1.42 us  |  27.3    |        - |        - |   113.5 KB |
|          IEEE_118 | 1 |  2,057.94 us |   5.33  us |   4.75 us  |  66.4    |  66.4    |  66.4    |  343.89 KB |
|     Nodes197_36PV | 1 |  4,483.22 us |  17.41  us |  14.53 us  | 164.0    | 164.0    | 164.0    |  813.90 KB |
|     Nodes300_27PV | 1 |  9,704.85 us |  43.92  us |  38.99 us  | 328.1    | 281.2    | 250.0    | 1733.94 KB |
|     Nodes398_35PV | 1 | 13,020.75 us |  39.08  us |  34.69 us  | 421.8    | 390.6    | 328.1    | 2858.77 KB |
| Nodes398_35PV_ZIP | 1 | 14,525.26 us |  89.05  us |  83.31 us  | 421.8    | 375.0    | 328.1    | 2866.56 KB |
|    Nodes874_143PV | 1 | 95,758.64 us | 482.20  us | 451.01 us  | 666.6    | 500.0    | 500.0    |12843.40 KB |



## Calculate grid :triangular_ruler:

Grid calculations. Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|            Method | N |         Mean |       Error |      StdDev |    Allocated |
|------------------ |-- |-------------:|------------:|------------:|-------------:|
|          Test_Ktr | 1 |     440.8 us |     3.99 us |     3.54 us |    321.91 KB |
|        Nodes4_1PV | 1 |     304.0 us |     4.55 us |     4.26 us |    224.19 KB |
|    Nodes4_1PV_ZIP | 1 |     390.9 us |     4.63 us |     4.33 us |    280.00 KB |
|     Nodes5_2Slack | 1 |     268.3 us |     5.35 us |     8.01 us |    198.47 KB |
|           IEEE_14 | 1 |     823.0 us |    14.50 us |    13.56 us |    545.35 KB |
|       Nodes15_3PV | 1 |   1,478.0 us |    13.33 us |    10.41 us |    984.62 KB |
|           IEEE_57 | 1 |   3,745.1 us |    43.15 us |    36.03 us |   3023.13 KB |
|          IEEE_118 | 1 |  12,906.1 us |   163.25 us |   136.32 us |   9834.21 KB |
|     Nodes197_36PV | 1 |  29,907.1 us |   452.39 us |   401.03 us |  27790.89 KB |
|     Nodes300_27PV | 1 |  63,686.7 us |   942.11 us |   881.25 us |  64156.33 KB |
|     Nodes398_35PV | 1 |  97,821.7 us | 1,895.90 us | 1,946.95 us | 105836.07 KB |
| Nodes398_35PV_ZIP | 1 | 113,065.0 us | 1,490.27 us | 1,394.00 us | 136388.46 KB |
|    Nodes874_143PV | 1 | 716,885.1 us | 4,915.39 us | 4,357.37 us | 564312.63 KB |


## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

|         Method | N |     Mean |    Error |   StdDev |        Gen0 |       Gen1 |      Gen2 | Allocated |
|--------------- |-- |---------:|---------:|---------:|------------:|-----------:|----------:|----------:|
| AllSampleGrids | 1 | 814.5 ms | 15.39 ms | 15.11 ms | 155000      | 11000      | 8000      | 892.38 MB |


## Large models collection :zap:

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

|                       Method |    Mean |   Error |  StdDev |          Gen0 |        Gen1 |       Gen2 | Allocated |
|----------------------------- |--------:|--------:|--------:|--------------:|------------:|-----------:|----------:|
|          CalculateSequential | 67.07 s | 0.201 s | 0.188 s | 11809000		 | 404000	   | 10000		|  61.17 GB |
|  CalculateParallel_100_items | 33.89 s | 0.252 s | 0.235 s | 10976000		 | 656000	   | 10000		|  61.12 GB |
| CalculateParallel_1000_items | 33.90 s | 0.261 s | 0.244 s | 10890000		 | 687000	   |  9000		|  61.12 GB |

## Multiple solvers :dizzy:

Apply multiple solvers to calculate grid can provide different results within different model configuration.
Following tests describe:

* `NewtonRaphsonOnly`: use only Newton-Raphson solver;
* `GaussThenNewtonRaphson`: make 5 iteration with Gauss-Seidel solver at first then solve by Newton-Raphson.

|                               Method | N |         Mean |       Error |      StdDev |  Ratio |   Allocated |
|------------------------------------: |-- |-------------:|------------:|------------:|-------:|------------:|
|            NewtonRaphsonOnly IEEE_14 | 1 |     842.3 us |    12.21 us |    11.42 us |   1.00 |   545.39 KB |
|       GaussThenNewtonRaphson IEEE_14 | 1 |   1,363.7 us |    25.66 us |    25.20 us |   1.62 |   865.29 KB |
|           NewtonRaphsonOnly IEEE_118 | 1 |  12,358.5 us |    28.65 us |    23.92 us |  14.65 |  9852.05 KB |
|      GaussThenNewtonRaphson IEEE_118 | 1 |  20,252.0 us |   298.19 us |   278.93 us |  24.05 | 12023.25 KB |
|      NewtonRaphsonOnly Nodes300_27PV | 1 |  71,965.1 us |   716.78 us |   635.41 us |  85.36 | 64188.03 KB |
| GaussThenNewtonRaphson Nodes300_27PV | 1 | 104,553.6 us | 2,087.41 us | 2,320.16 us | 124.43 | 74460.06 KB |	  


## Connectivity :o:

Checks for graph connectivity:

|            Method |  N |             Mean |         Error |        StdDev |    Gen0 |  Allocated |
|------------------ |--- |-----------------:|--------------:|--------------:|--------:|-----------:|
|          Test_Ktr |  1 |         1.6   us |     0.0182 us |     0.0161 us |  0.39   |    1.61 KB |
|        Nodes4_1PV |  1 |         1.4   us |     0.0099 us |     0.0093 us |  0.35   |    1.46 KB |
|    Nodes4_1PV_ZIP |  1 |         1.3   us |     0.0065 us |     0.0061 us |  0.35   |    1.46 KB |
|     Nodes5_2Slack |  1 |         2.2   us |     0.0145 us |     0.0128 us |  0.44   |    1.83 KB |
|           IEEE_14 |  1 |        11.9   us |     0.0849 us |     0.0794 us |  0.83   |    3.44 KB |
|       Nodes15_3PV |  1 |        11.5   us |     0.0464 us |     0.0411 us |  0.83   |    3.48 KB |
|           IEEE_57 |  1 |       186.3   us |     0.5510 us |     0.4884 us |  2.68   |   11.26 KB |
|          IEEE_118 |  1 |     1,060.2   us |     7.9586 us |     7.4445 us |  3.90   |   21.88 KB |
|     Nodes197_36PV |  1 |     3,395.1   us |     8.5316 us |     7.9804 us |  7.81   |   34.36 KB |
|     Nodes300_27PV |  1 |     9,083.5   us |     6.7608 us |     5.6456 us |       - |   53.09 KB |
|     Nodes398_35PV |  1 |    17,006.4   us |    72.4072 us |    67.7298 us |       - |   66.50 KB |
| Nodes398_35PV_ZIP |  1 |    17,118.3   us |   130.3044 us |   108.8101 us |       - |   66.50 KB |
|    Nodes874_143PV |  1 |   196,416.6   us | 3,784.9807 us | 3,355.2868 us |       - |   160.4 KB |




