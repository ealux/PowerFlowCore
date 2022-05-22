using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {      

        /// <summary>
        /// Show angle absolute difference on each branch
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="precision">Value precision</param>
        /// <returns><see cref="Vector{double}"/> of angle absolute differences</returns>
        public static Vector<double> GetAngleAbsoluteDifference(this Grid grid, uint precision = 2)
        {
            List<double> res = new List<double>();

            foreach (var branch in grid.Branches)
            {
                var start_ph = grid.Nodes.Where(n => n.Num == branch.Start).First().U.Phase * 180 / Math.PI;
                var end_ph   = grid.Nodes.Where(n => n.Num == branch.End).First().U.Phase * 180 / Math.PI;

                var diff = Math.Abs(Math.Round(start_ph - end_ph, (int)precision));                

                res.Add(diff);
            }            

            return Vector<double>.Build.DenseOfEnumerable(res);
        }
    }
}