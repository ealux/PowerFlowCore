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
        /// Calculate additional powers and currents in <seealso cref="Grid"/> object
        /// </summary>
        /// <param name="grid"><seealso cref="Grid"/> object to calculate flows</param>
        public static void CalculatePowerMatrix(this Grid grid)
        {
            //Slack buses
            for (int i = 0; i < grid.Slack_Count; i++)
            {
                var s = new Complex();

                for (int j = 0; j < grid.Nodes.Count; j++) 
                    s+= grid.Ucalc[grid.PQ_Count + grid.PV_Count + i].Conjugate() 
                        * grid.Ucalc[j] 
                        * grid.Y[grid.PQ_Count + grid.PV_Count + i, j];

                // Fill slack power flow as generation
                grid.Nodes[grid.PQ_Count + grid.PV_Count + i].S_gen = s.Conjugate();
            }

            // Current and power flows in branches
            foreach (var item in grid.Branches)
            {
                var start = item.Start_calc;
                var end = item.End_calc;

                //Lines or Breakers
                if (item.Ktr == 1 | item.Ktr == 0) 
                {
                    item.I_start    = (grid.Ucalc[start] - grid.Ucalc[end]) 
                                        * grid.Y[start, end] 
                                        / Math.Sqrt(3) 
                                        / item.Count;
                    item.I_end      = (grid.Ucalc[end] - grid.Ucalc[start]) 
                                        * grid.Y[end, start] 
                                        / Math.Sqrt(3) 
                                        / item.Count;

                    item.S_start    = Math.Sqrt(3) 
                                        * grid.Ucalc[start] 
                                        * item.I_start.Conjugate();
                    item.S_end      = Math.Sqrt(3) 
                                        * grid.Ucalc[end] 
                                        * item.I_end.Conjugate();
                }
                //Transformers
                else
                {
                    if(grid.Nodes[start].Unom.Magnitude > grid.Nodes[end].Unom.Magnitude)
                    {
                        item.I_start    = (grid.Ucalc[start] * item.Ktr - grid.Ucalc[end] ) 
                                          * grid.Y[start, end] 
                                          / Math.Sqrt(3) 
                                          / item.Count;
                        item.I_end      = (grid.Ucalc[end] / item.Ktr - grid.Ucalc[start]) 
                                          * grid.Y[end, start] 
                                          / Math.Sqrt(3) 
                                          / item.Count;

                        item.S_start    = Math.Sqrt(3) 
                                            * grid.Ucalc[start] 
                                            * item.I_start.Conjugate();
                        item.S_end      = Math.Sqrt(3) 
                                            * grid.Ucalc[end] 
                                            * item.I_end.Conjugate();
                    }
                    else if (grid.Nodes[start].Unom.Magnitude < grid.Nodes[end].Unom.Magnitude) 
                    {
                        item.I_start    = (grid.Ucalc[start] / item.Ktr - grid.Ucalc[end]) 
                                            * grid.Y[start, end] 
                                            / Math.Sqrt(3) 
                                            / item.Count;
                        item.I_end      = (grid.Ucalc[end] * item.Ktr - grid.Ucalc[start]) 
                                            * grid.Y[end, start] 
                                            / Math.Sqrt(3) 
                                            / item.Count;

                        item.S_start    = Math.Sqrt(3) 
                                            * grid.Ucalc[start] 
                                            * item.I_start.Conjugate();
                        item.S_end      = Math.Sqrt(3) 
                                            * grid.Ucalc[end] 
                                            * item.I_end.Conjugate();
                    }
                }
            }
        }

    }
}
