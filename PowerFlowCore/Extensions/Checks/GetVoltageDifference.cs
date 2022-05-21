using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Show difference between actual and nominal voltages
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="precision">Value precision</param>
        /// <param name="inPercent">Show result in percent</param>
        /// <returns><see cref="Vector{double}"/> of differences</returns>
        public static Vector<double> GetVoltageDifference(this Grid grid, uint precision = 2, bool inPercent = true)
        {
            Vector<double> res;

            if (!inPercent)
                res = Vector<double>.Build.DenseOfEnumerable(grid.Nodes.Select(n =>
                            Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)), (int)precision)));
            else
                res = Vector<double>.Build.DenseOfEnumerable(grid.Nodes.Select(n =>
                            Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)) * 100 / n.Unom.Magnitude, (int)precision)));

            return res;
        }
    }
}