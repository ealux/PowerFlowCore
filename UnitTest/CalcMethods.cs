using System;
using PowerFlowCore.Data;
using PowerFlowCore;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Complex = System.Numerics.Complex;

namespace UnitTest
{
    public static class CalcMethods
    {
        public static bool calc(Grid net, double err)
        {
            Complex[] U = new Complex[net.Nodes.Count];

            for (int i = 0; i < net.Nodes.Count; i++)
            {
                U[i] = (Complex)net.Nodes[i].U;
            }

            double[] delta = new double[net.Nodes.Count];


            var options = new CalculationOptions(); //Options

            Engine e = new Engine(net.Nodes, net.Branches, options);
            e.Calculate();
            Vector<Complex> calc = e.Grid.Ucalc;


            for (int i = 0; i < net.Nodes.Count; i++)
            {
                delta[i] = (calc[i].Magnitude - U[i].Magnitude) 
                           / U[i].Magnitude;
            }
            double maxValue = delta.Max();


            for (int i = 0; i < net.Nodes.Count; i++)
            {
                delta[i] = (calc[i].Phase - U[i].Phase) 
                           / U[i].Phase;
            }
            double maxValue2 = delta.Max();

            bool res = false;
            if ((maxValue < err) && (maxValue2 < err)) res = true;

            return res;
        }

        public static List<string> CheckPV_genLimit(Grid net, double err)
        {
            Dictionary<int,double> Qgen = new Dictionary<int, double>();
            //save target Qgen
            for (int i = 0; i < net.Nodes.Count; i++) 
            {
                if (net.Nodes[i].Type == NodeType.PV) Qgen.Add(i, net.Nodes[i].S_gen.Imaginary);
            }

            var options = new CalculationOptions(); //Options

            Engine e = new Engine(net.Nodes, net.Branches, options);
            e.Calculate();

            List<string> res = new List<string>();
            int[] k = Qgen.Keys.ToArray();
            for (int i = 0; i < Qgen.Count; i++)
            {
                if (net.Nodes[k[i]].S_gen.Imaginary != Qgen[i])
                {
                    res.Add(string.Concat("Error calc Qgen in Node: " 
                        , Convert.ToString(net.Nodes[i].Num)
                        , ". Calc value = "
                        , Convert.ToString(net.Nodes[i].S_gen.Imaginary)
                        , ", expected value = "
                        , Convert.ToString(Qgen[i])));                                        
                }
            }
            return res;
        }

        public static List<string> CheckSumPowerInjection(Grid net)
        {
            double[] Q = new double[net.Nodes.Count];
            double[] P = new double[net.Nodes.Count];

            for (int i = 0; i < net.Nodes.Count; i++)
            {
                P[i] = net.Nodes[i].S_gen.Real      - net.Nodes[i].S_load.Real;
                Q[i] = net.Nodes[i].S_gen.Imaginary - net.Nodes[i].S_load.Imaginary;
            }

            var options = new CalculationOptions(); //Options

            Engine e = new Engine(net.Nodes, net.Branches, options);
            e.Calculate();

            double[] Qres = new double[net.Nodes.Count];
            double[] Pres = new double[net.Nodes.Count];

            for (int i = 0; i < net.Nodes.Count; i++)
            {
                Pres[i] = net.Nodes[i].S_gen.Real      - net.Nodes[i].S_load.Real;
                Qres[i] = net.Nodes[i].S_gen.Imaginary - net.Nodes[i].S_load.Imaginary;
            }

            List<string> res = new List<string>();
            for (int i = 0; i < net.Nodes.Count; i++)
            {
                if ((P[i] - Pres[i]) > 0.001) res.Add("Error sum P Node: " + Convert.ToString(net.Nodes[i].Num));
                if ((Q[i] - Qres[i]) > 0.001) res.Add("Error sum Q Node: " + Convert.ToString(net.Nodes[i].Num));                
            }

            return res;
            }

        public static List<string> CheckChangeTypeNodes(Grid net)
        {
            List<NodeType> nt = new List<NodeType>();
            for (int i = 0; i < net.Nodes.Count; i++)
            {
                nt.Add(net.Nodes[i].Type);
            }

            var options = new CalculationOptions(); //Options

            Engine e = new Engine(net.Nodes, net.Branches, options);
            e.Calculate();

            List<string> res = new List<string>();
            for (int i = 0; i < net.Nodes.Count; i++)
            {
                if (nt[i] != net.Nodes[i].Type)
                    res.Add(string.Concat("Error type Node: "
                        , Convert.ToString(net.Nodes[i].Num)
                        , ". Calc value = "
                        , Convert.ToString(net.Nodes[i].Type)
                        , ", expected value = "
                        , Convert.ToString(nt[i])));
            }

            return res;
        }

    }
}
