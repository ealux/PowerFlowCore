using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
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
    }
}