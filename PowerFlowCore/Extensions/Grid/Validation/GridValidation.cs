﻿using System;
using System.Collections.Generic;

namespace PowerFlowCore.Data
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Validate <see cref="Grid"/> Nodes and Branches
        /// </summary>
        /// <param name="grid">input <see cref="Grid"/> object</param>
        /// <returns><see cref="true"/> if valid</returns>
        public static bool Validate(this Grid grid)
        {
            _ = grid ?? throw new ArgumentNullException(nameof(grid));

            if (grid.Nodes == null || grid.Branches == null)
                return false;

            bool isConnected = true;

            // Conectivity
            isConnected  = grid.IsConnected();

            return isConnected;
        }

        /// <summary>
        /// Validate <see cref="Grid"/> Nodes and Branches
        /// </summary>
        /// <param name="grid">input <see cref="Grid"/> object</param>
        /// <returns><see cref="true"/> if valid</returns>
        public static bool ValidateInput(IEnumerable<INode> nodes, IEnumerable<IBranch> branches)
        {
            _ = nodes ?? throw new ArgumentNullException(nameof(nodes));
            _ = branches ?? throw new ArgumentNullException(nameof(branches));

            bool nodesSuccess = true;
            bool branchesSuccess = true;

            bool success = true;

            // Validate Nodes and Branches. Conectivity
            nodesSuccess = ValidateNodes(nodes);
            branchesSuccess = ValidateBranches(nodes, branches);

            if (!nodesSuccess || !branchesSuccess)
                success = false;

            return success;
        }
    }
}
