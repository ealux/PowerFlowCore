using PowerFlowCore.Data;
using PowerFlowCore;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using Complex = System.Numerics.Complex;

namespace UnitTest
{
    public static class CalcMethods
    {
        public static bool calc(NetDescription net, double err)
        {
            Complex[] U = U = new Complex[net.Nodes.Count];
            for (int i = 0; i < net.Nodes.Count; i++)
            {
                U[i] = (Complex)net.Nodes[i].U;
            }
            double[] delta = new double[net.Nodes.Count];

            Engine e = new Engine(net.Nodes, net.Branches);
            e.Calculate();
            Vector<Complex> calc = e.desc.U_calc;

            for (int i = 0; i < net.Nodes.Count; i++)
            {
                delta[i] = (calc[i].Magnitude - U[i].Magnitude) / U[i].Magnitude;
            }
            double maxValue = delta.Max();
            for (int i = 0; i < net.Nodes.Count; i++)
            {
                delta[i] = (calc[i].Phase - U[i].Phase) / U[i].Phase;
            }
            double maxValue2 = delta.Max();

            bool res = false;
            if ((maxValue < err) && (maxValue2 < err)) res = true;

            return res;
        }
    }
}
