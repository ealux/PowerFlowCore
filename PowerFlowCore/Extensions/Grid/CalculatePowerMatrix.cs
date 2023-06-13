using PowerFlowCore.Algebra;
using System;
using System.Threading.Tasks;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Calculate power flows and currents in branches
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object to calculate flows</param>
        public static void CalculatePowerMatrix(this Grid grid)
        {
            var ucalc = grid.Ucalc.Copy();

            //Slack buses
            for (int i = 0; i < grid.Slack_Count; i++)
            {
                var s = new Complex();  // Slack power
                var pos = grid.PQ_Count + grid.PV_Count + i;

                for (int j = grid.Ysp.RowPtr[pos]; j < grid.Ysp.RowPtr[pos + 1]; j++)
                {
                    s += ucalc[pos].Conjugate()
                            * ucalc[grid.Ysp.ColIndex[j]]
                            * grid.Ysp.Values[j];
                }

                // Fill slack power flow as generation
                grid.Nodes[pos].S_gen = s.Conjugate() + grid.Nodes[pos].S_calc;
            }

            // Current and power flows in branches

            Parallel.ForEach(grid.Branches, item =>
            {
                {
                    var start = item.Start_calc;
                    var end = item.End_calc;
                    var kt = item.Ktr.Magnitude <= 0 ? Complex.One : item.Ktr;

                    //Lines or Breakers
                    if (kt == Complex.One)
                    {
                        item.I_start = ((ucalc[start] - ucalc[end])
                                         * item.Y                                         
                                         + ucalc[start] * item.Ysh / 2) / Math.Sqrt(3);
                        item.I_end = ((ucalc[end] - ucalc[start])
                                         * item.Y                                         
                                         + ucalc[end] * item.Ysh / 2) / Math.Sqrt(3);
                        item.S_start = ucalc[start] * item.I_start.Conjugate() * Math.Sqrt(3);                                            ;
                        item.S_end   = ucalc[end] * item.I_end.Conjugate() * Math.Sqrt (3);
                    }
                    //Transformers
                    else
                    {
                        if (grid.Nodes[start].Unom.Magnitude >= grid.Nodes[end].Unom.Magnitude)   // Unom_start >= Unom_end
                        {
                            item.I_start = ((ucalc[start] * kt - ucalc[end])
                                           * item.Y / kt
                                           + ucalc[start] * kt * item.Ysh / kt) / Math.Sqrt(3);

                            item.I_end = ((ucalc[end] / kt - ucalc[start])
                                           * item.Y / Complex.Conjugate(kt)) / Math.Sqrt(3);

                            item.S_start = ucalc[start] * item.I_start.Conjugate() * Math.Sqrt(3); ;
                            item.S_end   = ucalc[end] * item.I_end.Conjugate() * Math.Sqrt(3);
                        }
                        else if (grid.Nodes[start].Unom.Magnitude < grid.Nodes[end].Unom.Magnitude)  // Unom_start < Unom_end
                        {
                            item.I_start = ((ucalc[start] / kt - ucalc[end])
                                             * item.Y / Complex.Conjugate(kt)) / Math.Sqrt(3);

                            item.I_end = ((ucalc[end] * kt - ucalc[start])
                                           * item.Y / kt
                                           + ucalc[end] * kt * item.Ysh / kt) / Math.Sqrt(3);

                            item.S_start = ucalc[start] * item.I_start.Conjugate() * Math.Sqrt(3); ;
                            item.S_end   = ucalc[end] * item.I_end.Conjugate() * Math.Sqrt(3);
                        }
                    }
                }
            });
        }

    }
}
