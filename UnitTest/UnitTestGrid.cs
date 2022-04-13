using System.Collections.Generic;
using PowerFlowCore.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class TestGrid
    {
        private TestContext testContextInstance;

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
