# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **26.06.2026 - v.0.14.2** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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
BenchmarkDotNet v0.15.8, Windows 11 (10.0.22631.5472/23H2/2023Update/SunValley3)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
Job=.NET 10.0  Runtime=.NET 10.0
```

| Method            | Mean        | Error    | StdDev   | Gen0   | Gen1   | Gen2   | Allocated   |
|-----------------: |------------:|---------:|---------:|-------:|-------:|-------:|------------:|
| Test_Ktr          |     2.40 us |  0.04 us |  0.04 us |   0.95 |   0.01 |      - |     8.77 KB |
| Nodes4_1PV        |     2.23 us |  0.03 us |  0.02 us |   0.96 |      - |      - |     8.93 KB |
| Nodes4_1PV_ZIP    |     6.67 us |  0.06 us |  0.06 us |   2.18 |   0.03 |      - |    20.15 KB |
| Nodes5_2Slack     |     2.87 us |  0.04 us |  0.03 us |   1.12 |   0.00 |      - |    10.38 KB |
| IEEE_14           |     6.01 us |  0.11 us |  0.10 us |   2.43 |   0.07 |      - |    22.38 KB |
| Nodes15_3PV       |     6.53 us |  0.12 us |  0.11 us |   2.53 |   0.08 |      - |    23.33 KB |
| IEEE_57           |    21.29 us |  0.39 us |  0.37 us |   8.30 |   1.00 |      - |    76.41 KB |
| IEEE_118          |    51.49 us |  0.52 us |  0.43 us |  19.65 |   5.24 |      - |   181.17 KB |
| Nodes197_36PV     |    82.79 us |  1.57 us |  1.68 us |  26.97 |   9.64 |      - |   248.81 KB |
| Nodes300_27PV     |   146.33 us |  2.92 us |  3.79 us |  41.99 |  19.77 |      - |   387.18 KB |
| Nodes398_35PV     |   183.46 us |  2.34 us |  2.08 us |  51.02 |  27.34 |      - |   470.31 KB |
| Nodes398_35PV_ZIP |   190.84 us |  3.75 us |  4.16 us |  51.75 |  27.58 |      - |   477.87 KB |
| Nodes874_143PV    |   767.69 us | 15.09 us | 16.77 us | 125.00 | 105.46 |      - | 1,153.53 KB |
| Nodes1350_250PV   | 1,340.27 us | 22.37 us | 20.93 us | 210.93 | 207.03 | 105.46 | 2,088.74 KB |
| Nodes2628_50PV    | 1,892.70 us | 15.52 us | 13.75 us | 359.37 | 355.46 |  89.84 | 3,126.11 KB |

## Calculate grid :triangular_ruler:

Grid calculations. 

Config:
```ini
BenchmarkDotNet v0.15.8, Windows 11 (10.0.22631.5472/23H2/2023Update/SunValley3)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
Job=.NET 10.0  Runtime=.NET 10.0
```


| Method                | Mean         | Error      | StdDev     | Gen0 | Gen1 | Gen2 | Allocated    |
|----------------------:|-------------:|-----------:|-----------:|-----:|-----:|-----:|-------------:|
| Test_Ktr              |     52.57 us |   1.025 us |   2.293 us |    9 |    - |    - |     91.88 KB |
| Nodes4_1PV            |     37.58 us |   0.677 us |   1.368 us |    7 |    - |    - |     69.80 KB |
| Nodes4_1PV_ZIP        |     65.10 us |   1.290 us |   1.485 us |   14 |    - |    - |    132.43 KB |
| Nodes5_2Slack         |     44.64 us |   1.014 us |   2.959 us |    9 |    - |    - |     87.63 KB |
| IEEE_14               |    243.21 us |   5.780 us |  16.862 us |   54 |    1 |    - |    517.62 KB |
| Nodes15_3PV           |    336.40 us |   7.017 us |  20.468 us |   78 |    5 |    - |    732.78 KB |
| IEEE_57               |    634.54 us |  12.654 us |  28.302 us |  128 |   23 |    - |  1,208.75 KB |
| IEEE_118              |  1,322.36 us |  25.162 us |  22.305 us |  203 |   54 |    - |  1,858.94 KB |
| Nodes197_36PV         |  3,056.22 us |  60.744 us |  72.312 us |  460 |  164 |    - |  4,224.07 KB |
| Nodes300_27PV         |  5,529.00 us | 109.769 us | 160.898 us |  984 |  484 |  484 |  7,209.58 KB |
| Nodes398_35PV         |  5,974.88 us |  59.198 us |  52.478 us | 1187 |  593 |  593 |  8,585.77 KB |
| Nodes398_35PV_ZIP     | 10,104.71 us |  88.729 us |  78.656 us | 1500 | 1000 | 1000 | 14,168.49 KB |
| Nodes874_143PV        | 17,713.18 us |  97.844 us |  91.523 us | 3843 | 3468 | 3437 | 23,092.35 KB |
| Nodes1350_250PV       | 33,568.08 us | 669.433 us | 822.124 us | 8000 | 7625 | 6687 | 42,997.68 KB |
| Nodes2628_50PV (GS+NR)| 27,283.68 us | 274.853 us | 243.650 us | 7843 | 6281 | 5812 | 50,598.01 KB |


## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

Config:

```ini
BenchmarkDotNet v0.15.8, Windows 11 (10.0.22631.5472/23H2/2023Update/SunValley3)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
Job=.NET 10.0  Runtime=.NET 10.0
```

| Method                     | Mean     | Error   | StdDev  | Ratio | Gen0  | Gen1  | Gen2  | Allocated |
|--------------------------: |---------:|--------:|--------:|------:|------:|------:|------:|----------:|
| AllSampleGrids (seq)       | 192.6 ms | 3.41 ms | 4.67 ms |  1.00 | 46000 | 38000 | 35500 | 277.09 MB |
| AllSampleGrids (par)       | 132.9 ms | 2.64 ms | 4.82 ms |  0.69 | 36333 | 30000 | 24000 | 277.15 MB |
| AllSampleGrids (par async) | 134.1 ms | 2.68 ms | 3.84 ms |  0.70 | 35000 | 29666 | 22333 | 277.22 MB |


## Large models collection :zap:

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

Config:

```ini
BenchmarkDotNet v0.15.8, Windows 11 (10.0.22631.5472/23H2/2023Update/SunValley3)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
Job=.NET 10.0  Runtime=.NET 10.0
```

|                          Method |    N | Mean    | Error   | StdDev  | Gen0   | Gen1   | Gen2  | Allocated |
|:------------------------------- |:---: |--------:|--------:|--------:|-------:|-------:|------:|----------:|
| Nodes300_27PV (seq) x1          | 1000 | 5.420 s | 0.022 s | 0.018 s | 624000 | 229000 | 13000 |   6.87 GB |
| Nodes300_27PV (par) x10         |  100 | 1.752 s | 0.016 s | 0.015 s | 665000 | 321000 | 12000 |   6.87 GB |
| Nodes300_27PV (par) x100        |   10 | 1.700 s | 0.015 s | 0.014 s | 665000 | 370000 | 12000 |   6.86 GB |
| Nodes300_27PV (par) x1000       |    1 | 1.679 s | 0.015 s | 0.013 s | 666000 | 394000 | 12000 |   6.86 GB |
| Nodes300_27PV (par async) x10   |  100 | 1.768 s | 0.027 s | 0.024 s | 666000 | 314000 | 12000 |   6.87 GB |
| Nodes300_27PV (par async) x100  |   10 | 1.711 s | 0.012 s | 0.009 s | 664000 | 361000 | 11000 |   6.86 GB |
| Nodes300_27PV (par async) x1000 |    1 | 1.756 s | 0.033 s | 0.029 s | 663000 | 387000 | 10000 |   6.86 GB |


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
BenchmarkDotNet v0.15.8, Windows 11 (10.0.22631.5472/23H2/2023Update/SunValley3)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
Job=.NET 10.0  Runtime=.NET 10.0
```

|                    Method | State | Mean         | Error       | StdDev      | Ratio  | Allocated     |
|:------------------------- |:----: |-------------:|------------:|------------:|-------:|--------------:|
|              IEEE_14 (NR) |  conv |     251.4 us |     4.97 us |     9.46 us |   1.00 |     517.62 KB |
|           IEEE_14 (GS+NR) |  conv |     135.8 us |     2.69 us |     5.36 us |   0.54 |     262.33 KB |
|             IEEE_118 (NR) |  conv |   1,328.9 us |    19.24 us |    18.00 us |   5.29 |   1,858.91 KB |
|          IEEE_118 (GS+NR) |  conv |   1,312.0 us |    10.51 us |     9.83 us |   5.23 |   1,696.95 KB |
|        Nodes300_27PV (NR) |  conv |   5,507.4 us |   108.87 us |   234.36 us |  21.93 |   7,209.57 KB |
|     Nodes300_27PV (GS+NR) |  conv |   9,209.1 us |   123.81 us |   115.81 us |  36.67 |  12,088.15 KB |
|      Nodes1350_250PV (NR) |  conv |  33,441.0 us |   665.11 us | 1,015.69 us | 133.18 |  43,032.57 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  24,286.0 us |   239.16 us |   212.01 us |  96.72 |  30,043.87 KB |
|       Nodes2628_50PV (NR) |  div  | 108,305.4 us | 2,067.52 us | 2,212.22 us | 431.31 | 178,650.53 KB |
|    Nodes2628_50PV (GS+NR) |  conv |  27,987.3 us |   194.33 us |   162.27 us | 111.46 |  51,016.73 KB |


## Connectivity :o:

Checks for graph connectivity:


Config:

```ini
BenchmarkDotNet v0.15.8, Windows 11 (10.0.22631.5472/23H2/2023Update/SunValley3)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.201
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
Job=.NET 10.0  Runtime=.NET 10.0
```

|            Method | Mean        | Error       | StdDev      | Gen0  | Gen1| Allocated |
|-----------------: |------------:|------------:|------------:|------:|----:|----------:|
|          Test_Ktr |    121.1 ns |     2.42 ns |     3.77 ns |  0.07 |   - |     736 B |
|        Nodes4_1PV |    121.6 ns |     2.46 ns |     2.18 ns |  0.07 |   - |     744 B |
|    Nodes4_1PV_ZIP |    123.1 ns |     2.47 ns |     3.70 ns |  0.07 |   - |     744 B |
|     Nodes5_2Slack |    146.6 ns |     2.88 ns |     3.31 ns |  0.08 |   - |     840 B |
|           IEEE_14 |    422.9 ns |     6.13 ns |     6.02 ns |  0.18 | 0.00|   1,736 B |
|       Nodes15_3PV |    409.0 ns |     5.62 ns |     5.26 ns |  0.18 | 0.00|   1,760 B |
|           IEEE_57 |  1,505.8 ns |    27.86 ns |    38.14 ns |  0.63 | 0.00|   5,992 B |
|          IEEE_118 |  3,028.7 ns |    58.63 ns |    57.59 ns |  1.28 | 0.02|  12,104 B |
|     Nodes197_36PV |  4,499.8 ns |    86.11 ns |   105.76 ns |  1.77 | 0.03|  16,760 B |
|     Nodes300_27PV |  6,998.6 ns |   133.96 ns |   125.31 ns |  3.14 | 0.12|  29,616 B |
|     Nodes398_35PV |  8,244.0 ns |   156.64 ns |   186.47 ns |  3.47 | 0.13|  32,832 B |
| Nodes398_35PV_ZIP |  8,215.7 ns |   157.09 ns |   187.00 ns |  3.47 | 0.13|  32,832 B |
|    Nodes874_143PV | 20,207.1 ns |   179.50 ns |   159.12 ns |  7.53 | 0.61|  71,144 B |
|   Nodes1350_250PV | 46,384.2 ns |   826.07 ns |   772.71 ns | 13.85 | 2.25| 130,936 B |
|    Nodes2628_50PV | 65,704.2 ns | 1,312.90 ns | 1,562.91 ns | 26.85 | 5.61| 254,104 B |

