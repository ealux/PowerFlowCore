﻿using System;
using System.Linq;
using MathNet.Numerics;
using PowerFlowCore.Data;

using Complex = System.Numerics.Complex;

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

            bool nodesSuccess = true;
            bool branchesSuccess = true;
            bool success = true;

            // Validate Nodes and Branches
            nodesSuccess    = grid.ValidateNodes();
            branchesSuccess = grid.ValidateBranches();

            if (!nodesSuccess || !branchesSuccess)
                success = false;

            return success;
        }
    }
}
