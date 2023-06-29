using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Validate <see cref="Grid"/> branches on null, start/end nodes existence and Ktr value
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <returns><see cref="true"/> if all are valid</returns>
        internal static bool ValidateBranches(this Grid grid)
        {
            _ = grid ?? throw new ArgumentNullException(nameof(grid));

            bool success = true;

            // Null branches
            if (grid.Branches.Any(n => n == null))
            {
                Logger.LogCritical("One or several branches has no data");
                success = false;
            }

            Parallel.ForEach(grid.Branches, branch =>
            {
                // Nodes existence
                var start = branch.Start;
                var end = branch.End;
                if (!grid.Nodes.Any(n => n.Num == start))
                {
                    Logger.LogCritical($"Branch {branch.Start}-{branch.End} start node does not existing! Check the input data!");
                    success = false;
                }
                if (!grid.Nodes.Any(n => n.Num == end))
                {
                    Logger.LogCritical($"Branch {branch.Start}-{branch.End} end node does not existing! Check the input data!");
                    success = false;
                }

                // Ktr validation
                if (branch.Ktr.Magnitude < 0.0)
                {
                    Logger.LogWarning($"Branch {branch.Start}-{branch.End} Ktr value less then zero. Ktr = {Math.Round(branch.Ktr.Magnitude, 6)}! " +
                                        $"Check the input data!");
                    success = false;
                }
            });

            return success;
        }

        /// <summary>
        /// Validate <see cref="Grid"/> branches on null, start/end nodes existence and Ktr value
        /// </summary>
        /// <returns><see cref="true"/> if all are valid</returns>
        internal static bool ValidateBranches(IEnumerable<INode> nodes, IEnumerable<IBranch> branches)
        {
            _ = nodes ?? throw new ArgumentNullException(nameof(nodes));
            _ = branches ?? throw new ArgumentNullException(nameof(branches));

            bool success = true;

            // Null branches
            if (branches.Any(n => n == null))
            {
                Logger.LogCritical("One or several branches has no data");
                success = false;
            }

            var nlist = nodes.Select(n => n.Num).ToArray();
            Array.Sort(nlist);

            foreach (var branch in branches)
            {
                // Nodes existence
                var start = branch.Start;
                var end = branch.End;

                var is_start = Array.BinarySearch(nlist, start);
                var is_end = Array.BinarySearch(nlist, end);

                if (is_start <= -1)
                {
                    Logger.LogCritical($"Branch {branch.Start}-{branch.End} start node does not existing! Check the input data!");
                    success = false;
                }
                if (is_end <= -1)
                {
                    Logger.LogCritical($"Branch {branch.Start}-{branch.End} end node does not existing! Check the input data!");
                    success = false;
                }

                // Ktr validation
                if (branch.Ktr.Magnitude < 0.0)
                {
                    Logger.LogWarning($"Branch {branch.Start}-{branch.End} Ktr value less then zero. Ktr = {Math.Round(branch.Ktr.Magnitude, 6)}! " +
                                        $"Check the input data!");
                    success = false;
                }
            }

            return success;
        }
    }
}
