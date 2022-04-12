using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using PowerFlowCore.Data;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Calculate additional powers and currents in Grid object
        /// </summary>
        /// <param name="desc">Grid object to be calculated</param>
        public static void CalculatePowerMatrix(this Grid desc)
        {
            //Slack buses
            for (int i = 0; i < desc.Slack_Count; i++)
            {
                var s = new Complex();

                for (int j = 0; j < desc.Nodes.Count; j++) 
                    s+= desc.U_calc[desc.PQ_Count + desc.PV_Count + i].Conjugate() 
                        * desc.U_calc[j] 
                        * desc.Y[desc.PQ_Count + desc.PV_Count + i, j];

                desc.S[desc.PQ_Count + desc.PV_Count + i] = s.Conjugate();
            }

            //Generation power re-calculation
            for (int i = 0; i < desc.Nodes.Count; i++) 
                desc.Nodes[i].S_gen = desc.S[i] + desc.Nodes[i].S_load;
                        
            foreach (var item in desc.Branches)
            {
                var start = item.Start_calc;
                var end = item.End_calc;

                //Lines or Breakers
                if (item.Ktr == 1 | item.Ktr == 0) 
                {
                    item.I_start    = (desc.U_calc[start] - desc.U_calc[end]) 
                                        * desc.Y[start, end] 
                                        / Math.Sqrt(3) 
                                        / item.Count;
                    item.I_end      = (desc.U_calc[end] - desc.U_calc[start]) 
                                        * desc.Y[end, start] 
                                        / Math.Sqrt(3) 
                                        / item.Count;

                    item.S_start    = Math.Sqrt(3) 
                                        * desc.U_calc[start] 
                                        * item.I_start.Conjugate();
                    item.S_end      = Math.Sqrt(3) 
                                        * desc.U_calc[end] 
                                        * item.I_end.Conjugate();
                }
                //Transformers
                else
                {
                    if(desc.Nodes[start].Unom.Magnitude > desc.Nodes[end].Unom.Magnitude)
                    {
                        item.I_start    = (desc.U_calc[start] * item.Ktr - desc.U_calc[end] ) 
                                          * desc.Y[start, end] 
                                          / Math.Sqrt(3) 
                                          / item.Count;
                        item.I_end      = (desc.U_calc[end] / item.Ktr - desc.U_calc[start]) 
                                          * desc.Y[end, start] 
                                          / Math.Sqrt(3) 
                                          / item.Count;

                        item.S_start    = Math.Sqrt(3) 
                                            * desc.U_calc[start] 
                                            * item.I_start.Conjugate();
                        item.S_end      = Math.Sqrt(3) 
                                            * desc.U_calc[end] 
                                            * item.I_end.Conjugate();
                    }
                    else if (desc.Nodes[start].Unom.Magnitude < desc.Nodes[end].Unom.Magnitude) 
                    {
                        item.I_start    = (desc.U_calc[start] / item.Ktr - desc.U_calc[end]) 
                                            * desc.Y[start, end] 
                                            / Math.Sqrt(3) 
                                            / item.Count;
                        item.I_end      = (desc.U_calc[end] * item.Ktr - desc.U_calc[start]) 
                                            * desc.Y[end, start] 
                                            / Math.Sqrt(3) 
                                            / item.Count;

                        item.S_start    = Math.Sqrt(3) 
                                            * desc.U_calc[start] 
                                            * item.I_start.Conjugate();
                        item.S_end      = Math.Sqrt(3) 
                                            * desc.U_calc[end] 
                                            * item.I_end.Conjugate();
                    }
                }
            }
        }

    }
}
