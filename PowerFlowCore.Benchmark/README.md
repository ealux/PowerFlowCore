# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **30.05.2023 - v.0.13.3** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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

|            Method |        Mean |     Error |    StdDev |     Gen0 |     Gen1 |     Gen2 |   Allocated |
|-----------------: |------------:|----------:|----------:|---------:|---------:|---------:|------------:|
|          Test_Ktr |    14.24 us |   0.18 us |   0.16 us |      2.8 |        - |        - |    11.52 KB |
|        Nodes4_1PV |    14.04 us |   0.27 us |   0.25 us |      2.8 |        - |        - |    11.68 KB |
|    Nodes4_1PV_ZIP |    54.81 us |   0.25 us |   0.23 us |      5.6 |        - |        - |     23.3 KB |
|     Nodes5_2Slack |    16.15 us |   0.30 us |   0.40 us |      3.2 |        - |        - |    13.14 KB |
|           IEEE_14 |    33.11 us |   0.65 us |   0.97 us |      6.5 |        - |        - |    26.96 KB |
|       Nodes15_3PV |    35.79 us |   0.65 us |   0.61 us |      6.4 |        - |        - |    26.45 KB |
|           IEEE_57 |    93.38 us |   1.11 us |   1.03 us |     18.9 |      0.3 |        - |    76.82 KB |
|          IEEE_118 |   190.27 us |   2.16 us |   2.02 us |     38.8 |      0.2 |        - |   157.64 KB |
|     Nodes197_36PV |   290.56 us |   5.66 us |   5.81 us |     62.5 |      0.4 |        - |   253.75 KB |
|     Nodes300_27PV |   423.73 us |   2.77 us |   2.46 us |     99.1 |      2.4 |        - |   401.24 KB |
|     Nodes398_35PV |   524.12 us |  10.27 us |  11.42 us |     98.6 |     34.1 |        - |   470.28 KB |
| Nodes398_35PV_ZIP |   536.32 us |   4.53 us |   4.01 us |    101.5 |     39.0 |        - |   478.14 KB |
|    Nodes874_143PV | 1,481.70 us |  29.42 us |  27.52 us |    214.8 |    101.5 |        - | 1,144.53 KB |
|   Nodes1350_250PV | 2,436.99 us |  47.62 us |  50.95 us |    332.0 |    187.5 |     66.4 | 1,891.59 KB |
|    Nodes2628_50PV | 4,177.96 us |  43.25 us |  36.12 us |    492.1 |    328.1 |    164.0 | 3,110.93 KB |



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
|                  Test_Ktr |     406.9 us |     7.97 us |    10.09 us |    313.41 KB |
|                Nodes4_1PV |     272.7 us |     4.79 us |     6.23 us |    220.36 KB |
|            Nodes4_1PV_ZIP |     235.8 us |     3.57 us |     3.33 us |    178.18 KB |
|             Nodes5_2Slack |     234.5 us |     4.66 us |     4.78 us |    194.69 KB |
|                   IEEE_14 |     678.8 us |     7.98 us |     7.46 us |    542.10 KB |
|               Nodes15_3PV |   1,174.3 us |    12.75 us |    11.93 us |    963.79 KB |
|                   IEEE_57 |   1,666.3 us |    19.42 us |    18.16 us |    1716.8 KB |
|                  IEEE_118 |   3,020.6 us |    58.54 us |    62.64 us |  3,299.07 KB |
|             Nodes197_36PV |   5,109.1 us |    26.68 us |    22.28 us |  6,385.80 KB |
|             Nodes300_27PV |   9,047.3 us |   101.62 us |    84.86 us | 11,132.51 KB |
|             Nodes398_35PV |  10,549.2 us |    47.16 us |    39.38 us | 13,096.23 KB |
|         Nodes398_35PV_ZIP |  10,938.1 us |   208.51 us |   231.76 us | 13,314.09 KB |
|            Nodes874_143PV |  27,317.7 us |   253.64 us |   237.25 us | 35,248.70 KB |
|           Nodes1350_250PV |  55,741.3 us |   654.24 us |   611.97 us | 65,222.84 KB |
|    Nodes2628_50PV (GS+NR) | 174,391.7 us | 1,941.35 us | 1,621.12 us |147,979.54 KB |

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


|         Method |      Mean |   Error |   StdDev |  Gen0 |  Gen1 |  Gen2 | Allocated |
|--------------- |----------:|--------:|---------:|------:|------:|------:|----------:|
| AllSampleGrids | 305.00 ms | 5.98 ms | 11.67 ms | 78000 | 50000 | 27000 | 459.92 MB |


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

|                         Method |      N |    Mean |    Error |   StdDev |    Gen0 |   Gen1 |  Gen2 | Allocated |
|------------------------------: |:------:|--------:|---------:|---------:|--------:|-------:|------:|----------:|
|   Nodes300_27PV (sequental) x1 |   1000 |  8.60 s |  0.043 s |  0.036 s | 1931000 | 608000 | 11000 |  10.62 GB |
|  Nodes300_27PV (parallel) x100 |    100 |  5.08 s |  0.032 s |  0.028 s | 1919000 | 480000 | 11000 |  10.59 GB |
| Nodes300_27PV (parallel) x1000 |      1 |  5.08 s |  0.045 s |  0.042 s | 1900000 | 470000 | 10000 |  10.59 GB |

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
|              IEEE_14 (NR) |  conv |     702.6 us |    13.75 us |    15.84 us |   1.00 |     542.27 KB |
|           IEEE_14 (GS+NR) |  conv |     995.7 us |     5.52 us |     4.90 us |   1.41 |     696.86 KB |
|             IEEE_118 (NR) |  conv |   3,013.8 us |    59.29 us |    70.58 us |   4.29 |   3,298.25 KB |
|          IEEE_118 (GS+NR) |  conv |   3,487.5 us |    59.55 us |    52.79 us |   4.94 |   2,992.98 KB |
|        Nodes300_27PV (NR) |  conv |   8,867.5 us |    59.89 us |    56.02 us |  12.58 |  11,134.02 KB |
|     Nodes300_27PV (GS+NR) |  conv |  19,079.0 us |    84.22 us |    65.76 us |  27.00 |  22,202.18 KB |
|      Nodes1350_250PV (NR) |  conv |  54,514.9 us |   308.27 us |   273.27 us |  77.30 |  65,234.01 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  66,089.0 us |   782.69 us |   732.13 us |  93.73 |  49,646.97 KB |
|       Nodes2628_50PV (NR) |  div  | 227,600.4 us | 1,726.45 us | 1,530.45 us | 322.73 | 319,025.69 KB |
|    Nodes2628_50PV (GS+NR) |  conv | 192,739.1 us | 1,763.92 us | 1,472.95 us | 272.80 | 148,491.36 KB |


## Connectivity :o:

Checks for graph connectivity:

|            Method |        Mean |    Error |  StdDev |   Gen0 |  Gen1 | Allocated |
|-----------------: |------------:|---------:|--------:|-------:|------:|----------:|
|          Test_Ktr |     1.49 us |  0.01 us | 0.01 us |   0.51 |     - |   2.11 KB |
|        Nodes4_1PV |     1.49 us |  0.02 us | 0.02 us |   0.52 |     - |   2.13 KB |
|    Nodes4_1PV_ZIP |     1.42 us |  0.01 us | 0.01 us |   0.52 |     - |   2.13 KB |
|     Nodes5_2Slack |     1.64 us |  0.02 us | 0.02 us |   0.55 |     - |   2.27 KB |
|           IEEE_14 |     4.54 us |  0.00 us | 0.00 us |   1.01 |     - |   4.17 KB |
|       Nodes15_3PV |     4.56 us |  0.08 us | 0.07 us |   1.03 |     - |   4.23 KB |
|           IEEE_57 |    23.92 us |  0.20 us | 0.18 us |   3.20 |     - |  13.10 KB |
|          IEEE_118 |    64.78 us |  0.55 us | 0.51 us |   6.46 |     - |  26.79 KB |
|     Nodes197_36PV |    64.59 us |  0.21 us | 0.18 us |   9.27 |     - |  38.30 KB |
|     Nodes300_27PV |   129.86 us |  0.47 us | 0.39 us |  14.64 |     - |  60.22 KB |
|     Nodes398_35PV |   127.63 us |  0.42 us | 0.39 us |  17.33 |  0.24 |  71.35 KB |
| Nodes398_35PV_ZIP |   127.51 us |  1.02 us | 0.91 us |  17.33 |     - |  71.35 KB |
|    Nodes874_143PV |   573.08 us |  0.59 us | 0.53 us |  41.99 |     - | 174.94 KB |
|   Nodes1350_250PV |   782.10 us |  7.61 us | 6.74 us |  69.33 |     - | 284.33 KB |
|    Nodes2628_50PV | 2,917.68 us | 11.78 us | 9.19 us | 121.09 | 27.34 | 497.96 KB |




