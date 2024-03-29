﻿using PowerFlowCore.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        /// <returns><see cref="IEnumerable{double}"/> of differences</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<double> GetVoltageDifference(this Grid grid, uint precision = 2, bool inPercent = false)
        {
            IEnumerable<double> res;

            if (!inPercent)
                res = grid.Nodes.Select(n =>
                            Math.Round(n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude), (int)precision));
            else
                res = grid.Nodes.Select(n =>
                            Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)) * 100 / (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude), (int)precision));

            return res;
        }


        /// <summary>
        /// Find nodes where actual voltage is less then nominal one by a certain percentage
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="voltageRate">Voltage tolerance rate (voltageRate = 0.1 means 10% difference)</param>
        /// <returns>Collection of Nodes and differences in percentage if violation was found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(INode node, double diff)> CheckVoltageLack(this Grid grid, double voltageRate=0.1)
        {
            var k = 1 - Math.Abs(voltageRate);  // Set coef less then 100%
            var res = grid.Nodes.Where(n => ((n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude) * k - n.U.Magnitude) >= 0.0)
                                .Select(n => (n, Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)) * 100 / (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude), 2)));

            return res;
        }


        /// <summary>
        /// Find nodes where actual voltage is more then nominal one by a certain percentage
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <param name="voltageRate">Voltage tolerance rate (voltageRate = 0.1 means 10% difference)</param>
        /// <returns>Collection of Nodes and differences in percentage if violation was found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(INode node, double diff)> CheckVoltageOverflow(this Grid grid, double voltageRate=0.1)
        {
            var k = 1 + Math.Abs(voltageRate);  // Set coef over 100%
            var res = grid.Nodes.Where(n  => (n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude) * k) >= 0.0)
                                .Select(n => (n, Math.Round((n.U.Magnitude - (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)) * 100 / (n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude), 2)));

            return res;
        }


        #region Specific Node

        /// <summary>
        /// Find node where voltage level is minimal
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <returns>(Minimum voltage INode, Minimum voltage level [o.e.])</returns>    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (INode node, double diff) MinVoltageNode(this Grid grid)
        {
            var vlts = grid.Ucalc.Map(u => u.Magnitude)
                                 .Divide(VectorDouble.Create(grid.Nodes.Select(n => n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)));
            //var index = vlts.Substract(1).Map(v => Math.Round(v, 2)).MinimumIndex();
            var index = vlts.Substract(1).MinimumIndex();
            var vol = vlts.Minimum();
            var node = grid.Nodes[index];

            return (node, Math.Round(vol, 2));
        }


        /// <summary>
        /// Find node where voltage level is maximal
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <returns>(Maximum voltage INode, maximum voltage level [o.e.])</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (INode node, double diff) MaxVoltageNode(this Grid grid)
        {
            var vlts = grid.Ucalc.Map(u => u.Magnitude)
                                 .Divide(VectorDouble.Create(grid.Nodes.Select(n => n.Type == NodeType.PV ? n.Vpre : n.Unom.Magnitude)));
            //var index = vlts.Substract(1).Map(v => Math.Round(v, 2)).MaximumIndex();
            var index = vlts.Substract(1).MaximumIndex();
            var vol = vlts.Maximum();
            var node = grid.Nodes[index];

            return (node, Math.Round(vol, 2));
        }

        #endregion

    }
}