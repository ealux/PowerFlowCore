using System;
using PowerFlowCore.Data;
using PowerFlowCore;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Tests
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

            Grid e = new Grid(net.Nodes, net.Branches);
            Engine.Calculate(e);
            Vector<Complex> calc = e.Ucalc;


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

            Grid e = new Grid(net.Nodes, net.Branches);
            Engine.Calculate(e);

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
                P[i] = net.Nodes[i].S_gen.Real      - net.Nodes[i].S_calc.Real;
                Q[i] = net.Nodes[i].S_gen.Imaginary - net.Nodes[i].S_calc.Imaginary;
            }

            var options = new CalculationOptions(); //Options

            Grid e = new Grid(net.Nodes, net.Branches);
            Engine.Calculate(e);

            double[] Qres = new double[net.Nodes.Count];
            double[] Pres = new double[net.Nodes.Count];

            for (int i = 0; i < net.Nodes.Count; i++)
            {
                Pres[i] = net.Nodes[i].S_gen.Real      - net.Nodes[i].S_calc.Real;
                Qres[i] = net.Nodes[i].S_gen.Imaginary - net.Nodes[i].S_calc.Imaginary;
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

            Grid e = new Grid(net.Nodes, net.Branches);
            Engine.Calculate(e);

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

        public static List<string>  CheckAdmittanceMatrix(Grid net, Matrix<Complex> expexMatrix)
        {
            List<string> res = new List<string>();
            for (int ind_row = 0; ind_row < expexMatrix.RowCount; ind_row++)
            {
                for (int ind_col = 0; ind_col < expexMatrix.ColumnCount; ind_col++)
                {
                    if ( (Math.Round( (net.Y[ind_row,ind_col] - expexMatrix[ind_row, ind_col]).Real, 4) > 0.0001))
                    {
                        res.Add(string.Concat("Error matrix admitance element: ind row = "
                        , Convert.ToString(ind_row)
                        , "; ind column = "
                        , (Convert.ToString(ind_col))
                        , ". Calc Real value = "
                        , Convert.ToString(net.Y[ind_row, ind_col].Real)
                        , ", expected Real value = "
                        , Convert.ToString(expexMatrix[ind_row, ind_col].Real) ));
                    }
                    if ((Math.Round((net.Y[ind_row, ind_col] - expexMatrix[ind_row, ind_col]).Imaginary, 4) > 0.0001))
                    {
                        res.Add(string.Concat("Error matrix admitance element: ind row = "
                        , Convert.ToString(ind_row)
                        , "; ind column = "
                        , (Convert.ToString(ind_col))
                        , ". Calc Imaginary value = "
                        , Convert.ToString(net.Y[ind_row, ind_col].Imaginary)
                        , "), expected Imaginary value = "
                        , Convert.ToString(expexMatrix[ind_row, ind_col].Imaginary) ));
                    }
                }
            }
            return res;
        }


    }
}
