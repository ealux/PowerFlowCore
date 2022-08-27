using System;
using System.Linq;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Validate <see cref="Grid"/> branches on null, start/end nodes existence and Ktr value
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <returns><see cref="true"/> if all are valid</returns>
        private static bool ValidateBranches(this Grid grid)
        {
            _ = grid ?? throw new ArgumentNullException(nameof(grid));

            bool success = true;

            // Null branches
            if (grid.Branches.Any(n => n == null))
            {
                Logger.LogCritical("One or several branches has no data");
                success = false;
            }

            // Branch checks
            foreach (var branch in grid.Branches)
            {
                // Nodes existence
                var start = branch.Start;
                var end = branch.End;
                if(!grid.Nodes.Any(n => n.Num == start))
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
            }

            return success;
        }
    }
}
