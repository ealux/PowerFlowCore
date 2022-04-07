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

            //===========================================

            var nodes = new List<Node>()
            {
                new Node(){Num = 1,   Type = NodeType.Slack,  Unom=Complex.FromPolarCoordinates(121, -5*Math.PI/180)},
                new Node(){Num = 2,   Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15,3)},
                new Node(){Num = 3,   Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15,3)},
                new Node(){Num = 5,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 6,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 7,   Type = NodeType.PQ,     Unom=110, Vpre = 0},
                new Node(){Num = 21,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(32, 14.58)},
                new Node(){Num = 31,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(26, 10.28)},
                new Node(){Num = 51,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(12, 5.47)},
                new Node(){Num = 61,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(10, 10.02)},
                new Node(){Num = 71,  Type = NodeType.PQ,     Unom=10.5, Vpre = 0,      S_load = new Complex(47, 26.64)},
                new Node(){Num = 200, Type = NodeType.PQ,     Unom=110, Vpre = 0,       S_load = new Complex(15, 14.58)},
                new Node(){Num = 201, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 0), S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15},
                new Node(){Num = 202, Type = NodeType.PQ,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 0), S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15},
                new Node(){Num = 203, Type = NodeType.PV,     Unom=10.5, Vpre = 10.5,   S_load = new Complex(2.2, 0), S_gen = new Complex(20, 0), Q_min = 10, Q_max = 15}
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

            timer.Restart();                                                           //Start timer

            for (int i = 0; i < 100; i++)
            {
                
                var e = new Engine(nodes, branches);                                    //Create engine
                e.Calculate();                                                          //Performe calculations
                var calc = e.desc.U_calc;                                               //Take calculated U values
                //Console.WriteLine(e.desc.U_calc.Map(x => x.Magnitude));               //Show U magnitudes
                e.desc.Nodes.ForEach(n => Console.WriteLine(n.Num.ToString()));         //Show Nodes numbers
                //Console.WriteLine(e.desc.U_calc.Map(x => x.Phase * 180 / Math.PI));   //Show U angles
                //Console.WriteLine(e.desc.S);                                          //Show vector of calculated S
                
            }

            Console.WriteLine("End with: " + timer.ElapsedMilliseconds + " ms");        //Stop timer and show result



            //Console.WriteLine(e.desc.);

            //var d = DataExtentions.GetDescription(nodes, branches);

            ////var U = d.NewtonRaphsonSolver(d.U_init);
            //var U = d.GaussSeidelSolver(d.U_init, iterations: 150);
            //Console.WriteLine(timer.ElapsedMilliseconds);

            //Console.WriteLine("CALC Magnitude\n: " + U.Map(x => x.Magnitude));
            //Console.WriteLine("CALC Angle\n: " + U.Map(x => x.Phase * 180 / Math.PI));
            //Console.WriteLine("CALC Angle\n: " + d.S);
            //Console.WriteLine(timer.ElapsedMilliseconds);

            //===========================================


            //===========================================

            //var nodes = new List<Node>()
            //{
            //    new Node(){Num = 1, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10,15)},
            //    new Node(){Num = 2, Type = NodeType.PQ,     Unom=110, Vpre = 0, S_load = new Complex(10,40)},
            //    new Node(){Num = 3, Type = NodeType.PV,     Unom=110, Vpre = 110, S_load = new Complex(10,0), S_gen = new Complex(25,0), Q_min=-15, Q_max=35},
            //    new Node(){Num = 4, Type = NodeType.Slack,  Unom=115, Vpre = 115}
            //};

            //var branches = new List<Branch>()
            //{
            //    new Branch(){Start=1, End=2, Y=1/(new Complex(10, 2)), Ktr=1, Ysh = new Complex(0, 0)},
            //    new Branch(){Start=1, End=3, Y=1/(new Complex(10, 20)), Ktr=1,Ysh = new Complex(0, 0)},
            //    new Branch(){Start=1, End=4, Y=1/(new Complex(8, 15)), Ktr=1, Ysh = new Complex(0, 0)},
            //    new Branch(){Start=2, End=4, Y=1/(new Complex(20, 40)), Ktr=1,}
            //};

            //var d = DataExtentions.GetDescription(nodes, branches);

            //var U = d.NewtonRaphsonSolver(d.U_init, accuracy: 1e-6, iterations: 1500);
            //Console.WriteLine(timer.ElapsedMilliseconds);

            //Console.WriteLine("CALC Magnitude\n: " + U.Map(x => x.Magnitude));
            //Console.WriteLine("CALC Angle\n: " + U.Map(x => x.Phase * 180 / Math.PI));
            //Console.WriteLine("CALC S\n: " + d.S);
            //Console.WriteLine(timer.ElapsedMilliseconds);



            //var U2 = d.GaussSeidelSolver(d.U_init, accuracy: 1e-6, iterations: 50);
            //Console.WriteLine(timer.ElapsedMilliseconds);

            //Console.WriteLine("CALC Magnitude\n: " + U2.Map(x => x.Magnitude));
            //Console.WriteLine("CALC Angle\n: " + U2.Map(x => x.Phase * 180 / Math.PI));
            //Console.WriteLine("CALC Angle\n: " + d.S);
            //Console.WriteLine(timer.ElapsedMilliseconds);


            //===========================================

            //===========================================
            //var nodes = new List<Node>()
            //{
            //    new Node(){Num = 1, Type = NodeType.PQ,     Unom=230, Vpre = 0, S_load = new Complex(100,0)},
            //    new Node(){Num = 2, Type = NodeType.PQ,     Unom=230, Vpre = 0, S_load = new Complex(50,25)},
            //    new Node(){Num = 3, Type = NodeType.Slack,  Unom=230, Vpre = 0},
            //};

            //var branches = new List<Branch>()
            //{
            //    new Branch(){Start=1, End=2, Y=1/(new Complex(4.9, 21.5)), Ktr=1, Ysh=new Complex(0, 1188e-6)},
            //    new Branch(){Start=2, End=3, Y=1/(new Complex(4.9, 21.5)), Ktr=1, Ysh=new Complex(0, 1188e-6)},
            //    new Branch(){Start=1, End=3, Y=1/(new Complex(4.9, 21.5)), Ktr=1, Ysh=new Complex(0, 1188e-6)},
            //};

            //var d = DataExtentions.GetDescription(nodes, branches);

            //var U = d.NewtonRaphsonSolver(d.U_init);
            //Console.WriteLine(timer.ElapsedMilliseconds);

            //Console.WriteLine("CALC Magnitude\n: " + U.Map(x => x.Magnitude));
            //Console.WriteLine("CALC Angle\n: " + U.Map(x => x.Phase * 180 / Math.PI));
            //Console.WriteLine(timer.ElapsedMilliseconds);

            //var A = Matrix<double>.Build.DenseOfArray(new[,] { {-4.63538, 2.31769, -4677.95007, 2338.97503},
            //                                                   { 2.31769, -4.63538, 2338.97503, -4677.95007},
            //                                                   { -19.79243, 10.16946, 1066.13746, -533.06873},
            //                                                   { 10.16946, -19.79243, -533.06873, 1066.13746} });

            //var B = Vector<double>.Build.DenseOfArray(new[] { -100, -50, 62.8452, 37.8452 });

            //Console.WriteLine(A);
            //Console.WriteLine();
            //Console.WriteLine(B);
            //Console.WriteLine();
            //Console.WriteLine(A.Solve(B));
            //Console.WriteLine();
            //Console.WriteLine(A.Inverse() * B);
            //Console.WriteLine();
            //Console.WriteLine(A.LU().Solve(B));



            //var U2 = d.GaussSeidelSolver(d.U_init, accuracy: 1e-6, iterations: 50);
            //Console.WriteLine(timer.ElapsedMilliseconds);

            //Console.WriteLine("CALC Magnitude\n: " + U2.Map(x => x.Magnitude));
            //Console.WriteLine("CALC Angle\n: " + U2.Map(x => x.Phase * 180 / Math.PI));
            //Console.WriteLine("CALC Angle\n: " + d.S);
            //Console.WriteLine(timer.ElapsedMilliseconds);


            //===========================================


            //var desc = DataExtentions.GetDescription(nodes, branches);

            //Console.WriteLine("MATRIX Y: " + timer.ElapsedMilliseconds + "\n");


            //Vector<double>  X = Vector<double>.Build.Dense(2 * desc.U_init.Count);
            //Vector<double> YY = Vector<double>.Build.Dense(2 * desc.U_init.Count);

            //Vector<double> Start_Rect = desc.UFromComplexToDouble_Rect(desc.U_init);
            //Vector<double> Start_Polar = desc.UFromComplexToDouble_Polar(desc.U_init);

            ////Левенберг - Polar
            //var obj = ObjectiveFunction.NonlinearModel(desc.PolarResidualModel, X, YY, accuracyOrder: 2);
            //var solver = new LevenbergMarquardtMinimizer(maximumIterations: 750);
            //var result = solver.FindMinimum(obj, Start_Polar);

            //Console.WriteLine("Init:\n" + desc.U_init + "\n");

            //Console.WriteLine(desc.UFromDoubleToComplex_Polar(result.MinimizingPoint) + "\tЛевенберг\t" + result.Iterations);
            //Console.WriteLine("LM: " + timer.Elapsed);


            ////Левенберг - Rect
            //var obj = ObjectiveFunction.NonlinearModel(desc.RectangleResidualModel, X, YY, accuracyOrder: 2);
            //var solver = new LevenbergMarquardtMinimizer(maximumIterations: 750);
            //var result = solver.FindMinimum(obj, Start_Rect);

            //Console.WriteLine("Init:\n" + desc.U_init + "\n");

            //Console.WriteLine(desc.UFromDoubleToComplex_Polar(result.MinimizingPoint) + "\tЛевенберг\t" + result.Iterations);
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
