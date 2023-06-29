using System;
using System.Collections.Generic;
using System.Linq;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Validate input nodes on null and invalid Unom
        /// </summary>
        /// <param name="nodes">Input <see cref="IEnumerable{T}"/> collection of INodes</param>
        /// <returns><see cref="true"/> if all are valid</returns>
        private static bool ValidateNodes(IEnumerable<INode> nodes, IEnumerable<IBranch> branches)
        {
            _ = nodes ?? throw new ArgumentNullException(nameof(nodes));
            _ = branches ?? throw new ArgumentNullException(nameof(branches));

            bool success = true;

            // Null nodes
            if (nodes.Any(n => n == null))
            {
                Logger.LogCritical("One or several nodes has no data");
                success = false;
            }

            // No slack nodes
            if (!nodes.Any(n => n.Type == NodeType.Slack))
            {
                Logger.LogCritical("No slack node!");
                success = false;
            }

            // Orphans
            var orphans = string.Join(", ", Graph.FindOrphanNodes(nodes, branches));
            if(orphans.Length > 0)
            {
                Logger.LogWarning($"Orphan node: {orphans}");
                success = false;
            }

            // Zero or less Unom
            foreach (var node in nodes)
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
