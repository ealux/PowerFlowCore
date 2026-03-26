# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **29.06.2023 - v.0.13.6** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3086/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.304
  [Host]   : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
```


|            Method |        Mean |    Error |   StdDev |   Gen0 |   Gen1 |   Gen2 |   Allocated |
|-----------------: |------------:|---------:|---------:|-------:|-------:|-------:|------------:|
|          Test_Ktr |     8.69 us |  0.03 us |  0.03 us |   2.18 |      - |      - |     8.95 KB |
|        Nodes4_1PV |     8.75 us |  0.04 us |  0.03 us |   2.22 |      - |      - |     9.11 KB |
|    Nodes4_1PV_ZIP |    38.93 us |  0.06 us |  0.06 us |   5.06 |      - |      - |     20.8 KB |
|     Nodes5_2Slack |     9.44 us |  0.01 us |  0.01 us |   2.57 |      - |      - |    10.56 KB |
|           IEEE_14 |    19.74 us |  0.08 us |  0.08 us |   5.49 |      - |      - |    22.56 KB |
|       Nodes15_3PV |    21.43 us |  0.11 us |  0.11 us |   5.73 |      - |      - |    23.51 KB |
|           IEEE_57 |    74.87 us |  0.13 us |  0.10 us |  18.67 |      - |      - |    76.59 KB |
|          IEEE_118 |   176.53 us |  0.92 us |  0.86 us |  44.18 |   0.24 |      - |   181.36 KB |
|     Nodes197_36PV |   271.21 us |  1.22 us |  1.14 us |  60.54 |   0.97 |      - |   249.01 KB |
|     Nodes300_27PV |   423.06 us |  3.05 us |  2.85 us |  90.82 |  14.16 |      - |   387.38 KB |
|     Nodes398_35PV |   521.77 us |  2.03 us |  1.80 us |  98.63 |  30.27 |      - |   470.51 KB |
| Nodes398_35PV_ZIP |   547.59 us |  1.85 us |  1.73 us | 103.51 |  31.25 |      - |   478.38 KB |
|    Nodes874_143PV | 1,674.19 us | 16.09 us | 13.44 us | 210.93 |  89.84 |      - | 1,156.08 KB |
|   Nodes1350_250PV | 2,869.32 us |  7.74 us |  6.86 us | 320.31 | 207.03 | 105.46 | 2,090.91 KB |
|    Nodes2628_50PV | 4,796.81 us | 16.46 us | 15.40 us | 507.81 | 250.00 | 117.18 | 3,128.25 KB |


## Calculate grid :triangular_ruler:

Grid calculations. 

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3086/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.304
  [Host]   : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
```


|                    Method |        Mean |       Error |      StdDev |    Allocated |
|-------------------------: |------------:|------------:|------------:|-------------:|
|                  Test_Ktr |    183.8 us |     3.05 us |     2.85 us |    125.84 KB |
|                Nodes4_1PV |    136.0 us |     1.26 us |     1.18 us |     92.03 KB |
|            Nodes4_1PV_ZIP |    242.7 us |     2.19 us |     2.05 us |    177.59 KB |
|             Nodes5_2Slack |    149.6 us |     2.56 us |     2.40 us |    112.85 KB |
|                   IEEE_14 |    702.8 us |    13.39 us |    14.89 us |    661.69 KB |
|               Nodes15_3PV |    987.0 us |    19.04 us |    17.81 us |    948.00 KB |
|                   IEEE_57 |  1,234.2 us |     7.08 us |     6.63 us |    1468.5 KB |
|                  IEEE_118 |  2,155.8 us |    14.30 us |    13.38 us |  2,418.04 KB |
|             Nodes197_36PV |  4,661.6 us |    49.62 us |    46.41 us |  5,587.42 KB |
|             Nodes300_27PV |  8,675.3 us |   142.84 us |   133.61 us |  9,542.93 KB |
|             Nodes398_35PV |  9,589.7 us |    58.73 us |    54.94 us | 11,569.27 KB |
|         Nodes398_35PV_ZIP | 16,189.5 us |   114.11 us |   101.16 us | 19,137.07 KB |
|            Nodes874_143PV | 27,787.1 us |    54.81 us |    48.58 us | 30,364.70 KB |
|           Nodes1350_250PV | 63,784.7 us | 1,260.51 us | 1,117.41 us | 56,058.80 KB |
|    Nodes2628_50PV (GS+NR) | 55,233.2 us |   259.79 us |   243.01 us | 66,868.58 KB |


## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3086/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.304
  [Host]   : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
```


|         Method |     Mean |   Error |  StdDev |  Gen0 |  Gen1 |  Gen2 | Allocated |
|--------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
| AllSampleGrids | 253.4 ms | 4.57 ms | 4.28 ms | 60000 | 40500 | 22500 | 373.11 MB |


## Large models collection :zap:

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3086/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.304
  [Host]   : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
```

|                    Method |    N |    Mean |    Error |   StdDev |    Gen0 |   Gen1 |  Gen2 | Allocated |
|:------------------------- |:---: |--------:|---------:|---------:|--------:|-------:|------:|----------:|
| Nodes300_27PV (seq) x1    | 1000 | 8.012 s | 0.0297 s | 0.0248 s | 1561000 | 345000 | 13000 |   9.10 GB |
| Nodes300_27PV (par) x10   |  100 | 4.172 s | 0.0256 s | 0.0213 s | 1554000 | 522000 | 12000 |   9.09 GB |
| Nodes300_27PV (par) x100  |   10 | 4.375 s | 0.0244 s | 0.0217 s | 1547000 | 503000 | 12000 |   9.09 GB |
| Nodes300_27PV (par) x1000 |    1 | 4.325 s | 0.0275 s | 0.0244 s | 1533000 | 479000 | 12000 |   9.09 GB |


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
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3086/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.304
  [Host]   : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
```

|                    Method | State |         Mean |       Error |      StdDev |  Ratio |     Allocated |
|:------------------------- |:----: |-------------:|------------:|------------:|-------:|--------------:|
|              IEEE_14 (NR) |  conv |     721.4 us |     7.62 us |     6.75 us |   1.00 |     661.82 KB |
|           IEEE_14 (GS+NR) |  conv |     425.2 us |     3.62 us |     3.21 us |   0.59 |     338.85 KB |
|             IEEE_118 (NR) |  conv |   2,160.5 us |    28.52 us |    23.82 us |   2.99 |   2,417.56 KB |
|          IEEE_118 (GS+NR) |  conv |   2,268.7 us |    11.82 us |    11.06 us |   3.14 |   2,159.64 KB |
|        Nodes300_27PV (NR) |  conv |   8,487.3 us |    73.82 us |    65.44 us |  11.77 |   9,542.93 KB |
|     Nodes300_27PV (GS+NR) |  conv |  14,371.2 us |    54.06 us |    50.57 us |  19.93 |  16,055.87 KB |
|      Nodes1350_250PV (NR) |  conv |  58,826.9 us | 1,100.01 us | 1,080.36 us |  81.46 |  56,003.95 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  38,854.6 us |   239.85 us |   212.62 us |  53.86 |  38,186.70 KB |
|       Nodes2628_50PV (NR) |  div  | 195,544.6 us | 2,522.52 us | 2,359.57 us | 270.71 | 243,957.00 KB |
|    Nodes2628_50PV (GS+NR) |  conv |  55,678.5 us | 1,096.05 us | 1,076.47 us |  77.13 |  67,303.21 KB |


## Connectivity :o:

Checks for graph connectivity:


Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.3086/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.304
  [Host]   : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.18 (6.0.1823.26907), X64 RyuJIT AVX2
```


|            Method |         Mean |     Error |    StdDev |  Gen0 |  Gen1 |  Allocated |
|-----------------: |-------------:|----------:|----------:|------:|------:|-----------:|
|          Test_Ktr |     304.2 ns |   3.44 ns |   2.87 ns |  0.17 |     - |      736 B |
|        Nodes4_1PV |     328.4 ns |   0.31 ns |   0.24 ns |  0.17 |     - |      744 B |
|    Nodes4_1PV_ZIP |     317.4 ns |   0.56 ns |   0.44 ns |  0.17 |     - |      744 B |
|     Nodes5_2Slack |     392.2 ns |   3.37 ns |   3.15 ns |  0.20 |     - |      840 B |
|           IEEE_14 |   1,071.1 ns |   1.78 ns |   1.67 ns |  0.41 |     - |    1,736 B |
|       Nodes15_3PV |   1,046.2 ns |   1.78 ns |   1.58 ns |  0.41 |     - |    1,760 B |
|           IEEE_57 |   4,050.4 ns |  41.03 ns |  38.38 ns |  1.42 |     - |    5,992 B |
|          IEEE_118 |   9,719.2 ns |  15.06 ns |  13.35 ns |  2.88 |     - |   12,104 B |
|     Nodes197_36PV |  13,760.9 ns |  46.57 ns |  41.28 ns |  3.99 |     - |   16,760 B |
|     Nodes300_27PV |  23,039.6 ns | 118.60 ns | 105.13 ns |  7.04 |     - |   29,616 B |
|     Nodes398_35PV |  25,731.2 ns | 184.73 ns | 172.80 ns |  7.84 |     - |   32,832 B |
| Nodes398_35PV_ZIP |  23,828.8 ns |  42.96 ns |  33.54 ns |  7.84 |     - |   32,832 B |
|    Nodes874_143PV |  73,780.0 ns | 206.38 ns | 172.33 ns | 16.96 |     - |   71,144 B |
|   Nodes1350_250PV | 126,020.5 ns | 311.34 ns | 291.23 ns | 31.25 |  5.12 |  130,936 B |
|    Nodes2628_50PV | 163,368.5 ns | 181.28 ns | 169.57 ns | 60.54 | 14.89 |  254,104 B |

