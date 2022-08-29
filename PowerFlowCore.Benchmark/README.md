# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **27.08.2022**</u>

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

In addition to the well-known **IEEE** grids, configuration is displayed in grid name. So, **Node300_27PV** means 300 nodes where 27 is PV typed (generators). **Test_Ktr** include 4 nodes.

Tables legend:

-  **N**         : Value of the 'N' parameter
-  **Mean**      : Arithmetic mean of all measurements
-  **Error**     : Half of 99.9% confidence interval
-  **StdDev**    : Standard deviation of all measurements
-  **Ratio**     : Mean of the ratio distribution ([Current]/[Baseline = 1.00])
-  **Gen 0**     : GC Generation 0 collects per 1000 operations
-  **Gen 1**     : GC Generation 1 collects per 1000 operations
-  **Gen 2**     : GC Generation 2 collects per 1000 operations
-  **Allocated** : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
-  **1 us**      : 1 Microsecond (0.000001 sec)
-  **1 ms**      : 1 Millisecond (0.001 sec)
-  **1 s**       : 1 Second (1 sec)

## Environment ⌨️

```ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.400
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
```

## Create grid :hammer:

Performance tests on grid creation:

|            Method | N |         Mean |      Error |     StdDev |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|-----------------: |-- |-------------:|-----------:|-----------:|----------:|---------:|---------:|----------:|
|          Test_Ktr | 1 |     10.06 us |   0.194 us |   0.190 us |    2.0447 |        - |        - |      8 KB |
|        Nodes4_1PV | 1 |     10.22 us |   0.147 us |   0.138 us |    2.0142 |        - |        - |      8 KB |
|    Nodes4_1PV_ZIP | 1 |     50.61 us |   0.916 us |   0.857 us |    4.8218 |        - |        - |     20 KB |
|     Nodes5_2Slack | 1 |     11.20 us |   0.123 us |   0.109 us |    2.3651 |        - |        - |     10 KB |
|           IEEE_14 | 1 |     29.67 us |   0.584 us |   0.547 us |    6.0425 |        - |        - |     25 KB |
|       Nodes15_3PV | 1 |     36.07 us |   0.699 us |   0.620 us |    6.7749 |        - |        - |     28 KB |
|           IEEE_57 | 1 |    244.49 us |   4.850 us |   5.957 us |   49.5605 |   0.4883 |        - |    199 KB |
|          IEEE_118 | 1 |  1,064.88 us |   3.164 us |   2.642 us |  140.6250 |  66.4063 |  66.4063 |    746 KB |
|     Nodes197_36PV | 1 |  2,559.40 us |  27.605 us |  24.471 us |  496.0938 | 312.5000 | 164.0625 |  1,975 KB |
|     Nodes300_27PV | 1 |  9,973.68 us | 194.260 us | 181.711 us | 1515.6250 | 656.2500 | 328.1250 |  4,466 KB |
|     Nodes398_35PV | 1 | 15,261.51 us | 283.018 us | 290.638 us | 2078.1250 | 859.3750 | 500.0000 |  7,716 KB |
| Nodes398_35PV_ZIP | 1 | 15,288.03 us | 302.898 us | 583.581 us | 2093.7500 | 843.7500 | 500.0000 |  7,724 KB |



## Calculate grid :triangular_ruler:

Grid calculations. Config:

```ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.400
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
```


|            Method | N |           Mean |        Error |       StdDev |      Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|-----------------: |-- |---------------:|-------------:|-------------:|-----------:|----------:|----------:|-----------:|
|          Test_Ktr | 1 |       436.0 us |      8.63 us |     12.64 us |    70.3125 |         - |         - |     286 KB |
|        Nodes4_1PV | 1 |       307.0 us |      4.43 us |      4.14 us |    52.2461 |         - |         - |     213 KB |
|    Nodes4_1PV_ZIP | 1 |       539.8 us |      9.61 us |      8.99 us |    88.8672 |         - |         - |     363 KB |
|     Nodes5_2Slack | 1 |       262.3 us |      4.36 us |      3.40 us |    42.9688 |         - |         - |     174 KB |
|           IEEE_14 | 1 |     1,122.7 us |     11.01 us |     10.30 us |   152.3438 |         - |         - |     613 KB |
|       Nodes15_3PV | 1 |     2,648.0 us |     22.24 us |     20.80 us |   328.1250 |         - |         - |   1,330 KB |
|           IEEE_57 | 1 |    11,946.8 us |     30.30 us |     25.30 us |  1156.2500 |  312.5000 |   62.5000 |   5,289 KB |
|          IEEE_118 | 1 |    35,968.6 us |    719.04 us |    984.22 us |  3000.0000 |  733.3333 |  400.0000 |  15,201 KB |
|     Nodes197_36PV | 1 |   133,326.7 us |  1,073.79 us |    951.89 us |  8750.0000 | 2250.0000 | 1750.0000 |  49,328 KB |
|     Nodes300_27PV | 1 |   395,303.1 us |  4,090.63 us |  3,826.38 us | 21000.0000 | 3000.0000 | 3000.0000 | 116,157 KB |
|     Nodes398_35PV | 1 |   832,854.6 us | 13,297.94 us | 11,788.28 us | 27000.0000 | 4000.0000 | 3000.0000 | 193,144 KB |
| Nodes398_35PV_ZIP | 1 | 1,162,863.7 us |  6,443.30 us |  6,027.07 us | 36000.0000 | 5000.0000 | 4000.0000 | 267,824 KB |


Another (average) config:
```ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1766 (21H2)
Intel Core i5-4460 CPU 3.20GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.400
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
```

|            Method | N |           Mean |        Error |       StdDev |      Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|------------------ |-- |---------------:|-------------:|-------------:|-----------:|----------:|----------:|-----------:|
|          Test_Ktr | 1 |       367.7 us |      4.31 us |      3.82 us |    98.6328 |         - |         - |     299 KB |
|        Nodes4_1PV | 1 |       273.7 us |      3.57 us |      3.34 us |    73.2422 |         - |         - |     222 KB |
|    Nodes4_1PV_ZIP | 1 |       474.7 us |      6.59 us |      7.05 us |   124.0234 |         - |         - |     379 KB |
|     Nodes5_2Slack | 1 |       222.2 us |      2.14 us |      2.00 us |    59.5703 |         - |         - |     181 KB |
|           IEEE_14 | 1 |       769.9 us |     10.99 us |     10.28 us |   181.6406 |         - |         - |     551 KB |
|       Nodes15_3PV | 1 |     1,746.1 us |      7.47 us |      6.99 us |   382.8125 |         - |         - |   1,162 KB |
|           IEEE_57 | 1 |     9,759.4 us |    153.27 us |    127.99 us |  1296.8750 |  281.2500 |   62.5000 |   4,725 KB |
|          IEEE_118 | 1 |    35,634.7 us |    711.82 us |    847.37 us |  3333.3333 |  666.6667 |  400.0000 |  13,970 KB |
|     Nodes197_36PV | 1 |   175,721.7 us |  2,840.04 us |  2,789.30 us | 10000.0000 | 2333.3333 | 1666.6667 |  46,094 KB |
|     Nodes300_27PV | 1 |   589,954.4 us | 11,088.20 us | 10,371.91 us | 26000.0000 | 4000.0000 | 4000.0000 | 110,229 KB |
|     Nodes398_35PV | 1 | 1,256,082.4 us | 12,176.19 us | 10,793.87 us | 33000.0000 | 3000.0000 | 3000.0000 | 185,338 KB |
| Nodes398_35PV_ZIP | 1 | 1,839,526.3 us | 23,782.60 us | 22,246.26 us | 43000.0000 | 4000.0000 | 4000.0000 | 256,140 KB |


## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel:

|         Method | N |    Mean |    Error |   StdDev |      Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|--------------- |-- |--------:|---------:|---------:|-----------:|----------:|----------:|----------:|
| AllSampleGrids | 1 | 1.982 s | 0.0314 s | 0.0262 s | 92000.0000 | 9000.0000 | 8000.0000 |    623 MB |

### Large model in parallel

If you work with a number of different modification of one large grid it is appropriate to apply parallel calculations. Next performance test presents a list of **100 grids** that contains **300 nodes grids** ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) being calculated in parallel (**N=**) 1 and 10 times.

|              Method |  N |     Mean |   Error |  StdDev |    Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|-------------------- |---:|---------:|--------:|--------:|---------:|-------:|-------:|----------:|
| Nodes300_27PV x 100 |  1 |  14.71 s | 0.281 s | 0.335 s |  1012000 |  26000 |  16000 |      9 GB |
| Nodes300_27PV x 100 | 10 | 150.52 s | 1.314 s | 1.229 s | 10057000 | 262000 | 144000 |     91 GB |

## Large models collection

Different ways to calculate **1000** large models items ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) is presented here. Make choices according to your hardware or this benchmark. 

|                         Method |    N |     Mean |   Error |  StdDev |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------------------------------- |:----:|---------:|--------:|--------:|---------:|--------:|--------:|----------:|
| Nodes300_27PV (sequental) x1   | 1000 | 496.77 s | 3.758 s | 3.331 s | 12857000 | 5201000 | 4402000 |     91 GB |
| Nodes300_27PV (parallel) x100  |   10 | 150.52 s | 1.314 s | 1.229 s | 10057000 |  262000 |  144000 |     91 GB |
| Nodes300_27PV (parallel) x1000 |    1 | 172.30 s | 3.380 s | 4.740 s |  9907000 |  100000 |   17000 |     91 GB |

## Multiple solvers

Apply multiple solvers to calculate grid can provide different results within different model configuration.
Following tests describe:

* `NewtonRaphsonOnly`: use only Newton-Raphson solver;
* `GaussThenNewtonRaphson`: make 5 iteration with Gauss-Seidel solver at first then solve by Newton-Raphson.

|                               Method |  N |           Mean |       Error |      StdDev |  Ratio |
|-------------------------------------:|:--:|---------------:|------------:|------------:|-------:|
|            NewtonRaphsonOnly IEEE_14 |  1 |       614.5 us |      2.9 us |      2.7 us |   1.00 |
|       GaussThenNewtonRaphson IEEE_14 |  1 |       826.3 us |      9.2 us |      7.7 us |   1.35 |
|           NewtonRaphsonOnly IEEE_118 |  1 |    28,249.6 us |     78.9 us |     70.0 us |  46.00 |
|      GaussThenNewtonRaphson IEEE_118 |  1 |    35,226.4 us |     77.3 us |     64.5 us |  57.36 |
|      NewtonRaphsonOnly Nodes300_27PV |  1 |   514,392.7 us |  9,573.3 us | 10,640.7 us | 834.11 |
| GaussThenNewtonRaphson Nodes300_27PV |  1 |   445,377.1 us |  8,420.6 us |  7,031.6 us | 725.21 |
|                                      |    |                |             |             |        |
|            NewtonRaphsonOnly IEEE_14 | 10 |     6,222.5 us |     18.3 us |     16.2 us |   1.00 |
|       GaussThenNewtonRaphson IEEE_14 | 10 |     8,325.2 us |     67.4 us |     59.7 us |   1.34 |
|           NewtonRaphsonOnly IEEE_118 | 10 |   287,576.9 us |  5,324.9 us |  4,720.3 us |  46.22 |
|      GaussThenNewtonRaphson IEEE_118 | 10 |   357,267.8 us |  3,067.5 us |  2,719.3 us |  57.42 |
|      NewtonRaphsonOnly Nodes300_27PV | 10 | 4,954,212.4 us | 43,570.4 us | 38,624.1 us | 796.18 |
| GaussThenNewtonRaphson Nodes300_27PV | 10 | 4,433,039.3 us | 63,913.2 us | 59,784.4 us | 712.03 |

## Connectivity

Checks for graph connectivity:

|         Method |   N |         Mean |      Error |     StdDev |  Gen 0 | Allocated |
|--------------- |----:|-------------:|-----------:|-----------:|-------:|----------:|
|       Test_Ktr |   1 |       2.4 us |     0.0 us |     0.0 us |    0.5 |      2 KB |
|     Nodes4_1PV |   1 |       2.1 us |     0.0 us |     0.0 us |    0.5 |      2 KB |
| Nodes4_1PV_ZIP |   1 |       2.1 us |     0.0 us |     0.0 us |    0.5 |      2 KB |
|        IEEE_14 |   1 |      11.1 us |     0.0 us |     0.0 us |    0.9 |      4 KB |
|    Nodes15_3PV |   1 |      10.9 us |     0.0 us |     0.0 us |    1.0 |      4 KB |
|        IEEE_57 |   1 |     158.1 us |     2.0 us |     1.9 us |    2.6 |     12 KB |
|       IEEE_118 |   1 |     927.5 us |     2.6 us |     2.1 us |    4.8 |     23 KB |
|  Nodes197_36PV |   1 |   3,063.2 us |    17.8 us |    14.9 us |    7.8 |     35 KB |
|  Nodes300_27PV |   1 |   8,307.2 us |     4.4 us |     3.7 us |      - |     54 KB |




