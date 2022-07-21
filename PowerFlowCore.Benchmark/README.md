# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

Last update: **21.07.2022**

Grid samples list to be tested include (from [PowerFlowCore.Samples](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Samples) project):

* [Test_Ktr](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Test_Ktr.cs)
* [Nodes4_1PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes4_1PV.cs)
* [Nodes4_1PV_ZIP](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes4_1PV_ZIP.cs)
* [IEEE_14](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/IEEE-14.cs)
* [Nodes15_3PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes15_3PV.cs)
* [IEEE_57](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/IEEE-57.cs)
* [IEEE_118](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/IEEE-118.cs)
* [Nodes197_36PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes197_36PV.cs)
* [Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)

In addition to the well-known **IEEE** grids, configuration is displayed in grid name. So, **Node300_27PV** means 300 nodes where 27 is PV typed (generators). **Test_Ktr** include 4 nodes.

## Environment :toilet:

```ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1826 (21H2)
AMD Ryzen 5 2600, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT
  DefaultJob : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT
```

## Create grid :hammer:

Performance tests on grid creation provide several repetitions **N**: 1, 10, 100 and 1000 times for each one.

|         Method |    N |            Mean |        Error |       StdDev |      Gen 0 |      Gen 1 |      Gen 2 |    Allocated |
|--------------- |-----:|----------------:|-------------:|-------------:|-----------:|-----------:|-----------:|-------------:|
|       Test_Ktr |    1 |         5.41 us |      0.10 us |      0.11 us |      1.503 |          - |          - |         6 KB |
|     Nodes4_1PV |    1 |         5.31 us |      0.07 us |      0.07 us |      1.457 |          - |          - |         6 KB |
| Nodes4_1PV_ZIP |    1 |         7.68 us |      0.09 us |      0.08 us |      2.029 |          - |          - |         8 KB |
|        IEEE_14 |    1 |        19.16 us |      0.26 us |      0.22 us |      4.364 |          - |          - |        18 KB |
|    Nodes15_3PV |    1 |        25.23 us |      0.49 us |      0.49 us |      4.974 |          - |          - |        20 KB |
|        IEEE_57 |    1 |       195.17 us |      1.91 us |      1.69 us |     33.691 |      0.244 |          - |       139 KB |
|       IEEE_118 |    1 |     1,003.16 us |     12.83 us |     12.00 us |    132.812 |    132.812 |    132.812 |       511 KB |
|  Nodes197_36PV |    1 |     2,391.49 us |     28.19 us |     26.37 us |    332.031 |    332.031 |    332.031 |     1,342 KB |
|  Nodes300_27PV |    1 |     4,676.80 us |     53.69 us |     50.22 us |    546.875 |    523.437 |    500.000 |     3,004 KB |
|
|       Test_Ktr |   10 |        54.16 us |      0.54 us |      0.51 us |     15.014 |          - |          - |        61 KB |
|     Nodes4_1PV |   10 |        54.10 us |      0.64 us |      0.56 us |     14.587 |          - |          - |        60 KB |
| Nodes4_1PV_ZIP |   10 |        79.05 us |      1.24 us |      1.16 us |     20.385 |          - |          - |        83 KB |
|        IEEE_14 |   10 |       200.81 us |      4.00 us |      5.61 us |     43.701 |          - |          - |       179 KB |
|    Nodes15_3PV |   10 |       255.39 us |      4.93 us |      5.06 us |     49.804 |          - |          - |       204 KB |
|        IEEE_57 |   10 |     2,062.83 us |     29.35 us |     26.02 us |    335.937 |      3.906 |          - |     1,387 KB |
|       IEEE_118 |   10 |     9,997.07 us |     88.19 us |     82.49 us |   1328.125 |   1328.125 |   1328.125 |     5,114 KB |
|  Nodes197_36PV |   10 |    23,662.92 us |    260.97 us |    244.11 us |   3312.500 |   3312.500 |   3312.500 |    13,417 KB |
|  Nodes300_27PV |   10 |    47,241.19 us |    455.49 us |    426.06 us |   5454.545 |   5363.636 |   5000.000 |    30,035 KB |
|
|       Test_Ktr |  100 |       551.46 us |      6.34 us |      5.93 us |    149.414 |          - |          - |       614 KB |
|     Nodes4_1PV |  100 |       533.85 us |      7.26 us |      6.44 us |    145.507 |          - |          - |       597 KB |
| Nodes4_1PV_ZIP |  100 |       778.73 us |      6.61 us |      6.19 us |    204.101 |          - |          - |       834 KB |
|        IEEE_14 |  100 |     1,964.62 us |     31.30 us |     29.28 us |    437.500 |          - |          - |     1,793 KB |
|    Nodes15_3PV |  100 |     2,466.08 us |     27.80 us |     26.01 us |    496.093 |          - |          - |     2,037 KB |
|        IEEE_57 |  100 |    20,333.62 us |    228.03 us |    190.41 us |   3375.000 |     31.250 |          - |    13,873 KB |
|       IEEE_118 |  100 |    99,674.40 us |  1,217.71 us |  1,139.05 us |  13200.000 |  13200.000 |  13200.000 |    51,139 KB |
|  Nodes197_36PV |  100 |   239,958.83 us |  2,933.41 us |  2,743.92 us |  33000.000 |  33000.000 |  33000.000 |   134,170 KB |
|  Nodes300_27PV |  100 |   461,186.65 us |  4,272.00 us |  3,787.02 us |  54000.000 |  51000.000 |  50000.000 |   300,351 KB |
|
|       Test_Ktr | 1000 |     5,450.49 us |     85.87 us |     80.32 us |   1500.000 |          - |          - |     6,141 KB |
|     Nodes4_1PV | 1000 |     5,327.32 us |     85.60 us |     71.48 us |   1460.937 |          - |          - |     5,969 KB |
| Nodes4_1PV_ZIP | 1000 |     7,786.63 us |    139.14 us |    136.65 us |   2039.062 |          - |          - |     8,345 KB |
|        IEEE_14 | 1000 |    19,511.35 us |    341.48 us |    302.71 us |   4375.000 |          - |          - |    17,932 KB |
|    Nodes15_3PV | 1000 |    24,643.91 us |    409.16 us |    362.71 us |   4968.750 |          - |          - |    20,369 KB |
|        IEEE_57 | 1000 |   195,592.98 us |  3,761.44 us |  3,862.72 us |  33666.666 |    333.333 |          - |   138,727 KB |
|       IEEE_118 | 1000 | 1,006,830.08 us | 11,902.95 us | 11,134.03 us | 133000.000 | 133000.000 | 133000.000 |   511,393 KB |
|  Nodes197_36PV | 1000 | 2,393,877.06 us | 17,859.67 us | 16,705.95 us | 333000.000 | 333000.000 | 333000.000 | 1,341,695 KB |
|  Nodes300_27PV | 1000 | 4,636,352.63 us | 16,864.25 us | 14,949.71 us | 548000.000 | 529000.000 | 500000.000 | 3,003,499 KB |
Notes:
-  **N**         : Value of the 'N' parameter
-  **Mean**      : Arithmetic mean of all measurements
-  **Error**     : Half of 99.9% confidence interval
-  **StdDev**    : Standard deviation of all measurements
-  **Gen 0**     : GC Generation 0 collects per 1000 operations
-  **Gen 1**     : GC Generation 1 collects per 1000 operations
-  **Gen 2**     : GC Generation 2 collects per 1000 operations
-  **Allocated** : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
-  **1 us**      : 1 Microsecond (0.000001 sec)

## Calculate grid :triangular_ruler:

Grid calculation provides several repetitions **N**: 1, 10, 100 and 1000 times for each one.

|         Method |    N |             Mean |           Error |          StdDev |        Gen 0 |       Gen 1 |       Gen 2 |     Allocated |
|--------------- |-----:|-----------------:|----------------:|----------------:|-------------:|------------:|------------:|--------------:|
|       Test_Ktr |    1 |         243.9 us |         4.72 us |         4.64 us |       35.644 |           - |           - |        146 KB |
|     Nodes4_1PV |    1 |         169.4 us |         1.52 us |         1.19 us |       25.390 |           - |           - |        104 KB |
| Nodes4_1PV_ZIP |    1 |         289.0 us |         3.58 us |         3.17 us |       41.015 |           - |           - |        168 KB |
|        IEEE_14 |    1 |         622.8 us |         2.81 us |         2.20 us |       81.054 |           - |           - |        326 KB |
|    Nodes15_3PV |    1 |       1,387.2 us |         7.69 us |         6.82 us |      164.062 |       1.953 |           - |        660 KB |
|        IEEE_57 |    1 |       7,971.9 us |        80.51 us |        71.37 us |      906.250 |     515.625 |     250.000 |      3,533 KB |
|       IEEE_118 |    1 |      28,734.9 us |       240.47 us |       224.94 us |     3000.000 |    1718.750 |    1437.500 |     11,553 KB |
|  Nodes197_36PV |    1 |     151,114.0 us |     2,143.14 us |     2,004.69 us |     9500.000 |    6250.000 |    5750.000 |     39,963 KB |
|  Nodes300_27PV |    1 |     498,302.9 us |     6,675.20 us |     6,243.99 us |    13000.000 |    5000.000 |    5000.000 |     95,145 KB |
|
|       Test_Ktr |   10 |       2,346.0 us |        29.09 us |        25.79 us |      359.375 |           - |           - |      1,460 KB |
|     Nodes4_1PV |   10 |       1,733.7 us |        34.42 us |        51.52 us |      253.906 |           - |           - |      1,031 KB |
| Nodes4_1PV_ZIP |   10 |       2,866.8 us |        43.42 us |        66.30 us |      414.062 |           - |           - |      1,680 KB |
|        IEEE_14 |   10 |       6,226.2 us |        37.77 us |        35.33 us |      804.687 |           - |           - |      3,266 KB |
|    Nodes15_3PV |   10 |      14,284.3 us |       284.07 us |       327.13 us |     1640.625 |           - |           - |      6,600 KB |
|        IEEE_57 |   10 |      79,824.5 us |       621.91 us |       551.31 us |     9000.000 |    5000.000 |    2428.571 |     35,335 KB |
|       IEEE_118 |   10 |     284,601.8 us |     1,285.06 us |     1,202.04 us |    28000.000 |   15000.000 |   13000.000 |    115,534 KB |
|  Nodes197_36PV |   10 |   1,533,581.7 us |    22,589.00 us |    21,129.77 us |    99000.000 |   69000.000 |   60000.000 |    400,315 KB |
|  Nodes300_27PV |   10 |   4,970,245.0 us |    35,217.52 us |    32,942.49 us |   135000.000 |   60000.000 |   51000.000 |    951,329 KB |
|
|       Test_Ktr |  100 |      24,350.4 us |       343.12 us |       320.95 us |     3538.461 |           - |           - |     14,580 KB |
|     Nodes4_1PV |  100 |      16,646.3 us |       173.76 us |       135.66 us |     2531.250 |           - |           - |     10,324 KB |
| Nodes4_1PV_ZIP |  100 |      28,086.6 us |       222.81 us |       186.05 us |     4125.000 |           - |           - |     16,816 KB |
|        IEEE_14 |  100 |      64,064.0 us |     1,232.02 us |     1,644.72 us |     8000.000 |     125.000 |           - |     32,649 KB |
|    Nodes15_3PV |  100 |     140,892.0 us |     2,766.96 us |     3,075.47 us |    16250.000 |           - |           - |     66,014 KB |
|        IEEE_57 |  100 |     792,667.1 us |     2,795.22 us |     2,614.65 us |    92000.000 |   48000.000 |   26000.000 |    353,337 KB |
|       IEEE_118 |  100 |   2,832,895.8 us |     5,834.87 us |     4,872.38 us |   308000.000 |  177000.000 |  149000.000 |  1,155,240 KB |
|  Nodes197_36PV |  100 |  15,428,726.3 us |    76,883.18 us |    68,154.94 us |   994000.000 |  694000.000 |  598000.000 |  4,004,934 KB |
|  Nodes300_27PV |  100 |  49,603,350.5 us |    63,002.60 us |    55,850.16 us |  1352000.000 |  605000.000 |  507000.000 |  9,512,682 KB |
|
|       Test_Ktr | 1000 |     248,220.2 us |     4,855.19 us |     4,985.92 us |    35000.000 |           - |           - |    145,670 KB |
|     Nodes4_1PV | 1000 |     166,965.7 us |     1,644.38 us |     2,195.20 us |    25000.000 |           - |           - |    103,142 KB |
| Nodes4_1PV_ZIP | 1000 |     287,177.3 us |     3,608.19 us |     3,013.00 us |    41000.000 |           - |           - |    167,770 KB |
|        IEEE_14 | 1000 |     628,241.0 us |     6,024.13 us |     5,634.97 us |    81000.000 |           - |           - |    326,558 KB |
|    Nodes15_3PV | 1000 |   1,427,075.9 us |    27,595.12 us |    27,102.10 us |   164000.000 |           - |           - |    660,636 KB |
|        IEEE_57 | 1000 |   8,025,992.2 us |    65,219.21 us |    54,461.00 us |   941000.000 |  507000.000 |  276000.000 |  3,533,063 KB |
|       IEEE_118 | 1000 |  29,207,210.0 us |    92,149.69 us |    81,688.30 us |  3088000.000 | 1796000.000 | 1498000.000 | 11,553,774 KB |
|  Nodes197_36PV | 1000 | 155,215,431.9 us | 1,496,599.92 us | 1,249,728.72 us |  9958000.000 | 6062000.000 | 5997000.000 | 40,044,965 KB |
|  Nodes300_27PV | 1000 | 496,777,406.7 us | 3,758,180.30 us | 3,331,528.95 us | 12857000.000 | 5201000.000 | 4402000.000 | 95,118,289 KB |

Notes:
-  **N**         : Value of the 'N' parameter
-  **Mean**      : Arithmetic mean of all measurements
-  **Error**     : Half of 99.9% confidence interval
-  **StdDev**    : Standard deviation of all measurements
-  **Gen 0**     : GC Generation 0 collects per 1000 operations
-  **Gen 1**     : GC Generation 1 collects per 1000 operations
-  **Gen 2**     : GC Generation 2 collects per 1000 operations
-  **Allocated** : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
-  **1 us**      : 1 Microsecond (0.000001 sec)

## Parallel calculations :fire:

### Sample grids in parallel

Set of all presented sample grids can be calculated in parallel. Repetitions **N**: 1, 10 and 100 times.

|           Method |   N |        Mean |     Error |    StdDev |   Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|----------------- |----:|------------:|----------:|----------:|--------:|-------:|-------:|----------:|
| All Sample Grids |   1 |    522.7 ms |  10.13 ms |  14.53 ms |   20000 |   7000 |   6000 |    148 MB |
| All Sample Grids |  10 |  5,303.9 ms |  26.41 ms |  23.41 ms |  231000 |  92000 |  83000 |  1,478 MB |
| All Sample Grids | 100 | 52,524.4 ms | 229.12 ms | 214.32 ms | 2260000 | 856000 | 780000 | 14,781 MB |

Notes:
-  **N**         : Value of the 'N' parameter
-  **Mean**      : Arithmetic mean of all measurements
-  **Error**     : Half of 99.9% confidence interval
-  **StdDev**    : Standard deviation of all measurements
-  **Gen 0**     : GC Generation 0 collects per 1000 operations
-  **Gen 1**     : GC Generation 1 collects per 1000 operations
-  **Gen 2**     : GC Generation 2 collects per 1000 operations
-  **Allocated** : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
-  **1 ms**      : 1 Millisecond (0.001 sec)

### Large model in parallel

If you work with a number of different modification of one large grid it is appropriate to apply parallel calculations. Next performance test presents a list of **100 grids** that contains **300 nodes grids** ([Nodes300_27PV](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore.Samples/SampleGrids/Nodes300_27PV.cs)) being calculated in parallel (**N=**) 1 and 10 times.

|              Method |  N |     Mean |   Error |  StdDev |    Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|-------------------- |---:|---------:|--------:|--------:|---------:|-------:|-------:|----------:|
| Nodes300_27PV x 100 |  1 |  14.71 s | 0.281 s | 0.335 s |  1012000 |  26000 |  16000 |      9 GB |
| Nodes300_27PV x 100 | 10 | 150.52 s | 1.314 s | 1.229 s | 10057000 | 262000 | 144000 |     91 GB |

Notes:
-  **N**         : Value of the 'N' parameter
-  **Mean**      : Arithmetic mean of all measurements
-  **Error**     : Half of 99.9% confidence interval
-  **StdDev**    : Standard deviation of all measurements
-  **Gen 0**     : GC Generation 0 collects per 1000 operations
-  **Gen 1**     : GC Generation 1 collects per 1000 operations
-  **Gen 2**     : GC Generation 2 collects per 1000 operations
-  **Allocated** : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
-  **1 s**       : 1 Second (1 sec)