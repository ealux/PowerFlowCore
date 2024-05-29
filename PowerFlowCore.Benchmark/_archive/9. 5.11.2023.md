# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **05.11.2023 - v.0.14.0** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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
-  **1 ms**      : 1 Millisecond (0.001 sec)
-  **1 us**      : 1 Microsecond (0.000001 sec)
-  **1 ns**      : 1 Nanosecond  (0.000000001 sec) 


## Create grid :hammer:

Performance tests on grid creation:

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3570/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.403
  [Host]   : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
```
|            Method |        Mean |    Error |   StdDev |   Gen0 |   Gen1 |   Gen2 |   Allocated |
|-----------------: |------------:|---------:|---------:|-------:|-------:|-------:|------------:|
|          Test_Ktr |     8.60 us |  0.13 us |  0.11 us |   2.18 |      - |      - |     8.92 KB |
|        Nodes4_1PV |     8.53 us |  0.02 us |  0.01 us |   2.21 |      - |      - |     9.09 KB |
|    Nodes4_1PV_ZIP |    38.93 us |  0.07 us |  0.06 us |   5.06 |      - |      - |    20.78 KB |
|     Nodes5_2Slack |     9.81 us |  0.10 us |  0.10 us |   2.57 |      - |      - |    10.53 KB |
|           IEEE_14 |    20.26 us |  0.15 us |  0.13 us |   5.49 |      - |      - |    22.53 KB |
|       Nodes15_3PV |    21.77 us |  0.12 us |  0.11 us |   5.73 |      - |      - |    23.49 KB |
|           IEEE_57 |    75.54 us |  0.08 us |  0.07 us |  18.67 |   0.12 |      - |    76.57 KB |
|          IEEE_118 |   180.20 us |  1.34 us |  1.19 us |  44.18 |   0.97 |      - |   181.33 KB |
|     Nodes197_36PV |   277.15 us |  3.04 us |  2.69 us |  60.54 |   0.97 |      - |   248.98 KB |
|     Nodes300_27PV |   424.46 us |  2.99 us |  2.65 us |  90.82 |  15.13 |      - |   387.36 KB |
|     Nodes398_35PV |   522.80 us |  1.43 us |  1.34 us |  99.60 |  31.25 |      - |    470.5 KB |
| Nodes398_35PV_ZIP |   542.55 us |  2.75 us |  2.44 us | 102.53 |  33.20 |      - |   478.36 KB |
|    Nodes874_143PV | 1,677.11 us | 17.97 us | 16.81 us | 210.93 |  89.84 |      - | 1,156.09 KB |
|   Nodes1350_250PV | 2,898.54 us |  7.87 us |  6.57 us | 320.31 | 207.03 | 105.46 | 2,090.93 KB |
|    Nodes2628_50PV | 4,653.85 us | 10.31 us |  9.14 us | 539.06 | 265.62 |  85.93 | 3,128.13 KB |

## Calculate grid :triangular_ruler:

Grid calculations. 

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3570/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.403
  [Host]   : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
```


|                    Method |         Mean |       Error |      StdDev | Gen0 | Gen1 | Gen2 |    Allocated |
|-------------------------: |-------------:|------------:|------------:|-----:|-----:|-----:|-------------:|
|                  Test_Ktr |    102.96 us |     1.61 us |     1.43 us |   22 |    - |    - |      93.6 KB |
|                Nodes4_1PV |     80.30 us |     0.67 us |     0.59 us |   17 |    - |    - |     71.02 KB |
|            Nodes4_1PV_ZIP |    147.85 us |     2.43 us |     2.15 us |   32 |    - |    - |    134.98 KB |
|             Nodes5_2Slack |     88.85 us |     0.89 us |     0.79 us |   21 |    - |    - |     89.78 KB |
|                   IEEE_14 |    458.79 us |     2.17 us |     2.03 us |  127 |    - |    - |    523.92 KB |
|               Nodes15_3PV |    625.73 us |     2.01 us |     1.68 us |  180 |    6 |    - |    741.79 KB |
|                   IEEE_57 |  1,127.14 us |     4.51 us |     3.99 us |  296 |    2 |    - |  1,216.56 KB |
|                  IEEE_118 |  2,148.00 us |    10.90 us |    10.19 us |  464 |  105 |    - |  1,876.17 KB |
|             Nodes197_36PV |  4,446.12 us |    17.31 us |    15.35 us |  875 |  296 |    - |  4,255.25 KB |
|             Nodes300_27PV |  8,219.47 us |    69.90 us |    61.97 us | 1281 |  765 |  484 |  7,260.05 KB |
|             Nodes398_35PV |  9,330.71 us |   184.17 us |   307.70 us | 1406 |  765 |  593 |  8,634.62 KB |
|         Nodes398_35PV_ZIP | 15,221.22 us |   250.37 us |   221.95 us | 2328 | 1171 | 1000 | 14,250.29 KB |
|            Nodes874_143PV | 26,116.71 us |   262.95 us |   233.09 us | 4437 | 3906 | 3500 | 23,157.56 KB |
|           Nodes1350_250PV | 52,721.49 us | 1,019.50 us | 1,429.20 us | 9700 | 7600 | 6500 | 43,070.33 KB |
|    Nodes2628_50PV (GS+NR) | 52,249.67 us |   923.31 us | 1,200.56 us | 9300 | 6900 | 6100 | 50,614.47 KB |


## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3570/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.403
  [Host]   : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
```


|                     Method |     Mean |   Error |  StdDev |  Gen0 |  Gen1 |  Gen2 | Allocated |
|--------------------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
| AllSampleGrids (seq)       | 299.7 ms | 4.28 ms | 3.79 ms | 51000 | 38000 | 31000 | 277.36 MB |
| AllSampleGrids (par)       | 221.1 ms | 4.25 ms | 4.18 ms | 47000 | 36666 | 26000 | 277.41 MB |
| AllSampleGrids (par async) | 217.5 ms | 3.84 ms | 3.59 ms | 42333 | 29333 | 21000 | 277.26 MB |


## Large models collection :zap:

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3570/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.403
  [Host]   : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
```

|                          Method |    N |  Mean |  Error | StdDev |    Gen0 |   Gen1 |  Gen2 | Allocated |
|:------------------------------- |:---: |------:|-------:|-------:|--------:|-------:|------:|----------:|
| Nodes300_27PV (seq) x1          | 1000 | 7.6 s | 0.04 s | 0.04 s | 1039000 | 425000 | 13000 |   6.92 GB |
| Nodes300_27PV (par) x10         |  100 | 3.2 s | 0.02 s | 0.02 s | 1112000 | 391000 | 12000 |   6.91 GB |
| Nodes300_27PV (par) x100        |   10 | 3.3 s | 0.04 s | 0.03 s | 1102000 | 388000 | 12000 |   6.91 GB |
| Nodes300_27PV (par) x1000       |    1 | 3.4 s | 0.05 s | 0.05 s | 1107000 | 383000 | 12000 |   6.91 GB |
| Nodes300_27PV (par async) x10   |  100 | 3.5 s | 0.07 s | 0.12 s | 1106000 | 385000 | 12000 |   6.92 GB |
| Nodes300_27PV (par async) x100  |   10 | 3.5 s | 0.05 s | 0.04 s | 1097000 | 382000 | 11000 |   6.91 GB |
| Nodes300_27PV (par async) x1000 |    1 | 3.5 s | 0.05 s | 0.05 s | 1121000 | 391000 | 10000 |   6.91 GB |


## Multiple solvers :dizzy:

Apply multiple solvers to calculate grid can provide different results within different model configuration.
Following tests describe:

* `NR`: use only Newton-Raphson solver;
* `GS+NR`: make 5 iteration with Gauss-Seidel solver at first then solve by Newton-Raphson.

State:

*  **conv** - converged
*  **div**  - diverged

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3570/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.403
  [Host]   : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
```

|                    Method | State |         Mean |       Error |      StdDev |  Ratio |    Allocated |
|:------------------------- |:----: |-------------:|------------:|------------:|-------:|-------------:|
|              IEEE_14 (NR) |  conv |     476.7 us |     6.90 us |     6.46 us |   1.00 |    523.92 KB |
|           IEEE_14 (GS+NR) |  conv |     287.8 us |     5.62 us |     6.01 us |   0.61 |    266.28 KB |
|             IEEE_118 (NR) |  conv |   2,153.5 us |    20.83 us |    18.46 us |   4.52 |  1,875.46 KB |
|          IEEE_118 (GS+NR) |  conv |   2,260.6 us |    20.06 us |    17.79 us |   4.74 |  1,711.77 KB |
|        Nodes300_27PV (NR) |  conv |   7,774.1 us |    31.95 us |    29.89 us |  16.31 |  7,259.87 KB |
|     Nodes300_27PV (GS+NR) |  conv |  13,534.9 us |    78.48 us |    73.41 us |  28.40 | 12,176.71 KB |
|      Nodes1350_250PV (NR) |  conv |  51,156.0 us |   405.71 us |   338.79 us | 107.21 | 43,069.08 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  37,632.4 us |   190.71 us |   178.39 us |  78.96 |  3,0091.4 KB |
|       Nodes2628_50PV (NR) |  div  | 175,815.3 us | 2,488.30 us | 2,327.56 us | 368.90 | 17,8600.6 KB |
|    Nodes2628_50PV (GS+NR) |  conv |  52,262.5 us |   153.71 us |   136.26 us | 109.58 | 51,039.84 KB |


## Connectivity :o:

Checks for graph connectivity:


Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3570/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.403
  [Host]   : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
```


|            Method |         Mean |       Error |      StdDev |  Gen0 |  Gen1 |  Allocated |
|-----------------: |-------------:|------------:|------------:|------:|------:|-----------:|
|          Test_Ktr |     308.5 ns |     4.31 ns |     4.03 ns |  0.17 |     - |      736 B |
|        Nodes4_1PV |     319.8 ns |     0.40 ns |     0.31 ns |  0.17 |     - |      744 B |
|    Nodes4_1PV_ZIP |     325.1 ns |     0.41 ns |     0.36 ns |  0.17 |     - |      744 B |
|     Nodes5_2Slack |     387.7 ns |     1.88 ns |     1.67 ns |  0.20 |     - |      840 B |
|           IEEE_14 |   1,077.0 ns |     4.85 ns |     4.54 ns |  0.41 |     - |    1,736 B |
|       Nodes15_3PV |   1,016.4 ns |    11.27 ns |    10.55 ns |  0.41 |     - |    1,760 B |
|           IEEE_57 |   4,026.6 ns |    23.11 ns |    19.30 ns |  1.42 |     - |    5,992 B |
|          IEEE_118 |   9,820.2 ns |    20.91 ns |    18.54 ns |  2.88 |     - |   12,104 B |
|     Nodes197_36PV |  14,223.2 ns |    78.26 ns |    73.21 ns |  3.99 |     - |   16,760 B |
|     Nodes300_27PV |  22,847.5 ns |   179.71 ns |   159.31 ns |  7.04 |     - |   29,616 B |
|     Nodes398_35PV |  23,657.9 ns |   260.46 ns |   243.64 ns |  7.84 |     - |   32,832 B |
| Nodes398_35PV_ZIP |  23,544.8 ns |    21.95 ns |    19.46 ns |  7.84 |     - |   32,832 B |
|    Nodes874_143PV |  73,503.0 ns |   314.21 ns |   278.54 ns | 16.96 |     - |   71,144 B |
|   Nodes1350_250PV | 127,366.7 ns |    95.02 ns |    79.34 ns | 31.25 |  5.12 |  130,936 B |
|    Nodes2628_50PV | 160,792.5 ns | 1,738.28 ns | 1,451.54 ns | 60.54 | 14.89 |  254,104 B |

