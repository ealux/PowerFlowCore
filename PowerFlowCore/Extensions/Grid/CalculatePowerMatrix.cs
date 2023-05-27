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
                grid.Nodes[pos].S_gen = s.Conjugate();
            }

            // Current and power flows in branches

            Parallel.ForEach(grid.Branches, item =>
            {
                //foreach (var item in grid.Branches)
                {
                    var start = item.Start_calc;
                    var end = item.End_calc;
                    var kt = item.Ktr.Magnitude <= 0 ? Complex.One : item.Ktr;

                    //Lines or Breakers
                    if (kt == 1.0)
                    {
                        item.I_start = (ucalc[start] - ucalc[end])
                                         * item.Y
                                         / Math.Sqrt(3);
                        item.I_end = (ucalc[end] - ucalc[start])
                                         * item.Y
                                         / Math.Sqrt(3);
                        item.S_start = Math.Sqrt(3)
                                         * ucalc[start]
                                         * item.I_start.Conjugate();
                        item.S_end = Math.Sqrt(3)
                                         * ucalc[end]
                                         * item.I_end.Conjugate();
                    }
                    //Transformers
                    else
                    {
                        if (grid.Nodes[start].Unom.Magnitude >= grid.Nodes[end].Unom.Magnitude)   // Unom_start >= Unom_end
                        {
                            item.I_start = (ucalc[start] * kt - ucalc[end])
                                             * item.Y / kt
                                             / Math.Sqrt(3);
                            item.I_end = (ucalc[end] / kt - ucalc[start])
                                             * item.Y / Complex.Conjugate(kt)
                                             / Math.Sqrt(3);
                            item.S_start = Math.Sqrt(3)
                                             * ucalc[start]
                                             * item.I_start.Conjugate();
                            item.S_end = Math.Sqrt(3)
                                             * ucalc[end]
                                             * item.I_end.Conjugate();
                        }
                        else if (grid.Nodes[start].Unom.Magnitude < grid.Nodes[end].Unom.Magnitude)  // Unom_start < Unom_end
                        {
                            item.I_start = (ucalc[start] / kt - ucalc[end])
                                             * item.Y / Complex.Conjugate(kt)
                                             / Math.Sqrt(3);
                            item.I_end = (ucalc[end] * kt - ucalc[start])
                                             * item.Y / kt
                                             / Math.Sqrt(3);
                            item.S_start = Math.Sqrt(3)
                                             * ucalc[start]
                                             * item.I_start.Conjugate();
                            item.S_end = Math.Sqrt(3)
                                             * ucalc[end]
                                             * item.I_end.Conjugate();
                        }
                    }
                }
            });
        }

    }
}
