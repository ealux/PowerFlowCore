# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **24.05.2023 - v.0.13.1** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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
|          Test_Ktr | 1 |     422.4 us |     4.24 us |     3.76 us |    323.78 KB |
|        Nodes4_1PV | 1 |     294.8 us |     2.22 us |     2.08 us |    226.49 KB |
|    Nodes4_1PV_ZIP | 1 |     255.8 us |     5.05 us |     6.91 us |    183.07 KB |
|     Nodes5_2Slack | 1 |     260.7 us |     2.51 us |     2.35 us |    203.02 KB |
|           IEEE_14 | 1 |     776.9 us |     4.34 us |     3.84 us |    580.96 KB |
|       Nodes15_3PV | 1 |   1,373.0 us |    20.74 us |    19.40 us |   1060.96 KB |
|           IEEE_57 | 1 |   2,991.8 us |     9.61 us |     8.99 us |   2719.81 KB |
|          IEEE_118 | 1 |  10,039.9 us |    79.00 us |    73.89 us |    7752.3 KB |
|     Nodes197_36PV | 1 |  22,446.4 us |   115.54 us |   108.07 us |  19371.28 KB |
|     Nodes300_27PV | 1 |  48,320.5 us |   507.83 us |   475.03 us |  41102.71 KB |
|     Nodes398_35PV | 1 |  70,086.5 us | 1,082.97 us | 1,013.01 us |  63564.71 KB |
| Nodes398_35PV_ZIP | 1 |  71,307.6 us |   914.26 us |   855.20 us |  63784.93 KB |
|    Nodes874_143PV | 1 | 531,092.9 us | 7,779.70 us | 6,896.50 us | 324037.43 KB |

Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|            Method | N |         Mean |       Error |      StdDev |    Allocated |
|------------------ |-- |-------------:|------------:|------------:|-------------:|
|          Test_Ktr | 1 |     353.7 us |     1.19 us |     1.11 us |    327.08 KB |
|        Nodes4_1PV | 1 |     239.1 us |     0.85 us |     0.79 us |    228.96 KB |
|    Nodes4_1PV_ZIP | 1 |     201.3 us |     0.40 us |     0.35 us |     185.1 KB |
|     Nodes5_2Slack | 1 |     218.6 us |     1.29 us |     1.21 us |    206.06 KB |
|           IEEE_14 | 1 |     559.5 us |     0.99 us |     0.92 us |    543.07 KB |
|       Nodes15_3PV | 1 |     965.5 us |     2.14 us |     2.00 us |    991.76 KB |
|           IEEE_57 | 1 |   2,946.5 us |    17.96 us |    15.92 us |   2672.64 KB |
|          IEEE_118 | 1 |  11,340.0 us |   220.31 us |   454.98 us |   7697.22 KB |
|     Nodes197_36PV | 1 |  24,540.6 us |   237.38 us |   222.04 us |   19329.5 KB |
|     Nodes300_27PV | 1 |  52,011.6 us |   856.60 us |   801.26 us |  41068.42 KB |
|     Nodes398_35PV | 1 |  74,109.1 us |   793.10 us |   703.06 us |  63532.19 KB |
| Nodes398_35PV_ZIP | 1 |  74,193.2 us | 1,314.34 us | 1,290.85 us |  63744.98 KB |
|    Nodes874_143PV | 1 | 515,306.8 us | 3,904.37 us | 3,652.15 us | 323954.91 KB |


## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|                           Method | N |     Mean |   Error |  StdDev |        Gen0 |      Gen1 |      Gen2 | Allocated |
|--------------------------------- |-- |---------:|--------:|--------:|------------:|----------:|----------:|----------:|
|                   AllSampleGrids | 1 | 593.1 ms | 9.57 ms | 8.96 ms |      131000 |      6000 |      1000 | 512.47 MB |

Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```

|                           Method | N |     Mean |   Error |  StdDev |        Gen0 |      Gen1 |      Gen2 | Allocated |
|--------------------------------- |-- |---------:|--------:|--------:|------------:|----------:|----------:|----------:|
|                   AllSampleGrids | 1 | 620.8 ms | 8.63 ms | 8.07 ms |       164000|      5000 |      1000 | 512.00 MB |

## Large models collection :zap:

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

Config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|                       Method |    N |    Mean |   Error |  StdDev |          Gen0 |        Gen1 |      Gen2 | Allocated |
|----------------------------- |:----:|--------:|--------:|--------:|--------------:|------------:|----------:|----------:|
|          CalculateSequential | 1000 | 49.82 s | 0.274 s | 0.256 s |      10458000 |      587000 |      1000 |   39.2 GB |
|  CalculateParallel_100_items |   10 | 26.45 s | 0.180 s | 0.159 s |       9677000 |      858000 |      7000 |  39.16 GB |
| CalculateParallel_1000_items |    1 | 26.95 s | 0.238 s | 0.211 s |       9627000 |      878000 |      9000 |  39.16 GB |

Another config:

```ini
BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.2604/21H2/November2021Update)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=7.0.302
  [Host]   : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
```


|                       Method |    N |    Mean |   Error |  StdDev |          Gen0 |        Gen1 |      Gen2 | Allocated |
|----------------------------- |:----:|--------:|--------:|--------:|--------------:|------------:|----------:|----------:|
|          CalculateSequential | 1000 | 54.38 s | 0.160 s | 0.150 s |      13147000 |      924000 |      3000 |  39.16 GB |
|  CalculateParallel_100_items |   10 | 26.62 s | 0.198 s | 0.185 s |      11427000 |      774000 |      3000 |   39.1 GB |
| CalculateParallel_1000_items |    1 | 26.67 s | 0.398 s | 0.372 s |      11430000 |      865000 |      3000 |  39.11 GB |

## Multiple solvers :dizzy:

Apply multiple solvers to calculate grid can provide different results within different model configuration.
Following tests describe:

* `NewtonRaphsonOnly`: use only Newton-Raphson solver;
* `GaussThenNewtonRaphson`: make 5 iteration with Gauss-Seidel solver at first then solve by Newton-Raphson.
	 

|                               Method | N |        Mean |       Error |      StdDev | Ratio |   Allocated |
|------------------------------------- |-- |------------:|------------:|------------:|------:|------------:|
|            NewtonRaphsonOnly_IEEE_14 | 1 |    802.4 us |     7.41 us |     6.19 us |  1.00 |   580.96 KB |
|       GaussThenNewtonRaphson_IEEE_14 | 1 |  1,343.3 us |    26.24 us |    35.91 us |  1.70 |   917.06 KB |
|           NewtonRaphsonOnly_IEEE_118 | 1 | 10,286.6 us |    44.07 us |    41.22 us | 12.81 |  7742.73 KB |
|      GaussThenNewtonRaphson_IEEE_118 | 1 | 18,560.3 us |   350.71 us |   328.05 us | 23.10 | 10387.72 KB |
|      NewtonRaphsonOnly_Nodes300_27PV | 1 | 49,285.3 us |   717.81 us |   636.32 us | 61.48 | 41118.64 KB |
| GaussThenNewtonRaphson_Nodes300_27PV | 1 | 74,563.9 us | 1,354.96 us | 1,267.43 us | 92.82 | 55861.42 KB |


## Connectivity :o:

Checks for graph connectivity:

|            Method | N |           Mean |         Error |        StdDev |   Gen0 | Allocated |
|------------------ |-- |---------------:|--------------:|--------------:|-------:|----------:|
|          Test_Ktr | 1 |       1.64  us |     0.012  us |     0.011  us | 0.39   |   1.61 KB |
|        Nodes4_1PV | 1 |       1.29  us |     0.006  us |     0.005  us | 0.35   |   1.46 KB |
|    Nodes4_1PV_ZIP | 1 |       1.33  us |     0.009  us |     0.008  us | 0.35   |   1.46 KB |
|     Nodes5_2Slack | 1 |       2.19  us |     0.012  us |     0.011  us | 0.44   |   1.83 KB |
|           IEEE_14 | 1 |      12.08  us |     0.078  us |     0.073  us | 0.83   |   3.44 KB |
|       Nodes15_3PV | 1 |      11.49  us |     0.046  us |     0.040  us | 0.83   |   3.48 KB |
|           IEEE_57 | 1 |     189.28  us |     0.398  us |     0.373  us | 2.68   |  11.26 KB |
|          IEEE_118 | 1 |   1,056.89  us |     2.723  us |     2.547  us | 3.90   |  21.88 KB |
|     Nodes197_36PV | 1 |   3,384.69  us |    12.156  us |    10.776  us | 7.81   |  34.36 KB |
|     Nodes300_27PV | 1 |   9,148.43  us |    11.015  us |    10.303  us |      - |  53.09 KB |
|     Nodes398_35PV | 1 |  16,934.68  us |    55.310  us |    51.737  us |      - |   66.5 KB |
| Nodes398_35PV_ZIP | 1 |  17,341.20  us |    16.776  us |    14.871  us |      - |   66.5 KB |
|    Nodes874_143PV | 1 | 194,786.44  us | 1,440.719  us | 1,277.160  us |      - | 160.52 KB |




