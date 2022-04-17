using System;
using System.Collections.Generic;
using System.Diagnostics;
//using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.LinearAlgebra.Complex;
//using System.Threading.Tasks;
//using MathNet.Numerics;
//using MathNet.Numerics.Differentiation;
//using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.LinearAlgebra.Double.Solvers;
//using MathNet.Numerics.LinearAlgebra.Double;
//using MathNet.Numerics.LinearAlgebra.Solvers;
//using MathNet.Numerics.Optimization;
using PowerFlowCore.Data;
using PowerFlowCore;
using Complex = System.Numerics.Complex;

namespace TestCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            Console.WriteLine("===========================================");
            Console.WriteLine("15 nodes: 3 - PV  11 - PQ  1 - Slack");
            Console.WriteLine("===========================================");

            var nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,  Unom=Complex.FromPolarCoordinates(121, -5*Math.PI/180)},
                new Node(){Num = 2,   Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15,3)},
                new Node(){Num = 3,   Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15,3)},
                new Node(){Num = 5,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 6,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 7,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 21,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(16, 5)},
                new Node(){Num = 31,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(24, 10.28)},
                new Node(){Num = 51,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(12, 8)},
                new Node(){Num = 61,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(10, 10.02)},
                new Node(){Num = 71,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(16, 13)},
                new Node(){Num = 200, Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15, 14.58)},
                new Node(){Num = 201, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 12.5),  S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15},
                new Node(){Num = 202, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 0),  S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15},
                new Node(){Num = 203, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 0),  S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15}
            };

            var branches = new List<Branch>()
            {
                new Branch(){Start=1, End=3,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=1, End=3,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=1, End=5,    Y=1/(new Complex(10.69, 22.68)),Ktr=1,      Ysh=new Complex(0, 145.8e-6)},
                new Branch(){Start=3, End=7,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=3, End=7,    Y=1/(new Complex(3.6, 12.15)),  Ktr=1,      Ysh=new Complex(0, 84.3e-6)},
                new Branch(){Start=5, End=7,    Y=1/(new Complex(11.95, 20.5)), Ktr=1,      Ysh=new Complex(0, 127.7e-6)},
                new Branch(){Start=2, End=7,    Y=1/(new Complex(11.95, 20.5)), Ktr=1,      Ysh=new Complex(0, 127.7e-6)},
                new Branch(){Start=2, End=7,    Y=1/(new Complex(11.95, 20.5)), Ktr=1,      Ysh=new Complex(0, 127.7e-6)},
                new Branch(){Start=2, End=200,  Y=1/(new Complex(2.16, 7.29)),  Ktr=1,      Ysh=new Complex(0, 50.6e-6)},
                new Branch(){Start=2, End=6,    Y=1/(new Complex(7.47, 12.81)), Ktr=1,      Ysh=new Complex(0, 79.8e-6)},
                new Branch(){Start=6, End=200,  Y=1/(new Complex(10.46, 17.93)),Ktr=1,      Ysh=new Complex(0, 117.7e-6)},
                new Branch(){Start=200, End=201,Y=1/(new Complex(1.46, 38.4)),  Ktr=0.087,  Ysh=new Complex(3.4e-6, -17.8e-6)},
                new Branch(){Start=200, End=202,Y=1/(new Complex(1.46, 38.4)),  Ktr=0.087,  Ysh=new Complex(3.4e-6, -17.8e-6)},
                new Branch(){Start=200, End=203,Y=1/(new Complex(1.46, 38.4)),  Ktr=0.087,  Ysh=new Complex(3.4e-6, -17.8e-6)},
                new Branch(){Start=2, End=21,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=2, End=21,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=3, End=31,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=3, End=31,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=5, End=51,   Y=1/(new Complex(7.95, 139)),   Ktr=0.096,  Ysh=new Complex(1.06e-6, -5.03e-6)},
                new Branch(){Start=5, End=51,   Y=1/(new Complex(7.95, 139)),   Ktr=0.096,  Ysh=new Complex(1.06e-6, -5.03e-6)},
                new Branch(){Start=6, End=61,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=6, End=61,   Y=1/(new Complex(2.54, 55.9)),  Ktr=0.091,  Ysh=new Complex(2e-6, -13.2e-6)},
                new Branch(){Start=7, End=71,   Y=1/(new Complex(1.4, 34.7)),   Ktr=0.091,  Ysh=new Complex(2.72e-6, -19.66e-6)},
                new Branch(){Start=7, End=71,   Y=1/(new Complex(1.4, 34.7)),   Ktr=0.091,  Ysh=new Complex(2.72e-6, -19.66e-6)}
            };

            timer.Restart();                                                      //Start timer

            var options = new CalculationOptions() { IterationsCount = 300 };
            var e = new Engine(nodes, branches, options);                         //Create engine

            e.Calculate();                                                        //Performe calculations
            var calc = e.Grid.Ucalc;                                             //Take calculated U values

            e.Grid.Nodes.ForEach(n => Console.WriteLine(n.Num.ToString()));       //Show Nodes numbers
            for (int i = 0; i < e.Grid.Nodes.Count; i++)
                Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
                                  $" {e.Grid.Nodes[i].Type}" +
                                  " \tV.: " + Math.Round(e.Grid.Ucalc[i].Magnitude, 6) + 
                                  "\tAngle: " + Math.Round(e.Grid.Ucalc[i].Phase * 180 / Math.PI, 6));
            Console.WriteLine(e.Grid.S);                                          //Show vector of calculated S
            e.Grid.Nodes.ForEach(n => Console.WriteLine(n.S_gen.ToString()));       //Show Nodes numbers

            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result



            Console.WriteLine("\n\n===========================================");
            Console.WriteLine("3 nodes: 1 - PV  2 - PQ  1 - Slack");
            Console.WriteLine("===========================================");

            nodes = new List<Node>()
            {
                new Node(){Num = 1, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10, 15)},
                new Node(){Num = 2, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10, 40)},
                new Node(){Num = 3, Type = NodeType.PV,     Unom=110, Vpre = 110, S_load = new Complex(10, 25), S_gen = new Complex(25, 0), Q_min=15, Q_max=35},
                new Node(){Num = 4, Type = NodeType.Slack,  Unom=115, Vpre = 115}
            };

            branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2, Y=1/(new Complex(10, 2)), Ktr=1, Ysh = new Complex(0, 0)},
                new Branch(){Start=1, End=3, Y=1/(new Complex(10, 20)), Ktr=1,Ysh = new Complex(0, 0)},
                new Branch(){Start=1, End=4, Y=1/(new Complex(8, 15)), Ktr=1, Ysh = new Complex(0, 0)},
                new Branch(){Start=2, End=4, Y=1/(new Complex(20, 40)), Ktr=1,}
            };

            timer.Restart();                                                      //Start timer

            options = new CalculationOptions();
            e = new Engine(nodes, branches, options);                             //Create engine

            e.Calculate();                                                        //Performe calculations
            calc = e.Grid.Ucalc;                                                 //Take calculated U values

            e.Grid.Nodes.ForEach(n => Console.WriteLine(n.Num.ToString()));       //Show Nodes numbers
            for (int i = 0; i < e.Grid.Nodes.Count; i++)
                Console.WriteLine("Node: " + e.Grid.Nodes[i].Num +
                                  $" {e.Grid.Nodes[i].Type}" + 
                                  " \tV.: " + Math.Round(e.Grid.Ucalc[i].Magnitude, 6) +
                                  "\tAngle: " + Math.Round(e.Grid.Ucalc[i].Phase * 180 / Math.PI, 6));
            Console.WriteLine(e.Grid.S);                                          //Show vector of calculated S
            e.Grid.Nodes.ForEach(n => Console.WriteLine(n.S_gen.ToString()));       //Show Nodes numbers


            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result


            //Console.WriteLine("\n\n===========================================");
            //Console.WriteLine("3 nodes: 2 - PQ  1 - Slack");
            //Console.WriteLine("===========================================");
            //nodes = new List<Node>()
            //{
            //    new Node(){Num = 1, Type = NodeType.PQ,     Unom=230, Vpre = 0, S_load = new Complex(100,0)},
            //    new Node(){Num = 2, Type = NodeType.PQ,     Unom=230, Vpre = 0, S_load = new Complex(50,25)},
            //    new Node(){Num = 3, Type = NodeType.Slack,  Unom=230, Vpre = 0},
            //};

            //branches = new List<Branch>()
            //{
            //    new Branch(){Start=1, End=2, Y=1/(new Complex(4.9, 21.5)), Ktr=1, Ysh=new Complex(0, 1188e-6)},
            //    new Branch(){Start=2, End=3, Y=1/(new Complex(4.9, 21.5)), Ktr=1, Ysh=new Complex(0, 1188e-6)},
            //    new Branch(){Start=1, End=3, Y=1/(new Complex(4.9, 21.5)), Ktr=1, Ysh=new Complex(0, 1188e-6)},
            //};


            //timer.Restart();                                                      //Start timer

            //options = new CalculationOptions();
            //e = new Engine(nodes, branches, options);                             //Create engine

            //e.Calculate();                                                        //Performe calculations
            //calc = e.Grid.Ucalc;                                                 //Take calculated U values

            //e.Grid.Nodes.ForEach(n => Console.WriteLine(n.Num.ToString()));       //Show Nodes numbers
            //Console.WriteLine();
            //Console.WriteLine(e.Grid.Ucalc.Map(x => x.Magnitude));               //Show U magnitudes
            //Console.WriteLine(e.Grid.Ucalc.Map(x => x.Phase * 180 / Math.PI));   //Show U angles
            //Console.WriteLine(e.Grid.S);                                          //Show vector of calculated S

            //Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");  //Stop timer and show result

            ////===========================================


            //var Grid = ExtensionMethods.GetDescription(nodes, branches);

            //Console.WriteLine("MATRIX Y: " + timer.ElapsedMilliseconds + "\n");


            //Vector<double>  X = Vector<double>.Build.Dense(2 * Grid.Uinit.Count);
            //Vector<double> YY = Vector<double>.Build.Dense(2 * Grid.Uinit.Count);

            //Vector<double> Start_Rect = Grid.UFromComplexToDouble_Rect(Grid.Uinit);
            //Vector<double> Start_Polar = Grid.UFromComplexToDouble_Polar(Grid.Uinit);

            ////Левенберг - Polar
            //var obj = ObjectiveFunction.NonlinearModel(Grid.PolarResidualModel, X, YY, accuracyOrder: 2);
            //var solver = new LevenbergMarquardtMinimizer(maximumIterations: 750);
            //var result = solver.FindMinimum(obj, Start_Polar);

            //Console.WriteLine("Init:\n" + Grid.Uinit + "\n");

            //Console.WriteLine(Grid.UFromDoubleToComplex_Polar(result.MinimizingPoint) + "\tЛевенберг\t" + result.Iterations);
            //Console.WriteLine("LM: " + timer.Elapsed);


            ////Левенберг - Rect
            //var obj = ObjectiveFunction.NonlinearModel(Grid.RectangleResidualModel, X, YY, accuracyOrder: 2);
            //var solver = new LevenbergMarquardtMinimizer(maximumIterations: 750);
            //var result = solver.FindMinimum(obj, Start_Rect);

            //Console.WriteLine("Init:\n" + Grid.Uinit + "\n");

            //Console.WriteLine(Grid.UFromDoubleToComplex_Polar(result.MinimizingPoint) + "\tЛевенберг\t" + result.Iterations);
            //Console.WriteLine("LM: " + timer.Elapsed);


            //Console.WriteLine(Y);
            //Console.WriteLine();
            //Console.WriteLine(timer.ElapsedMilliseconds);




            //Console.WriteLine();
            //Console.WriteLine();

            //var m = Matrix<double>.Build.Random(5, 5, 5);
            //Console.WriteLine(m + "\n");
            //var mdiag = m.Diagonal();
            //Console.WriteLine(mdiag + "\n");
            //var mnondiag = Matrix<double>.Build.DenseOfMatrix(m); mnondiag.SetDiagonal(Vector<double>.Build.Dense(m.RowCount, 0.0));
            //Console.WriteLine(mnondiag + "\n");
            //Console.WriteLine("Initial:\n" + m);
            //double i = -15;
            //Console.WriteLine(+i);
            //Console.WriteLine();

            //var U1 = m.Column(0);
            //var U2 = m.Column(1);

            //var res = U1.ToColumnMatrix().Stack(U2.ToColumnMatrix()).Column(0);
            //Console.WriteLine("Stack:\n" + res);

            //int count = U1.Count;

            //var U3 = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(res.ToArray(), 0, count).ToArray());
            //var U4 = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(res.ToArray(), count, count).ToArray());

            //var res2 = Vector<Complex>.Build.DenseOfEnumerable(U3.Zip(U4, (u1, u2) => new Complex(u1, u2)));
            //Console.WriteLine("De-Stack:\n" + res2);

            Console.ReadKey();
        }
    }
}
