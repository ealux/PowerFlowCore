using System;
using System.Collections.Generic;

using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Samples
{
    public static partial class SampleGrids
    {
        public static Engine IEEE_57()
        {
            Logger.LogInfo("===========================================");
            Logger.LogInfo("IEEE-57: 6 - PV  50 - PQ  1 - Slack");
            Logger.LogInfo("===========================================");

            List<INode> nodes = new List<INode>()
            {
                new Node(){Num = 1,  Type = NodeType.Slack, Unom=Complex.FromPolarCoordinates(143.52, 0), S_load = new Complex(55.0, 17.0)},
                new Node(){Num = 2,  Type = NodeType.PV,    Unom=138, Vpre = 139.38,   S_load = new Complex(3.0, 88.0),   S_gen = new Complex(0, 0),     Q_min = -17,   Q_max = 50},
                new Node(){Num = 3,  Type = NodeType.PV,    Unom=138, Vpre = 135.93,   S_load = new Complex(41.0, 21.0),  S_gen = new Complex(40.0, 0),  Q_min = -10,   Q_max = 60},
                new Node(){Num = 4,  Type = NodeType.PQ,    Unom=138},
                new Node(){Num = 5,  Type = NodeType.PQ,    Unom=138,                  S_load = new Complex(13.0, 4.0)},
                new Node(){Num = 6,  Type = NodeType.PV,    Unom=138, Vpre = 135.24,   S_load = new Complex(75.0, 2.0),   S_gen = new Complex(0, 0),     Q_min = -8,    Q_max = 25},
                new Node(){Num = 7,  Type = NodeType.PQ,    Unom=138},
                new Node(){Num = 8,  Type = NodeType.PV,    Unom=138, Vpre = 138.69,   S_load = new Complex(150.0, 22.0), S_gen = new Complex(450.0, 0), Q_min = -140,  Q_max = 200},
                new Node(){Num = 9,  Type = NodeType.PV,    Unom=138, Vpre = 135.24,   S_load = new Complex(121.0, 26.0), S_gen = new Complex(0, 0),     Q_min = -3,    Q_max = 9},
                new Node(){Num = 10, Type = NodeType.PQ,    Unom=138,                  S_load = new Complex(5.0, 2.0)},
                new Node(){Num = 11, Type = NodeType.PQ,    Unom=138},
                new Node(){Num = 12, Type = NodeType.PV,    Unom=138, Vpre = 140.07,   S_load = new Complex(377.0, 24.0), S_gen = new Complex(310.0, 0), Q_min = -150,  Q_max = 155},
                new Node(){Num = 13, Type = NodeType.PQ,    Unom=138,                  S_load = new Complex(18.0, 2.3)},
                new Node(){Num = 14, Type = NodeType.PQ,    Unom=138,                  S_load = new Complex(10.5, 5.3)},
                new Node(){Num = 15, Type = NodeType.PQ,    Unom=138,                  S_load = new Complex(22.0, 5.0)},
                new Node(){Num = 16, Type = NodeType.PQ,    Unom=138,                  S_load = new Complex(43.0, 3.0)},
                new Node(){Num = 17, Type = NodeType.PQ,    Unom=138,                  S_load = new Complex(42.0, 8.0)},
                new Node(){Num = 18, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(27.2, 9.8), Ysh = new Complex(0, 2100.4e-6)},
                new Node(){Num = 19, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(3.3, 0.6)},
                new Node(){Num = 20, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(2.3, 1.0)},
                new Node(){Num = 21, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 22, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 23, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(6.3, 2.1)},
                new Node(){Num = 24, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 25, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(6.3, 3.2), Ysh = new Complex(0, 1239.24e-6)},
                new Node(){Num = 26, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 27, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(9.3, 0.5)},
                new Node(){Num = 28, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(4.6, 2.3)},
                new Node(){Num = 29, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(17.0, 2.6)},
                new Node(){Num = 30, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(3.6, 1.8)},
                new Node(){Num = 31, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(5.8, 2.9)},
                new Node(){Num = 32, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(1.6, 0.8)},
                new Node(){Num = 33, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(3.8, 1.9)},
                new Node(){Num = 34, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 35, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(6.0, 3.0)},
                new Node(){Num = 36, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 37, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 38, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(14.0, 7.0)},
                new Node(){Num = 39, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 40, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 41, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(6.3, 3.0)},
                new Node(){Num = 42, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(7.1, 4.4)},
                new Node(){Num = 43, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(2.0, 1.0)},
                new Node(){Num = 44, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(12.0, 1.8)},
                new Node(){Num = 45, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 46, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 47, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(29.7, 11.6)},
                new Node(){Num = 48, Type = NodeType.PQ,    Unom=69},
                new Node(){Num = 49, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(18.0, 8.5)},
                new Node(){Num = 50, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(21.0, 10.5)},
                new Node(){Num = 51, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(18.0, 5.3)},
                new Node(){Num = 52, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(4.9, 2.2)},
                new Node(){Num = 53, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(20.0, 10.0), Ysh = new Complex(0, 1323.25e-6)},
                new Node(){Num = 54, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(4.1, 1.4)},
                new Node(){Num = 55, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(6.8, 3.4)},
                new Node(){Num = 56, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(7.6, 2.2)},
                new Node(){Num = 57, Type = NodeType.PQ,    Unom=69,                   S_load = new Complex(6.7, 2.0)}
            };

            List<IBranch> branches = new List<IBranch>()
            {
                new Branch(){Start=1,  End=2,   Y=1/(new Complex(1.5807, 5.3323)),   Ktr=1,          Ysh=new Complex(0, 677.4e-6)},
                new Branch(){Start=2,  End=3,   Y=1/(new Complex(5.6751, 16.1874)),  Ktr=1,          Ysh=new Complex(0, 429.5e-6)},
                new Branch(){Start=3,  End=4,   Y=1/(new Complex(2.1329, 6.9701)),   Ktr=1,          Ysh=new Complex(0, 199.5e-6)},
                new Branch(){Start=4,  End=5,   Y=1/(new Complex(11.9025, 25.1381)), Ktr=1,          Ysh=new Complex(0, 135.5e-6)},
                new Branch(){Start=4,  End=6,   Y=1/(new Complex(8.1889, 28.1851)),  Ktr=1,          Ysh=new Complex(0, 182.7e-6)},
                new Branch(){Start=6,  End=7,   Y=1/(new Complex(3.8088, 19.4249)),  Ktr=1,          Ysh=new Complex(0, 144.9e-6)},
                new Branch(){Start=6,  End=8,   Y=1/(new Complex(6.4559, 32.9461)),  Ktr=1,          Ysh=new Complex(0, 246.8e-6)},
                new Branch(){Start=8,  End=9,   Y=1/(new Complex(1.8854, 9.6172)),   Ktr=1,          Ysh=new Complex(0, 287.8e-6)},
                new Branch(){Start=9,  End=10,  Y=1/(new Complex(7.0272, 31.9749)),  Ktr=1,          Ysh=new Complex(0, 231.0e-6)},
                new Branch(){Start=9,  End=11,  Y=1/(new Complex(4.9134, 16.1493)),  Ktr=1,          Ysh=new Complex(0, 114.5e-6)},
                new Branch(){Start=9,  End=12,  Y=1/(new Complex(12.3405, 56.1798)), Ktr=1,          Ysh=new Complex(0, 405.4e-6)},
                new Branch(){Start=9,  End=13,  Y=1/(new Complex(9.1602, 30.0895)),  Ktr=1,          Ysh=new Complex(0, 213.2e-6)},
                new Branch(){Start=13, End=14,  Y=1/(new Complex(2.5138, 8.2651)),   Ktr=1,          Ysh=new Complex(0, 57.8e-6)},
                new Branch(){Start=13, End=15,  Y=1/(new Complex(5.1228, 16.5492)),  Ktr=1,          Ysh=new Complex(0, 120.8e-6)},
                new Branch(){Start=1,  End=15,  Y=1/(new Complex(3.3898, 17.3300)),  Ktr=1,          Ysh=new Complex(0, 518.8e-6)},
                new Branch(){Start=1,  End=16,  Y=1/(new Complex(8.6460, 39.2306)),  Ktr=1,          Ysh=new Complex(0, 286.7e-6)},
                new Branch(){Start=1,  End=17,  Y=1/(new Complex(4.5325, 20.5675)),  Ktr=1,          Ysh=new Complex(0, 150.2e-6)},
                new Branch(){Start=3,  End=15,  Y=1/(new Complex(3.0851, 10.0933)),  Ktr=1,          Ysh=new Complex(0, 285.7e-6)},
                new Branch(){Start=4,  End=18,  Y=1/(new Complex(0.0, 99.4477)),     Ktr=0.515464,   Ysh=new Complex(0, 0)},
                new Branch(){Start=4,  End=18,  Y=1/(new Complex(0.0, 78.3257)),     Ktr=0.511247,   Ysh=new Complex(0, 0)},
                new Branch(){Start=5,  End=6,   Y=1/(new Complex(5.7513, 12.2072)),  Ktr=1,          Ysh=new Complex(0, 65.1e-6)},
                new Branch(){Start=7,  End=8,   Y=1/(new Complex(2.6471, 13.5593)),  Ktr=1,          Ysh=new Complex(0, 101.9e-6)},
                new Branch(){Start=10, End=12,  Y=1/(new Complex(5.2752, 24.0335)),  Ktr=1,          Ysh=new Complex(0, 172.2e-6)},
                new Branch(){Start=11, End=13,  Y=1/(new Complex(4.2468, 13.9402)),  Ktr=1,          Ysh=new Complex(0, 98.7e-6)},
                new Branch(){Start=12, End=13,  Y=1/(new Complex(3.3898, 11.0455)),  Ktr=1,          Ysh=new Complex(0, 317.2e-6)},
                new Branch(){Start=12, End=16,  Y=1/(new Complex(3.4279, 15.4828)),  Ktr=1,          Ysh=new Complex(0, 113.4e-6)},
                new Branch(){Start=12, End=17,  Y=1/(new Complex(7.5605, 34.0888)),  Ktr=1,          Ysh=new Complex(0, 249.9e-6)},
                new Branch(){Start=14, End=15,  Y=1/(new Complex(3.2565, 10.4171)),  Ktr=1,          Ysh=new Complex(0, 77.7e-6)},
                new Branch(){Start=18, End=19,  Y=1/(new Complex(21.9482, 32.6129)), Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=19, End=20,  Y=1/(new Complex(13.4736, 20.6627)), Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=21, End=20,  Y=1/(new Complex(0.0, 40.2272)),     Ktr=0.958773,   Ysh=new Complex(0, 0)},
                new Branch(){Start=21, End=22,  Y=1/(new Complex(3.5041, 5.5704)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=22, End=23,  Y=1/(new Complex(0.4713, 0.7237)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=23, End=24,  Y=1/(new Complex(7.9033, 12.1882)),  Ktr=1,          Ysh=new Complex(0, 176.4e-6)},
                new Branch(){Start=24, End=25,  Y=1/(new Complex(0.0, 56.2750)),     Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=24, End=25,  Y=1/(new Complex(0.0, 58.5603)),     Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=24, End=26,  Y=1/(new Complex(0.0, 2.4498)),      Ktr=0.958773,   Ysh=new Complex(0, 0)},
                new Branch(){Start=26, End=27,  Y=1/(new Complex(7.8557, 12.0929)),  Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=27, End=28,  Y=1/(new Complex(2.9423, 4.5420)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=28, End=29,  Y=1/(new Complex(1.9901, 2.7947)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=7,  End=29,  Y=1/(new Complex(0.0, 11.5395)),     Ktr=0.517063,   Ysh=new Complex(0, 0)},
                new Branch(){Start=25, End=30,  Y=1/(new Complex(6.4274, 9.6172)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=30, End=31,  Y=1/(new Complex(15.5209, 23.6622)), Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=31, End=32,  Y=1/(new Complex(24.1383, 35.9456)), Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=32, End=33,  Y=1/(new Complex(1.8663, 1.7140)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=34, End=32,  Y=1/(new Complex(0.0, 43.1321)),     Ktr=1.025641,   Ysh=new Complex(0, 0)},
                new Branch(){Start=34, End=35,  Y=1/(new Complex(2.4757, 3.7136)),   Ktr=1,          Ysh=new Complex(0, 67.2e-6)},
                new Branch(){Start=35, End=36,  Y=1/(new Complex(2.0472, 2.5567)),   Ktr=1,          Ysh=new Complex(0, 33.6e-6)},
                new Branch(){Start=36, End=37,  Y=1/(new Complex(1.3807, 1.7425)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=37, End=38,  Y=1/(new Complex(3.0994, 4.8038)),   Ktr=1,          Ysh=new Complex(0, 42.0e-6)},
                new Branch(){Start=37, End=39,  Y=1/(new Complex(1.1379, 1.8044)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=36, End=40,  Y=1/(new Complex(1.4283, 2.2186)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=22, End=38,  Y=1/(new Complex(0.9141, 1.4045)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=11, End=41,  Y=1/(new Complex(0.0, 30.0908)),     Ktr=0.523560,   Ysh=new Complex(0, 0)},
                new Branch(){Start=41, End=42,  Y=1/(new Complex(9.8553, 16.7587)),  Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=41, End=43,  Y=1/(new Complex(0.0, 19.6153)),     Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=38, End=44,  Y=1/(new Complex(1.3759, 2.7852)),   Ktr=1,          Ysh=new Complex(0, 42.0e-6)},
                new Branch(){Start=15, End=45,  Y=1/(new Complex(0.0, 18.0981)),     Ktr=0.523560,   Ysh=new Complex(0, 0)},
                new Branch(){Start=14, End=46,  Y=1/(new Complex(0.0, 11.3378)),     Ktr=0.555556,   Ysh=new Complex(0, 0)},
                new Branch(){Start=46, End=47,  Y=1/(new Complex(1.0950, 3.2375)),   Ktr=1,          Ysh=new Complex(0, 67.2e-6)},
                new Branch(){Start=47, End=48,  Y=1/(new Complex(0.8665, 1.1093)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=48, End=49,  Y=1/(new Complex(3.9707, 6.1417)),   Ktr=1,          Ysh=new Complex(0, 100.8e-6)},
                new Branch(){Start=49, End=50,  Y=1/(new Complex(3.8136, 6.0941)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=50, End=51,  Y=1/(new Complex(6.5987, 10.4742)),  Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=10, End=51,  Y=1/(new Complex(0.0, 11.7275)),     Ktr=0.537634,   Ysh=new Complex(0, 0)},
                new Branch(){Start=13, End=49,  Y=1/(new Complex(0.0, 29.1365)),     Ktr=0.558659,   Ysh=new Complex(0, 0)},
                new Branch(){Start=29, End=52,  Y=1/(new Complex(6.8654, 8.9031)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=52, End=53,  Y=1/(new Complex(3.6279, 4.6848)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=53, End=54,  Y=1/(new Complex(8.9412, 11.0455)),  Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=54, End=55,  Y=1/(new Complex(8.2461, 10.7837)),  Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=11, End=43,  Y=1/(new Complex(0.0, 26.7412)),     Ktr=0.521921,   Ysh=new Complex(0, 0)},
                new Branch(){Start=44, End=45,  Y=1/(new Complex(2.9709, 5.9132)),   Ktr=1,          Ysh=new Complex(0, 84.0e-6)},
                new Branch(){Start=40, End=56,  Y=1/(new Complex(0.0, 52.2152)),     Ktr=1.043841,   Ysh=new Complex(0, 0)},
                new Branch(){Start=56, End=41,  Y=1/(new Complex(26.3283, 26.1379)), Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=56, End=42,  Y=1/(new Complex(10.1171, 16.8539)), Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=39, End=57,  Y=1/(new Complex(0.0, 61.9569)),     Ktr=1.020408,   Ysh=new Complex(0, 0)},
                new Branch(){Start=57, End=56,  Y=1/(new Complex(8.2841, 12.3786)),  Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=38, End=49,  Y=1/(new Complex(5.4752, 8.4270)),   Ktr=1,          Ysh=new Complex(0, 63.0e-6)},
                new Branch(){Start=38, End=48,  Y=1/(new Complex(1.4854, 2.2948)),   Ktr=1,          Ysh=new Complex(0, 0)},
                new Branch(){Start=9,  End=55,  Y=1/(new Complex(0.0, 20.2769)),     Ktr=0.531915,   Ysh=new Complex(0, 0)}
            };

            var options = new CalculationOptions();             //Create options
            var engine = new Engine(nodes, branches, options);  //Create engine

            return engine;
        }
    }
}
