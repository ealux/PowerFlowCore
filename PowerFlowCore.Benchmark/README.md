# PowerFlowCore.Benchmark

Benchmark of [PowerFlowCore](https://github.com/ealux/PowerFlowCore) library performance is presented.
Performance tests include grids creation and calculations with default solver ([Newton-Raphson](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs)).

<u>Last update: **19.06.2023 - v.0.13.5** (previous results in [***_archive***](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark/_archive) folder)</u>

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


|            Method |        Mean |     Error |    StdDev |   Gen0 |   Gen1 |   Gen2 |   Allocated |
|-----------------: |------------:|----------:|----------:|-------:|-------:|-------:|------------:|
|          Test_Ktr |    17.00 us |  0.279 us |  0.352 us |   3.08 |      - |      - |    12.55 KB |
|        Nodes4_1PV |    16.30 us |  0.264 us |  0.247 us |   3.11 |      - |      - |    12.72 KB |
|    Nodes4_1PV_ZIP |    60.50 us |  1.103 us |  1.988 us |   5.85 |      - |      - |    24.32 KB |
|     Nodes5_2Slack |    19.48 us |  0.181 us |  0.151 us |   3.47 |      - |      - |    14.18 KB |
|           IEEE_14 |    38.25 us |  0.255 us |  0.226 us |   7.20 |      - |      - |    29.46 KB |
|       Nodes15_3PV |    39.50 us |  0.715 us |  1.048 us |   7.01 |      - |      - |    28.77 KB |
|           IEEE_57 |   104.05 us |  1.616 us |  1.511 us |  20.99 |      - |      - |     85.4 KB |
|          IEEE_118 |   203.89 us |  1.297 us |  1.150 us |  42.48 |      - |      - |    172.2 KB |
|     Nodes197_36PV |   313.24 us |  6.007 us |  6.676 us |  69.33 |   2.92 |      - |   281.44 KB |
|     Nodes300_27PV |   464.97 us |  3.811 us |  3.565 us |  97.16 |  28.80 |      - |   427.94 KB |
|     Nodes398_35PV |   545.93 us |  4.763 us |  4.222 us | 113.28 |  26.36 |      - |   497.24 KB |
| Nodes398_35PV_ZIP |   565.02 us |  4.471 us |  3.734 us | 111.32 |  31.25 |      - |   505.07 KB |
|    Nodes874_143PV | 1,615.83 us | 18.824 us | 17.608 us | 216.79 | 123.04 |  29.29 | 1,282.44 KB |
|   Nodes1350_250PV | 2,712.12 us | 38.850 us | 36.340 us | 328.12 | 148.43 |  66.40 | 2,011.85 KB |
|    Nodes2628_50PV | 4,203.48 us | 48.635 us | 43.114 us | 531.25 | 210.93 | 132.81 | 3,202.55 KB |


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


|                    Method |        Mean |       Error |      StdDev |    Allocated |
|-------------------------: |------------:|------------:|------------:|-------------:|
|                  Test_Ktr |    360.5 us |     7.14 us |     9.29 us |    268.97 KB |
|                Nodes4_1PV |    255.9 us |     3.90 us |     3.65 us |    190.07 KB |
|            Nodes4_1PV_ZIP |    503.2 us |     9.07 us |    14.90 us |    365.85 KB |
|             Nodes5_2Slack |    266.5 us |     4.48 us |     3.97 us |    215.95 KB |
|                   IEEE_14 |  1,341.4 us |    10.82 us |     9.60 us |  1,040.67 KB |
|               Nodes15_3PV |  1,906.5 us |    28.05 us |    23.43 us |  1,502.18 KB |
|                   IEEE_57 |  1,790.0 us |    23.59 us |    20.91 us |  1,891.31 KB |
|                  IEEE_118 |  2,725.3 us |    51.92 us |    61.81 us |  2,972.43 KB |
|             Nodes197_36PV |  5,847.0 us |   116.84 us |   171.27 us |  6,851.94 KB |
|             Nodes300_27PV |  9,581.7 us |    98.89 us |    87.66 us | 11,416.52 KB |
|             Nodes398_35PV | 11,060.0 us |   135.02 us |   112.75 us | 13,848.84 KB |
|         Nodes398_35PV_ZIP | 19,478.8 us |   378.25 us |   435.59 us | 22,899.30 KB |
|            Nodes874_143PV | 30,772.0 us |   606.14 us |   595.31 us | 36,322.64 KB |
|           Nodes1350_250PV | 66,593.6 us | 1,324.72 us | 1,813.29 us | 65,393.64 KB |
|    Nodes2628_50PV (GS+NR) | 58,275.6 us |   609.74 us |   509.16 us | 78,732.37 KB |


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
| AllSampleGrids | 285.7 ms | 5.69 ms | 8.51 ms | 74500 | 47500 | 24000 | 444.20 MB |


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

|                    Method |    N |    Mean |   Error |  StdDev |    Gen0 |   Gen1 |  Gen2 | Allocated |
|:------------------------- |:---: |--------:|--------:|--------:|--------:|-------:|------:|----------:|
| Nodes300_27PV (seq) x1    | 1000 | 8.835 s | 0.077 s | 0.064 s | 1996000 | 335000 | 13000 |  10.89 GB |
| Nodes300_27PV (par) x10   |  100 | 5.168 s | 0.060 s | 0.053 s | 1885000 | 589000 | 12000 |  10.86 GB |
| Nodes300_27PV (par) x100  |   10 | 5.296 s | 0.055 s | 0.049 s | 1887000 | 521000 | 12000 |  10.86 GB |
| Nodes300_27PV (par) x1000 |    1 | 5.216 s | 0.088 s | 0.082 s | 1888000 | 524000 | 12000 |  10.86 GB |


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

|                    Method | State |         Mean |       Error |      StdDev |  Ratio |    Allocated |
|:------------------------- |:----: |-------------:|------------:|------------:|-------:|-------------:|
|              IEEE_14 (NR) |  conv |   1,369.8 us |     8.93 us |     7.91 us |   1.00 |  1,040.28 KB |
|           IEEE_14 (GS+NR) |  conv |     901.4 us |     8.94 us |     8.36 us |   0.66 |    597.85 KB |
|             IEEE_118 (NR) |  conv |   2,782.4 us |    53.97 us |    55.42 us |   2.03 |  2,971.68 KB |
|          IEEE_118 (GS+NR) |  conv |   3,074.1 us |    26.22 us |    21.89 us |   2.24 |  2,685.90 KB |
|        Nodes300_27PV (NR) |  conv |   9,534.2 us |    51.31 us |    42.84 us |   6.96 | 11,417.01 KB |
|     Nodes300_27PV (GS+NR) |  conv |  16,408.0 us |   102.26 us |    90.65 us |  11.98 | 19,240.25 KB |
|      Nodes1350_250PV (NR) |  conv |  60,287.8 us | 1,057.89 us | 1,615.52 us |  44.27 | 65,356.73 KB |
|   Nodes1350_250PV (GS+NR) |  conv |  43,200.4 us |   845.80 us | 1,341.53 us |  31.53 | 44,078.49 KB |
|       Nodes2628_50PV (NR) |  div  | 216,482.0 us | 4,245.20 us | 4,169.36 us | 158.08 |289,919.24 KB |
|    Nodes2628_50PV (GS+NR) |  conv |  60,774.8 us |   762.57 us |   676.00 us |  44.37 | 79,181.52 KB |


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
|          Test_Ktr |     316.7 ns |     5.71 ns |     5.34 ns |  0.17 |     - |      736 B |
|        Nodes4_1PV |     323.7 ns |     1.47 ns |     1.23 ns |  0.17 |     - |      744 B |
|    Nodes4_1PV_ZIP |     333.4 ns |     3.18 ns |     2.97 ns |  0.17 |     - |      744 B |
|     Nodes5_2Slack |     400.4 ns |     1.00 ns |     0.84 ns |  0.20 |     - |      840 B |
|           IEEE_14 |   1,108.5 ns |     2.26 ns |     2.00 ns |  0.41 |     - |    1,736 B |
|       Nodes15_3PV |   1,042.6 ns |     1.88 ns |     1.76 ns |  0.41 |     - |    1,760 B |
|           IEEE_57 |   4,110.1 ns |     3.90 ns |     3.05 ns |  1.42 |     - |    5,992 B |
|          IEEE_118 |   9,877.9 ns |    79.16 ns |    70.17 ns |  2.88 |     - |   12,104 B |
|     Nodes197_36PV |  14,528.5 ns |   290.16 ns |   515.75 ns |  3.99 |     - |   16,760 B |
|     Nodes300_27PV |  24,159.9 ns |   443.12 ns |   392.82 ns |  7.04 |     - |   29,616 B |
|     Nodes398_35PV |  25,263.5 ns |   493.19 ns |   461.33 ns |  7.84 |     - |   32,832 B |
| Nodes398_35PV_ZIP |  25,755.2 ns |   498.55 ns |   593.49 ns |  7.84 |     - |   32,832 B |
|    Nodes874_143PV |  87,251.8 ns | 1,698.39 ns | 2,837.63 ns | 16.96 |     - |   71,144 B |
|   Nodes1350_250PV | 136,639.9 ns | 2,726.60 ns | 3,348.51 ns | 31.25 |  5.12 |  130,936 B |
|    Nodes2628_50PV | 171,132.1 ns | 2,932.05 ns | 2,742.65 ns | 60.54 | 14.89 |  254,104 B |

