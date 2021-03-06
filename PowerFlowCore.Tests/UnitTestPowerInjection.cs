using System.Collections.Generic;
using PowerFlowCore.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerFlowCore.Tests
{
    [TestClass]
    public class UnitTestPowerInjection
    {
        private TestContext testContextInstance;

        [TestMethod]
        public void Test_PowerInjectionPVtoPQmax_110()
        {
            Grid net = SmallNodes.PV_PQmax_load_110();
            List<string> report = CalcMethods.CheckSumPowerInjection(net);
            for (int i = 0; i < report.Count; i++)
            {
                TestContext.WriteLine(report[i]);
            }
            int Expected = 0;
            Assert.AreEqual(Expected, report.Count);
        }

        [TestMethod]
        public void Test_PowerInjectionPVtoPQmin_110()
        {
            Grid net = SmallNodes.PV_PQmin_load_110();
            List<string> report = CalcMethods.CheckSumPowerInjection(net);            
            for (int i = 0; i< report.Count; i++)
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
