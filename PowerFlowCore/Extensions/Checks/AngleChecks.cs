using PowerFlowCore.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {

        /// <summary>
        /// Show angle absolute difference on each branch
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="precision">Value precision</param>
        /// <returns><see cref="double[]"/> of angle absolute differences</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] GetAngleAbsoluteDifference(this Grid grid, uint precision = 2)
        {
            List<double> res = new List<double>();

            foreach (var branch in grid.Branches)
            {
                var start_ph = grid.Nodes.Where(n => n.Num == branch.Start).First().U.Phase * 180 / Math.PI;
                var end_ph   = grid.Nodes.Where(n => n.Num == branch.End).First().U.Phase * 180 / Math.PI;

                var diff = Math.Abs(Math.Round(start_ph - end_ph, (int)precision));                

                res.Add(diff);
            }            

            return VectorDouble.Create(res);
        }

        /// <summary>
        /// Find branch with maximum value of difference between voltage angles in start and end nodes
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <returns><see cref="Tuple{T1, T2}"/> of (Maximum angle IBranch, Maximum difference value [deg])</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (IBranch, double) MaxAngleBranch(this Grid grid)
        {
            var diff   = GetAngleAbsoluteDifference(grid, 5);

            var index  = diff.MaximumIndex();
            var angle  = diff.Maximum();
            var branch = grid.Branches[index];

            return (branch, Math.Round(angle, 2));
        }
    }
}