using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using Complex = System.Numerics.Complex;
using PowerFlowCore.Data;

namespace PowerFlowCore.Data
{
    public static partial class Solvers
    {
        #region [Checks]

        /// <summary>
        /// Find nodes where actual voltage is more then nominal one by a certain percentage
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="voltageRate">Voltage tolerance rate (voltageRate = 0.1 means 10% difference)</param>
        /// <returns>Collection of Nodes and differences in percentage if violation was found</returns>
        public static IEnumerable<(INode, double)> CheckVoltageOverflow(this Grid grid, double voltageRate)
        {
            var k = 1 + Math.Abs(voltageRate);
            var res = grid.Nodes.Where(n => (n.U.Magnitude - n.Unom.Magnitude * k) >= 0.0)
                                .Select(n => (n, Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)) * 100 / n.Unom.Magnitude, 2))).ToList();

            return res;
        }


        /// <summary>
        /// Find nodes where actual voltage is less then nominal one by a certain percentage
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="voltageRate">Voltage tolerance rate (voltageRate = 0.1 means 10% difference)</param>
        /// <returns>Collection of Nodes and differences in percentage if violation was found</returns>
        public static List<(INode, double)> CheckVoltageLack(this Grid grid, double voltageRate)
        {
            var k = 1 - Math.Abs(voltageRate);
            var res = grid.Nodes.Where(n => (n.Unom.Magnitude * k - n.U.Magnitude) >= 0.0)
                                .Select(n => (n, Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)) * 100 / n.Unom.Magnitude, 2))).ToList();

            return res;
        }


        /// <summary>
        /// Show difference between actual and nominal voltages
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="precision">Value precision</param>
        /// <param name="inPercent">Show result in percent</param>
        /// <returns><see cref="Vector{double}"/> of differences</returns>
        public static Vector<double> GetVoltageDifference(this Grid grid, uint precision = 2, bool inPercent=true)
        {
            Vector<double> res;

            if(!inPercent)
                res = Vector<double>.Build.DenseOfEnumerable(grid.Nodes.Select(n => 
                            Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)), (int)precision)));
            else
                res = Vector<double>.Build.DenseOfEnumerable(grid.Nodes.Select(n => 
                            Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)) * 100 / n.Unom.Magnitude, (int)precision)));

            return res;
        }




        #endregion [Checks]
    }
}
