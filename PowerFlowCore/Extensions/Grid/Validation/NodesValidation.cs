using System;
using System.Linq;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Validate <see cref="Grid"/> nodes on null and invalid Unom
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <returns><see cref="true"/> if all are valid</returns>
        private static bool ValidateNodes(this Grid grid)
        {
            _ = grid ?? throw new ArgumentNullException(nameof(grid));

            bool success = true;

            // Null nodes
            if (grid.Nodes.Any(n => n == null))
            {
                Logger.LogCritical("One or several nodes has no data");
                success = false;
            }

            // Zero or less Unom
            foreach (var node in grid.Nodes)
            {
                if (node.Unom == Complex.Zero || node.Unom.Magnitude < 0.0)
                {
                    Logger.LogWarning($"Node {node.Num} Unom value is equal zero or less");
                    success = false;
                }
            }                

            return success;
        }
    }
}
