using PowerFlowCore.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerFlowCore.Tests
{
    [TestClass]
    public class UnitTestTwoNodes
    {
        private TestContext testContextInstance;

        const double err = 0.001;

        [TestMethod]
        public void Test_PV_110()
        {
            Grid net = SmallNodes.PV_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
        }

        [TestMethod]
        public void Test_PQ_110()
        {
            Grid net = SmallNodes.PQ_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
        }

        [TestMethod]
        public void Test_Trans_PV_110()
        {
            Grid net = SmallNodes.Trans_PV_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
        }

        [TestMethod]
        public void Test_Trans_PQ_110()
        {
            Grid net = SmallNodes.Trans_PQ_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
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
