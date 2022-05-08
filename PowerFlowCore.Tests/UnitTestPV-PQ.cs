using System.Collections.Generic;
using PowerFlowCore.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerFlowCore.Tests
{

    [TestClass]
    public class UnitTest_PV_PQ
    {
        private TestContext testContextInstance;

        const double err = 0.001;

        [TestMethod]
        public void Test_PVtoPQmax_110()
        {
            Grid net = SmallNodes.PV_PQmax_load_110();
            List<string> report = CalcMethods.CheckPV_genLimit(net, err);
            for (int i = 0; i < report.Count; i++)
            {
                TestContext.WriteLine(report[i]);
            }
            int Expected = 0;
            Assert.AreEqual(Expected, report.Count);
        }


        [TestMethod]
        public void Test_PVtoPQmin_110()
        {
            Grid net = SmallNodes.PV_PQmin_load_110();
            List<string> report = CalcMethods.CheckPV_genLimit(net, err);
            for (int i = 0; i < report.Count; i++)
            {
                TestContext.WriteLine(report[i]);
            }
            int Expected = 0;
            Assert.AreEqual(Expected, report.Count);
        }

        [TestMethod]
        public void Test_ChangeTypeNodes_TwoNode_PVtoPQmax()
        {
            Grid net = SmallNodes.PV_PQmax_load_110();
            List<string> report = CalcMethods.CheckChangeTypeNodes(net);
            for (int i = 0; i < report.Count; i++)
            {
                TestContext.WriteLine(report[i]);
            }
            int Expected = 0;
            Assert.AreEqual(Expected, report.Count);
        }

        [TestMethod]
        public void Test_ChangeTypeNodes_TwoNode_PVtoPQmin()
        {
            Grid net = SmallNodes.PV_PQmin_load_110();
            List<string> report = CalcMethods.CheckChangeTypeNodes(net);
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
