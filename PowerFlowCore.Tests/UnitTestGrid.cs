using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFlowCore.Data;
using System.Collections.Generic;
using Complex = System.Numerics.Complex;


namespace PowerFlowCore.Tests
{
    [TestClass]
    public class TestGrid
    {
        private TestContext testContextInstance;

        [TestMethod]
        public void TestGrid_Y_SimpleWithoutTrans()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(){Num = 1, Unom=115},
                new Node(){Num = 2, Unom=115},
                new Node(){Num = 3, Unom=115, Ysh = new Complex(10, 10)},
            };

            List<Branch> branches = new List<Branch>()
            {
                new Branch(){Start=1, End=2,    Y=new Complex(5, -5),  Ktr=1,      Ysh=new Complex(0.2, 0.2)},
                new Branch(){Start=3, End=2,    Y=new Complex(2, -2),  Ktr=1,      Ysh=new Complex(0.5, 0.5)},
                new Branch(){Start=2, End=3,    Y=new Complex(6, -6),  Ktr=1,      Ysh=new Complex(0.8, 0.8)},
            };

            Grid net = new Grid(nodes, branches);

            Complex[,] Y = new Complex[3,3];
            
            Y[0, 0] = new Complex(-5.1,4.9);
            Y[1, 0] = new Complex(5,-5);
            Y[2, 0] = new Complex(0, 0);
            Y[0, 1] = new Complex(5,-5);
            Y[1, 1] = new Complex(-13.75,12.25);
            Y[2, 1] = new Complex(8,-8);
            Y[0, 2] = new Complex(0, 0);
            Y[1, 2] = new Complex(8,-8);
            Y[2, 2] = new Complex(-18.65,-2.65);

            for (int i = 0; i < Y.GetLength(0); i++)
            {
                for (int j = 0; j < Y.GetLength(1); j++)
                {
                    Y[i, j] = -Y[i, j];
                }
            }

            List<string> report = CalcMethods.CheckAdmittanceMatrix(net, Y);
            for (int i = 0; i < report.Count; i++)
            {
                TestContext.WriteLine(report[i]);
            }
            int Expected = 0;
            Assert.AreEqual(Expected, report.Count);
        }

        [TestMethod]
        public void TestGrid_Y_WithotTrans()
        {
            Grid net = SmallNodes.PV_110();
            List<string> report = CalcMethods.CheckAdmittanceMatrix(net, SmallNodes.PV_110_AdmittanceMatrix());
            for (int i = 0; i < report.Count; i++)
            {
                TestContext.WriteLine(report[i]);
            }
            int Expected = 0;
            Assert.AreEqual(Expected, report.Count);
        }




        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
    }
}
