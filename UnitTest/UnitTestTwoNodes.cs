using PowerFlowCore.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTestTwoNodes
    {
        const double err = 0.001;

        [TestMethod]
        public void Test_PV_110()
        {
            Grid net = TwoNodesGrid.PV_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
        }

        [TestMethod]
        public void Test_PQ_110()
        {
            Grid net = TwoNodesGrid.PQ_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
        }

        [TestMethod]
        public void Test_Trans_PV_110()
        {
            Grid net = TwoNodesGrid.Trans_PV_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
        }

        [TestMethod]
        public void Test_Trans_PQ_110()
        {
            Grid net = TwoNodesGrid.Trans_PQ_110();
            bool res = CalcMethods.calc(net, err);
            bool Expected = true;
            Assert.AreEqual(Expected, res);
        }


    }
}
