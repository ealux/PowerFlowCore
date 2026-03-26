# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **29.05.2024 - v.0.14.1** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
```
|            Method | Mean        | Error    | StdDev   | Gen0   | Gen1   | Gen2   | Allocated   |
|-----------------: |------------:|---------:|---------:|-------:|-------:|-------:|------------:|
|          Test_Ktr |     5.17 us |  0.08 us |  0.07 us |   2.18 |      - |      - |     8.92 KB |
|        Nodes4_1PV |     4.98 us |  0.07 us |  0.07 us |   2.22 |      - |      - |     9.09 KB |
|    Nodes4_1PV_ZIP |    15.69 us |  0.19 us |  0.16 us |   5.06 |      - |      - |    20.78 KB |
|     Nodes5_2Slack |     5.56 us |  0.10 us |  0.09 us |   2.57 |      - |      - |    10.53 KB |
|           IEEE_14 |    11.65 us |  0.01 us |  0.01 us |   5.50 |      - |      - |    22.53 KB |
|       Nodes15_3PV |    12.52 us |  0.22 us |  0.20 us |   5.73 |      - |      - |    23.49 KB |
|           IEEE_57 |    48.88 us |  0.14 us |  0.13 us |  18.73 |   0.06 |      - |    76.57 KB |
|          IEEE_118 |   127.98 us |  0.62 us |  0.55 us |  44.18 |   0.48 |      - |   181.33 KB |
|     Nodes197_36PV |   190.87 us |  0.69 us |  0.64 us |  60.79 |   0.24 |      - |   248.98 KB |
|     Nodes300_27PV |   291.86 us |  2.45 us |  2.17 us |  92.28 |   6.83 |      - |   387.36 KB |
|     Nodes398_35PV |   352.79 us |  2.79 us |  2.47 us | 100.09 |  26.36 |      - |   470.49 KB |
| Nodes398_35PV_ZIP |   372.33 us |  2.11 us |  1.97 us | 103.51 |  32.22 |      - |   478.36 KB |
|    Nodes874_143PV | 1,158.56 us |  2.22 us |  1.73 us | 214.84 | 140.62 |      - | 1,154.76 KB |
|   Nodes1350_250PV | 2,059.57 us | 10.80 us | 10.10 us | 320.31 | 304.68 | 105.46 | 2,089.78 KB |
|    Nodes2628_50PV | 3,115.98 us | 16.20 us | 15.15 us | 531.25 | 421.87 |  78.12 | 3,127.31 KB |

## Calculate grid :triangular_ruler:

Grid calculations. 

Config:

```ini
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
```


|                    Method | Mean         | Error      | StdDev     | Gen0 | Gen1 | Gen2 | Allocated    |
|-------------------------: |-------------:|-----------:|-----------:|-----:|-----:|-----:|-------------:|
|                  Test_Ktr |     77.95 us |   0.898 us |   0.840 us |   22 |    - |    - |     92.12 KB |
|                Nodes4_1PV |     57.55 us |   1.143 us |   1.403 us |   17 |    - |    - |     70.04 KB |
|            Nodes4_1PV_ZIP |    100.42 us |   0.491 us |   0.435 us |   32 |    - |    - |    132.98 KB |
|             Nodes5_2Slack |     64.67 us |   0.671 us |   0.627 us |   21 |    - |    - |     88.79 KB |
|                   IEEE_14 |    396.37 us |   7.618 us |   7.823 us |  126 |    2 |    - |    521.68 KB |
|               Nodes15_3PV |    508.58 us |   7.432 us |   6.952 us |  179 |    4 |    - |    738.56 KB |
|                   IEEE_57 |    914.27 us |   8.551 us |   7.998 us |  296 |    7 |    - |  1,215.28 KB |
|                  IEEE_118 |  1,766.09 us |  19.565 us |  18.301 us |  460 |  132 |    - |  1,872.40 KB |
|             Nodes197_36PV |  3,802.11 us |   8.674 us |   7.689 us |  859 |  453 |    - |  4,252.78 KB |
|             Nodes300_27PV |  6,926.09 us |  29.689 us |  26.319 us | 1234 |  664 |  492 |  7,258.64 KB |
|             Nodes398_35PV |  7,724.37 us |  28.996 us |  25.704 us | 1406 |  945 |  593 |  8,634.45 KB |
|         Nodes398_35PV_ZIP | 12,706.27 us |  81.108 us |  75.868 us | 2484 | 1468 |  984 | 14,248.56 KB |
|            Nodes874_143PV | 22,576.15 us | 108.193 us | 101.204 us | 4500 | 3656 | 3437 | 23,151.00 KB |
|           Nodes1350_250PV | 45,414.36 us | 284.256 us | 265.893 us | 9363 | 8090 | 6454 | 43,058.67 KB |
|    Nodes2628_50PV (GS+NR) | 41,576.86 us | 162.146 us | 151.672 us | 9142 | 6571 | 6000 | 50,617.35 KB |


## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

Config:

```ini
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
```


|                     Method | Mean     | Error   | StdDev  | Ratio | Gen0  | Gen1  | Gen2  | Allocated |
|--------------------------- |---------:|--------:|--------:|------:|------:|------:|------:|----------:|
| AllSampleGrids (seq)       | 257.7 ms | 2.84 ms | 2.66 ms |  1.00 | 51000 | 42000 | 33000 | 277.39 MB |
| AllSampleGrids (par)       | 186.6 ms | 1.41 ms | 1.25 ms |  0.72 | 45000 | 37000 | 25000 | 277.37 MB |
| AllSampleGrids (par async) | 188.5 ms | 1.84 ms | 1.63 ms |  0.73 | 42000 | 33000 | 22000 | 277.25 MB |


## Large models collection :zap:

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

Config:

```ini
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
```

|                          Method |    N | Mean    | Error   | StdDev  | Gen0    | Gen1   | Gen2  | Allocated |
|:------------------------------- |:---: |--------:|--------:|--------:|--------:|-------:|------:|----------:|
| Nodes300_27PV (seq) x1          | 1000 | 6.810 s | 0.016 s | 0.015 s | 1043000 | 545000 | 13000 |   6.92 GB |
| Nodes300_27PV (par) x10         |  100 | 2.890 s | 0.016 s | 0.014 s | 1012000 | 617000 | 13000 |   6.91 GB |
| Nodes300_27PV (par) x100        |   10 | 3.030 s | 0.013 s | 0.011 s | 1011000 | 504000 | 13000 |   6.91 GB |
| Nodes300_27PV (par) x1000       |    1 | 3.017 s | 0.020 s | 0.018 s | 1011000 | 482000 | 13000 |   6.91 GB |
| Nodes300_27PV (par async) x10   |  100 | 2.995 s | 0.028 s | 0.026 s | 1013000 | 635000 | 13000 |   6.92 GB |
| Nodes300_27PV (par async) x100  |   10 | 3.087 s | 0.018 s | 0.017 s | 1014000 | 530000 | 12000 |   6.91 GB |
| Nodes300_27PV (par async) x1000 |    1 | 3.171 s | 0.018 s | 0.016 s | 1010000 | 489000 | 11000 |   6.91 GB |


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
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
```

|                    Method | State | Mean         | Error       | StdDev      | Ratio  | Allocated     |
|:------------------------- |:----: |-------------:|------------:|------------:|-------:|--------------:|
|              IEEE_14 (NR) |  conv |     400.5 us |     6.40 us |     5.98 us |   1.00 |     521.68 KB |
|           IEEE_14 (GS+NR) |  conv |     218.4 us |     3.30 us |     3.08 us |   0.55 |     264.20 KB |
|             IEEE_118 (NR) |  conv |   1,779.5 us |     7.57 us |     5.91 us |   4.44 |   1,872.51 KB |
|          IEEE_118 (GS+NR) |  conv |   1,795.7 us |    25.70 us |    24.04 us |   4.48 |   1,707.59 KB |
|        Nodes300_27PV (NR) |  conv |   6,836.2 us |    29.41 us |    26.08 us |  17.05 |   7,258.69 KB |
|     Nodes300_27PV (GS+NR) |  conv |  11,904.8 us |    36.59 us |    34.22 us |  29.73 |  12,173.38 KB |
|      Nodes1350_250PV (NR) |  conv |  48,136.4 us |   423.53 us |   396.17 us | 120.21 |  43,036.15 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  32,057.6 us |   169.51 us |   150.26 us |  79.95 |  30,080.71 KB |
|       Nodes2628_50PV (NR) |  div  | 153,492.8 us | 1,660.58 us | 1,472.06 us | 382.82 | 178,628.00 KB |
|    Nodes2628_50PV (GS+NR) |  conv |  42,682.2 us |   270.09 us |   252.64 us | 106.59 |  51,041.47 KB |


## Connectivity :o:

Checks for graph connectivity:


Config:

```ini
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
```


|            Method | Mean         | Error       | StdDev      | Gen0  | Allocated  |
|-----------------: |-------------:|------------:|------------:|------:|-----------:|
|          Test_Ktr |     281.5 ns |     3.60 ns |     3.37 ns |  0.17 |      736 B |
|        Nodes4_1PV |     292.9 ns |     1.13 ns |     1.06 ns |  0.17 |      744 B |
|    Nodes4_1PV_ZIP |     292.3 ns |     4.16 ns |     3.47 ns |  0.17 |      744 B |
|     Nodes5_2Slack |     357.5 ns |     1.89 ns |     1.77 ns |  0.20 |      840 B |
|           IEEE_14 |     984.2 ns |     5.79 ns |     4.52 ns |  0.41 |    1,736 B |
|       Nodes15_3PV |     903.4 ns |    12.92 ns |    11.45 ns |  0.42 |    1,760 B |
|           IEEE_57 |   3,309.9 ns |    16.32 ns |    15.27 ns |  1.43 |    5,992 B |
|          IEEE_118 |   7,612.0 ns |    65.97 ns |    58.48 ns |  2.88 |   12,104 B |
|     Nodes197_36PV |  11,595.9 ns |    41.26 ns |    34.46 ns |  3.99 |   16,760 B |
|     Nodes300_27PV |  18,519.9 ns |   173.00 ns |   161.83 ns |  7.08 |   29,616 B |
|     Nodes398_35PV |  21,709.2 ns |   168.46 ns |   149.33 ns |  7.84 |   32,832 B |
| Nodes398_35PV_ZIP |  21,160.6 ns |    56.03 ns |    49.67 ns |  7.84 |   32,832 B |
|    Nodes874_143PV |  66,174.6 ns |   228.03 ns |   178.03 ns | 16.96 |   71,144 B |
|   Nodes1350_250PV | 119,540.1 ns |   786.76 ns |   735.94 ns | 31.25 |  130,936 B |
|    Nodes2628_50PV | 171,658.4 ns | 2,789.52 ns | 2,472.84 ns | 60.54 |  254,104 B |

