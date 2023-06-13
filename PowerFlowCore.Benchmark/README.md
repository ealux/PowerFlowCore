# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **13.06.2023 - v.0.13.4** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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

|            Method |        Mean |    Error |   StdDev |  Gen0 |  Gen1 |  Gen2 |   Allocated |
|-----------------: |------------:|---------:|---------:|------:|------:|------:|------------:|
|          Test_Ktr |    17.07 us |  0.30 us |  0.25 us |   3.0 |     - |     - |    12.53 KB |
|        Nodes4_1PV |    16.08 us |  0.15 us |  0.14 us |   3.1 |     - |     - |    12.70 KB |
|    Nodes4_1PV_ZIP |    59.10 us |  0.26 us |  0.22 us |   5.9 |     - |     - |    24.34 KB |
|     Nodes5_2Slack |    19.38 us |  0.33 us |  0.36 us |   3.4 |     - |     - |    14.19 KB |
|           IEEE_14 |    39.62 us |  0.77 us |  0.98 us |   7.2 |     - |     - |    29.46 KB |
|       Nodes15_3PV |    39.59 us |  0.79 us |  1.08 us |   7.0 |     - |     - |    28.76 KB |
|           IEEE_57 |   107.26 us |  1.80 us |  1.68 us |  20.9 |   0.3 |     - |    85.34 KB |
|          IEEE_118 |   218.31 us |  3.03 us |  2.83 us |  42.4 |   0.7 |     - |   172.20 KB |
|     Nodes197_36PV |   315.21 us |  6.01 us |  5.62 us |  69.3 |   0.4 |     - |   281.41 KB |
|     Nodes300_27PV |   462.16 us |  3.96 us |  3.71 us |  97.6 |  30.7 |     - |   427.96 KB |
|     Nodes398_35PV |   549.42 us |  4.10 us |  3.64 us | 113.2 |  30.2 |     - |   497.29 KB |
| Nodes398_35PV_ZIP |   571.23 us |  9.51 us |  8.90 us | 114.2 |  25.3 |     - |   505.18 KB |
|    Nodes874_143PV | 1,601.28 us | 18.16 us | 16.99 us | 218.7 | 123.0 |  29.2 | 1,282.38 KB |
|   Nodes1350_250PV | 2,653.63 us | 43.04 us | 40.26 us | 332.0 | 140.6 |  66.4 | 2,011.86 KB |
|    Nodes2628_50PV | 4,137.73 us | 41.83 us | 37.08 us | 531.2 | 210.9 | 132.8 | 3,202.51 KB |



## Calculate grid :triangular_ruler:

Grid calculations. Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|                    Method |         Mean |       Error |      StdDev |    Allocated |
|-------------------------: |-------------:|------------:|------------:|-------------:|
|                  Test_Ktr |     369.2 us |     4.69 us |     4.38 us |    300.62 KB |
|                Nodes4_1PV |     256.0 us |     4.29 us |     4.41 us |    211.17 KB |
|            Nodes4_1PV_ZIP |     507.4 us |     4.85 us |     4.30 us |    408.07 KB |
|             Nodes5_2Slack |     277.0 us |     2.88 us |     2.69 us |    238.76 KB |
|                   IEEE_14 |   1,386.2 us |     6.51 us |     6.09 us |  1,159.52 KB |
|               Nodes15_3PV |   1,956.6 us |    11.00 us |     9.75 us |  1,679.63 KB |
|                   IEEE_57 |   1,861.6 us |     8.46 us |     7.91 us |  2,104.20 KB |
|                  IEEE_118 |   2,789.0 us |    14.63 us |    13.68 us |  3,258.84 KB |
|             Nodes197_36PV |   5,595.8 us |    12.44 us |    10.38 us |  7,535.16 KB |
|             Nodes300_27PV |  10,183.8 us |    45.87 us |    38.30 us | 13,018.21 KB |
|             Nodes398_35PV |  11,171.8 us |    87.59 us |    81.93 us | 15,585.33 KB |
|         Nodes398_35PV_ZIP |  19,652.1 us |   210.99 us |   197.36 us | 25,791.39 KB |
|            Nodes874_143PV |  29,693.4 us |    89.60 us |    79.43 us | 41,373.75 KB |
|           Nodes1350_250PV |  60,129.1 us |   840.18 us |   898.98 us | 72,649.01 KB |
|    Nodes2628_50PV (GS+NR) | 131,016.4 us | 1,289.86 us | 1,077.09 us | 87,854.17 KB |

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


|         Method |     Mean |   Error |  StdDev |  Gen0 |  Gen1 |  Gen2 | Allocated |
|--------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
| AllSampleGrids | 303.2 ms | 5.16 ms | 4.58 ms | 82500 | 48500 | 26500 | 497.15 MB |


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

|                    Method |    N |    Mean |  Error | StdDev |    Gen0 |   Gen1 |  Gen2 | Allocated |
|:------------------------- |:---: |--------:|-------:|-------:|--------:|-------:|------:|----------:|
| Nodes300_27PV (seq) x1    | 1000 | 9.280 s | 0.01 s | 0.01 s | 2341000 | 367000 | 13000 |  12.41 GB |
| Nodes300_27PV (par) x10   |  100 | 5.649 s | 0.02 s | 0.01 s | 2243000 | 681000 | 12000 |  12.39 GB |
| Nodes300_27PV (par) x100  |   10 | 5.800 s | 0.09 s | 0.08 s | 2264000 | 617000 | 12000 |  12.38 GB |
| Nodes300_27PV (par) x1000 |    1 | 5.733 s | 0.04 s | 0.03 s | 2260000 | 616000 | 12000 |  12.38 GB |

## Multiple solvers :dizzy:

Apply multiple solvers to calculate grid can provide different results within different model configuration.
Following tests describe:

* `NR`: use only Newton-Raphson solver;
* `GS+NR`: make 5 iteration with Gauss-Seidel solver at first then solve by Newton-Raphson.

State:

*  **conv** - converged
*  **div**  - diverged

|                    Method | State |         Mean |       Error |      StdDev |  Ratio |     Allocated |
|:------------------------- |:----: |-------------:|------------:|------------:|-------:|--------------:|
|              IEEE_14 (NR) |  conv |   1,432.1 us |    11.31 us |    10.58 us |   1.00 |   1,159.70 KB |
|           IEEE_14 (GS+NR) |  conv |     972.9 us |    12.25 us |    11.46 us |   0.68 |     672.44 KB |
|             IEEE_118 (NR) |  conv |   2,774.1 us |    11.53 us |    10.78 us |   1.94 |   3,258.48 KB |
|          IEEE_118 (GS+NR) |  conv |   3,267.0 us |    20.12 us |    16.80 us |   2.28 |   2,955.82 KB |
|        Nodes300_27PV (NR) |  conv |   9,916.1 us |    41.18 us |    38.52 us |   6.92 |  13,017.25 KB |
|     Nodes300_27PV (GS+NR) |  conv |  17,982.1 us |    71.32 us |    66.71 us |  12.56 |  21,972.06 KB |
|      Nodes1350_250PV (NR) |  conv |  59,182.1 us |   299.91 us |   280.54 us |  41.33 |  72,612.72 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  63,073.1 us |   524.68 us |   490.78 us |  44.05 |  48,798.66 KB |
|       Nodes2628_50PV (NR) |  div  | 230,943.9 us | 4,578.10 us | 6,852.28 us | 159.78 | 323,629.93 KB |
|    Nodes2628_50PV (GS+NR) |  conv | 151,665.1 us | 1,453.82 us | 1,288.77 us | 105.86 |  88,343.85 KB |


## Connectivity :o:

Checks for graph connectivity:

|            Method |         Mean |       Error |      StdDev |  Gen0 |  Gen1 |  Allocated |
|-----------------: |-------------:|------------:|------------:|------:|------:|-----------:|
|          Test_Ktr |     359.3 ns |     1.22 ns |     1.02 ns |  0.21 |     - |      880 B |
|        Nodes4_1PV |     378.3 ns |     7.51 ns |    13.72 ns |  0.21 |     - |      888 B |
|    Nodes4_1PV_ZIP |     379.9 ns |     2.93 ns |     2.60 ns |  0.21 |     - |      888 B |
|     Nodes5_2Slack |     455.6 ns |     8.94 ns |     8.36 ns |  0.24 |     - |    1,008 B |
|           IEEE_14 |   1,336.2 ns |     4.12 ns |     3.86 ns |  0.53 |     - |    2,264 B |
|       Nodes15_3PV |   1,258.8 ns |     1.15 ns |     1.02 ns |  0.53 |     - |    2,264 B |
|           IEEE_57 |   5,267.8 ns |    72.14 ns |    67.48 ns |  1.94 |     - |    8,144 B |
|          IEEE_118 |  11,768.2 ns |    28.10 ns |    24.91 ns |  3.93 |     - |   16,504 B |
|     Nodes197_36PV |  18,047.3 ns |    64.50 ns |    53.86 ns |  5.70 |     - |   23,936 B |
|     Nodes300_27PV |  27,595.4 ns |   299.90 ns |   265.85 ns |  9.61 |     - |   40,208 B |
|     Nodes398_35PV |  29,828.0 ns |   538.21 ns |   503.45 ns | 11.07 |     - |   46,352 B |
| Nodes398_35PV_ZIP |  30,900.9 ns |    38.43 ns |    35.94 ns | 11.04 |     - |   46,352 B |
|    Nodes874_143PV |  88,481.8 ns |   323.90 ns |   287.13 ns | 24.53 |     - |  103,096 B |
|   Nodes1350_250PV | 147,475.4 ns | 1,752.88 ns | 1,639.65 ns | 42.96 |  0.24 |  180,104 B |
|    Nodes2628_50PV | 196,510.7 ns |   163.70 ns |   153.12 ns | 82.03 | 18.06 |  344,040 B |




