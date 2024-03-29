﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Static class to check for breakers in <see cref="Grid"/> and set template impedance value
    /// </summary>
    internal static class BreakersTemplate
    {
        /// <summary>
        /// Find breakers (low or zero impedance) and set template impedance value
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool SetBreakers(this Grid grid)
        {
            // Check for transformes value
            foreach (var item in grid.Branches.Where(b => b.Ktr.Magnitude > 0 & b.Ktr.Magnitude != Complex.One))
                if (1 / item.Y.Magnitude <= 0.001 || item.Y.Magnitude == 0.0)
                {
                    Logger.LogCritical($"Transformer branch {item.Start}-{item.End} has around or zero impedance! Check the input data!");
                    return false;
                }

            // Find non-transformers branches
            var brs = grid.Branches
                          .Where(b => (b.Ktr.Magnitude <= 0 | b.Ktr.Magnitude == Complex.One) && (1 / b.Y.Magnitude <= 0.0001 || b.Y.Magnitude == 0.0));

            // No low impedance branches
            if (!brs.Any()) 
                return true;

            // Iterate branches
            foreach (var item in brs)
            {
                var start = grid.Nodes.Where(n => n.Num == item.Start).FirstOrDefault();
                var end = grid.Nodes.Where(n => n.Num == item.End).FirstOrDefault();

                // Check nodes existatnce
                if (start == null)
                {
                    Logger.LogCritical($"Branch {item.Start}-{item.End} start node does not existing! Check the input data!");
                    return false;
                }
                else if (end == null)
                {
                    Logger.LogCritical($"Branch {item.Start}-{item.End} end node does not existing! Check the input data!");
                    return false;
                }

                // Set new impedance
                //item.Y = 1 / Complex.FromPolarCoordinates(0.00044 * start.Unom.Magnitude, 0.0);

                // Impedance from linear model of handbook breakers data
                item.Y = 1 / new Complex(1.3338e-5 * start.Unom.Magnitude + 1.058e-4, 0.0);

                // Impedance from default common guess (e.g. matlab)
                //item.Y = 1 / new Complex(0.001, 0.0);
            }

            return true;
        }
    }

}
