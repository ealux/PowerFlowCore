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
-  **1 ns**      : 1 Nanosecond  (0.000000001 sec) 


## Create grid :hammer:

Performance tests on grid creation:

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|            Method |        Mean |     Error |    StdDev |  Gen0 |  Gen1 |  Gen2 |   Allocated |
|-----------------: |------------:|----------:|----------:|------:|------:|------:|------------:|
|          Test_Ktr |    14.24 us |   0.18 us |   0.16 us |   2.8 |     - |     - |    11.52 KB |
|        Nodes4_1PV |    14.04 us |   0.27 us |   0.25 us |   2.8 |     - |     - |    11.68 KB |
|    Nodes4_1PV_ZIP |    54.81 us |   0.25 us |   0.23 us |   5.6 |     - |     - |     23.3 KB |
|     Nodes5_2Slack |    16.15 us |   0.30 us |   0.40 us |   3.2 |     - |     - |    13.14 KB |
|           IEEE_14 |    33.11 us |   0.65 us |   0.97 us |   6.5 |     - |     - |    26.96 KB |
|       Nodes15_3PV |    35.79 us |   0.65 us |   0.61 us |   6.4 |     - |     - |    26.45 KB |
|           IEEE_57 |    93.38 us |   1.11 us |   1.03 us |  18.9 |   0.3 |     - |    76.82 KB |
|          IEEE_118 |   190.27 us |   2.16 us |   2.02 us |  38.8 |   0.2 |     - |   157.64 KB |
|     Nodes197_36PV |   290.56 us |   5.66 us |   5.81 us |  62.5 |   0.4 |     - |   253.75 KB |
|     Nodes300_27PV |   423.73 us |   2.77 us |   2.46 us |  99.1 |   2.4 |     - |   401.24 KB |
|     Nodes398_35PV |   524.12 us |  10.27 us |  11.42 us |  98.6 |  34.1 |     - |   470.28 KB |
| Nodes398_35PV_ZIP |   536.32 us |   4.53 us |   4.01 us | 101.5 |  39.0 |     - |   478.14 KB |
|    Nodes874_143PV | 1,481.70 us |  29.42 us |  27.52 us | 214.8 | 101.5 |     - | 1,144.53 KB |
|   Nodes1350_250PV | 2,436.99 us |  47.62 us |  50.95 us | 332.0 | 187.5 |  66.4 | 1,891.59 KB |
|    Nodes2628_50PV | 4,177.96 us |  43.25 us |  36.12 us | 492.1 | 328.1 | 164.0 | 3,110.93 KB |


Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|            Method |        Mean |    Error |   StdDev |  Gen0 |  Gen1 |  Gen2 |   Allocated |
|-----------------: |------------:|---------:|---------:|------:|------:|------:|------------:|
|          Test_Ktr |    14.73 us |  0.19 us |  0.18 us |   4.1 |     - |     - |    12.66 KB |
|        Nodes4_1PV |    14.08 us |  0.12 us |  0.10 us |   4.1 |     - |     - |    12.76 KB |
|    Nodes4_1PV_ZIP |    40.59 us |  0.37 us |  0.35 us |   7.9 |     - |     - |    24.34 KB |
|     Nodes5_2Slack |    15.08 us |  0.06 us |  0.05 us |   4.6 |     - |     - |    14.24 KB |
|           IEEE_14 |    27.96 us |  0.09 us |  0.08 us |   9.2 |     - |     - |    28.17 KB |
|       Nodes15_3PV |    29.38 us |  0.08 us |  0.08 us |   9.3 |     - |     - |    28.42 KB |
|           IEEE_57 |    89.35 us |  0.24 us |  0.22 us |  27.7 |     - |     - |    84.72 KB |
|          IEEE_118 |   197.60 us |  0.77 us |  0.72 us |  56.3 |     - |     - |   171.68 KB |
|     Nodes197_36PV |   309.96 us |  3.85 us |  3.41 us |  91.3 |   7.3 |     - |   280.46 KB |
|     Nodes300_27PV |   498.89 us |  3.41 us |  3.19 us | 121.0 |  31.2 |     - |   426.55 KB |
|     Nodes398_35PV |   605.83 us |  7.28 us |  6.81 us | 132.8 |  38.0 |     - |   495.62 KB |
| Nodes398_35PV_ZIP |   623.43 us |  3.18 us |  2.98 us | 150.3 |  24.4 |     - |   503.55 KB |
|    Nodes874_143PV | 1,774.92 us | 31.14 us | 29.13 us | 226.5 | 105.4 |  42.9 | 1,279.53 KB |
|   Nodes1350_250PV | 2,866.07 us | 10.88 us | 10.18 us | 335.9 | 136.7 |  66.4 | 2,008.63 KB |
|    Nodes2628_50PV | 4,653.50 us | 17.10 us | 14.28 us | 531.2 | 210.9 | 132.8 | 3,199.30 KB |


## Calculate grid :triangular_ruler:

Grid calculations. 

Config:

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


Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|                    Method |         Mean |       Error |      StdDev |    Allocated |
|-------------------------: |-------------:|------------:|------------:|-------------:|
|                  Test_Ktr |     322.2 us |     3.06 us |     2.87 us |    303.60 KB |
|                Nodes4_1PV |     222.8 us |     1.51 us |     1.42 us |    213.42 KB |
|            Nodes4_1PV_ZIP |     428.0 us |     2.58 us |     2.29 us |    412.24 KB |
|             Nodes5_2Slack |     247.6 us |     1.46 us |     1.29 us |    241.56 KB |
|                   IEEE_14 |     941.6 us |     5.66 us |     5.30 us |  1,076.32 KB |
|               Nodes15_3PV |   1,312.9 us |     5.51 us |     5.15 us |  1,557.20 KB |
|                   IEEE_57 |   1,693.7 us |    19.22 us |    17.98 us |  2,048.85 KB |
|                  IEEE_118 |   2,777.0 us |    10.52 us |     9.84 us |  3,205.36 KB |
|             Nodes197_36PV |   5,911.2 us |    39.77 us |    35.26 us |  7,476.74 KB |
|             Nodes300_27PV |  11,089.7 us |    74.29 us |    69.49 us | 12,952.25 KB |
|             Nodes398_35PV |  12,568.4 us |    71.59 us |    66.96 us | 15,519.47 KB |
|         Nodes398_35PV_ZIP |  21,591.2 us |   143.36 us |   134.10 us | 25,677.37 KB |
|            Nodes874_143PV |  32,994.5 us |   640.51 us |   599.14 us | 41,294.95 KB |
|           Nodes1350_250PV |  70,267.7 us | 1,388.38 us | 2,867.25 us | 72,544.96 KB |
|    Nodes2628_50PV (GS+NR) | 133,859.1 us | 1,745.45 us | 1,547.30 us | 87,707.03 KB |


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


Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|         Method |     Mean |   Error |   StdDev |  Gen0 |  Gen1 |  Gen2 | Allocated |
|--------------- |---------:|--------:|---------:|------:|------:|------:|----------:|
| AllSampleGrids | 379.7 ms | 7.43 ms | 13.59 ms | 78000 | 46000 | 22000 | 497.09 MB |


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


Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|                    Method |    N |     Mean |  Error | StdDev |    Gen0 |   Gen1 |  Gen2 | Allocated |
|:------------------------- |:---: |---------:|-------:|-------:|--------:|-------:|------:|----------:|
| Nodes300_27PV (seq) x1    | 1000 | 10.491 s | 0.01 s | 0.01 s | 2493000 | 453000 | 13000 |  12.35 GB |
| Nodes300_27PV (par) x10   |  100 |  5.736 s | 0.05 s | 0.04 s | 2086000 | 612000 | 13000 |  12.32 GB |
| Nodes300_27PV (par) x100  |   10 |  5.551 s | 0.08 s | 0.07 s | 2086000 | 691000 | 13000 |  12.32 GB |
| Nodes300_27PV (par) x1000 |    1 |  5.498 s | 0.06 s | 0.05 s | 2087000 | 775000 | 13000 |  12.32 GB |

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
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

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


Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|                    Method | State |         Mean |       Error |      StdDev |  Ratio |     Allocated |
|:------------------------- |:----: |-------------:|------------:|------------:|-------:|--------------:|
|              IEEE_14 (NR) |  conv |     943.4 us |     4.36 us |     4.08 us |   1.00 |   1,075.64 KB |
|           IEEE_14 (GS+NR) |  conv |     604.8 us |     1.87 us |     1.75 us |   0.64 |    ,602.34 KB |
|             IEEE_118 (NR) |  conv |   2,714.9 us |    10.58 us |     9.38 us |   2.88 |   3,205.03 KB |
|          IEEE_118 (GS+NR) |  conv |   2,963.5 us |    23.24 us |    21.74 us |   3.14 |   2,882.08 KB |
|        Nodes300_27PV (NR) |  conv |  11,084.7 us |    86.98 us |    81.36 us |  11.75 |  12,952.47 KB |
|     Nodes300_27PV (GS+NR) |  conv |  19,569.6 us |   136.77 us |   127.94 us |  20.74 |  21,827.43 KB |
|      Nodes1350_250PV (NR) |  conv |  68,047.9 us | 1,347.74 us | 2,137.66 us |  72.91 |  72,578.19 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  70,262.2 us | 1,343.15 us | 1,492.90 us |  74.63 |  48,699.86 KB |
|       Nodes2628_50PV (NR) |  div  | 266,023.9 us | 5,194.92 us | 4,859.33 us | 281.97 | 324,956.01 KB |
|    Nodes2628_50PV (GS+NR) |  conv | 157,131.1 us | 3,068.52 us | 4,400.78 us | 167.98 |  88,213.72 KB |


## Connectivity :o:

Checks for graph connectivity:


Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


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

Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|            Method |         Mean |       Error |    StdDev |     Gen0 |  Allocated |
|-----------------: |-------------:|------------:|----------:|---------:|-----------:|
|          Test_Ktr |     443.4 ns |     4.62 ns |   4.32 ns |   0.2804 |      880 B |
|        Nodes4_1PV |     444.1 ns |     3.95 ns |   3.69 ns |   0.2828 |      888 B |
|    Nodes4_1PV_ZIP |     458.1 ns |     8.29 ns |   9.87 ns |   0.2823 |      888 B |
|     Nodes5_2Slack |     552.1 ns |     3.43 ns |   3.21 ns |   0.3204 |    1,008 B |
|           IEEE_14 |   1,471.5 ns |    14.71 ns |  13.76 ns |   0.7210 |    2,264 B |
|       Nodes15_3PV |   1,388.4 ns |    27.14 ns |  26.65 ns |   0.7210 |    2,264 B |
|           IEEE_57 |   5,484.9 ns |   109.13 ns | 173.09 ns |   2.5940 |    8,144 B |
|          IEEE_118 |  11,597.0 ns |    76.85 ns |  60.00 ns |   5.2490 |   16,504 B |
|     Nodes197_36PV |  18,572.7 ns |   368.26 ns | 584.10 ns |   7.6294 |   23,936 B |
|     Nodes300_27PV |  30,523.6 ns |   266.37 ns | 207.96 ns |  12.8174 |   40,208 B |
|     Nodes398_35PV |  34,150.9 ns |   164.71 ns | 146.01 ns |  14.7705 |   46,352 B |
| Nodes398_35PV_ZIP |  33,932.4 ns |   156.90 ns | 139.09 ns |  14.7705 |   46,352 B |
|    Nodes874_143PV | 113,515.4 ns |   437.20 ns | 408.96 ns |  32.8369 |  103,096 B |
|   Nodes1350_250PV | 190,383.1 ns |   428.65 ns | 379.99 ns |  57.3730 |  180,104 B |
|    Nodes2628_50PV | 254,455.2 ns | 1,016.01 ns | 950.37 ns | 109.3750 |  344,040 B |

