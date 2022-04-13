using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using Complex = System.Numerics.Complex;
using PowerFlowCore.Data;

namespace PowerFlowCore.Solvers
{
    public static partial class Solvers
    {
        #region [Checks]

        /// <summary>
        /// Check for tolerance on initial and calculated voltage
        /// </summary>
        /// <param name="U_nominal">Vector complex -> nominal voltages</param>
        /// <param name="U">Vector complex -> current iteration calculated voltages</param>
        /// <param name="grid">Grid object</param>
        /// <param name="voltageRate">Voltage difference rate (voltageRate=0.5 -> ± 50% difference)</param>
        private static void CheckVoltage(Vector<Complex> U_nominal, 
                                         Vector<Complex> U,
                                         Grid grid,
                                         double voltageRate)
        {
            var init = U_nominal.Map(vol => vol.Magnitude);
            var u = U.Map(vol => vol.Magnitude);

            var init_max = u * (1 + Math.Abs(voltageRate));
            var init_min = u * (1 - Math.Abs(voltageRate));

            var diff_max = init_max - init;    //normal - when only positives
            var diff_min = init_min - init;    //normal - when only negatives

            if (diff_max.Any(i => i < 0))
            {
                for (int i = 0; i < diff_max.Count; i++)
                {
                    if (diff_max[i] < 0) throw new VoltageLackException(grid.Nodes[i].Num.ToString());
                }

            }
            else if (diff_min.Any(i => i > 0))
            {
                for (int i = 0; i < diff_min.Count; i++)
                {
                    if (diff_min[i] > 0)
                        throw new VoltageOverflowException(grid.Nodes[i].Num.ToString());
                }
            }
        }

        #endregion [Checks]
    }
}
