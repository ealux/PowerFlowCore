using System;
using MathNet.Numerics;
using PowerFlowCore.Data;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Calculate additional powers and currents in <see cref="Grid"/> object
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object to calculate flows</param>
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
                var end   = item.End_calc;
                var kt    = item.Ktr.Magnitude <= 0 ? Complex.One : item.Ktr;

                //Lines or Breakers
                if (kt == 1.0) 
                {
                    item.I_start = (grid.Ucalc[start] - grid.Ucalc[end])
                                        * item.Y
                                        / Math.Sqrt(3);
                    item.I_end = (grid.Ucalc[end] - grid.Ucalc[start])
                                        * item.Y
                                        / Math.Sqrt(3);

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
                    if(grid.Nodes[start].Unom.Magnitude > grid.Nodes[end].Unom.Magnitude 
                        | grid.Nodes[start].Unom.Magnitude == grid.Nodes[end].Unom.Magnitude)
                    {
                        item.I_start = (grid.Ucalc[start] * kt - grid.Ucalc[end])
                                          * item.Y / kt
                                          / Math.Sqrt(3);
                        item.I_end = (grid.Ucalc[end] / kt - grid.Ucalc[start])
                                          * item.Y / Complex.Conjugate(kt)
                                          / Math.Sqrt(3);
                        item.S_start    = Math.Sqrt(3) 
                                            * grid.Ucalc[start] 
                                            * item.I_start.Conjugate();
                        item.S_end      = Math.Sqrt(3) 
                                            * grid.Ucalc[end] 
                                            * item.I_end.Conjugate();
                    }
                    else if (grid.Nodes[start].Unom.Magnitude < grid.Nodes[end].Unom.Magnitude) 
                    {
                        item.I_start = (grid.Ucalc[start] / kt - grid.Ucalc[end])
                                            * item.Y / Complex.Conjugate(kt)
                                            / Math.Sqrt(3);
                        item.I_end = (grid.Ucalc[end] * kt - grid.Ucalc[start])
                                            * item.Y / kt
                                            / Math.Sqrt(3);
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
