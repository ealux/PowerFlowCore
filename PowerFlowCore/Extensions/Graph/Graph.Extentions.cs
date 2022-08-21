using System;
using System.Linq;
using System.Collections.Generic;

using PowerFlowCore.Solvers;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Extension methods to work with <see cref="Grid"/> graphs
    /// </summary>
    public static class Graph
    {
        #region Connectivity

        /// <summary>
        /// Inspect <see cref="Grid"/> graph connectivity
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object</param>
        /// <returns><see cref="true"/> if connected, esle <see cref="false"/></returns>
        public static bool IsConnected(this Grid grid)
        {
            // Check nodes
            switch (grid.Nodes.Count)
            {
                case 0:
                    Logger.LogWarning($"Nodes count is equals 0!");
                    return false;
                case 1:
                    Logger.LogWarning($"Grid graph is not connected. Only one node is presented");
                    return false;
            }
            // Check branches
            if(grid.Branches.Count == 0)
            {
                Logger.LogWarning($"Branhces count is equals 0!");
                return false;
            }
            
            var branches = grid.Branches.Distinct(new BranchEqualityComparer()).OrderBy(b => b.Start).ToList(); // Unique branches list
            var exNodes  = grid.Nodes.OrderBy(n => n.Num).Select(n => n.Num).ToList();                          // Nodes list for excluding
            
            // If first node is orphan
            if (!branches.Any(b => (b.Start == exNodes[0]) | (b.End == exNodes[0])))
            {
                var orphans = string.Join(", ", FindOrphanNodes(grid).Select(n => n.Num));
                Logger.LogWarning($"Grid graph is not connected. Orphan node: {orphans}");
                return false;
            }                         

            List<int> linked = new List<int>(grid.Nodes.Count); // List for accepted nodes
            
            // Connected list for the first node
            foreach (var b in branches) 
            {
                if (b.Start == exNodes[0]) linked.Add(b.End); 
                else if (b.End == exNodes[0]) linked.Add(b.Start);
            } 
            // Remove first node from excluding list
            exNodes.Remove(exNodes[0]);

            // Run recursive finder
            RecurseConnectionFinder(linked, ref exNodes);

            if (exNodes.Count == 0)
                return true;

            var orphanNodes = string.Join(", ", FindOrphanNodes(grid).Select(n=>n.Num));
            Logger.LogWarning($"Grid graph is not connected. Orphan node: {orphanNodes}");
            return false;


            // Private recursive function for depth search
            void RecurseConnectionFinder(List<int> Linked, ref List<int> Exnodes)
            {
                foreach (int j in Linked)
                    if (Exnodes.Contains(j)) Exnodes.Remove(j);

                if (Exnodes.Count == 0) return;

                foreach (int link in Linked)
                {
                    var linker = new List<int>();

                    foreach (var b in branches!)
                    {
                        if (b.Start == link & Exnodes.Contains(b.End)) linker.Add(b.End);
                        else if (b.End == link & Exnodes.Contains(b.Start)) linker.Add(b.Start);
                    }

                    if (linker.Count != 0) RecurseConnectionFinder(new List<int>(linker), ref Exnodes);
                }
            }
        }

        /// <summary>
        /// Inspect <see cref="SolvableGrid"/> graphs on connectivity
        /// </summary>
        /// <param name="grid"><see cref="SolvableGrid"/> object</param>
        /// <returns><see cref="true"/> if connected, esle <see cref="false"/></returns>
        public static bool IsConnected(this SolvableGrid grid) => IsConnected(grid.Grid);

        #endregion

        #region Orphan Nodes

        /// <summary>
        /// Find all orphan nodes in <see cref="Grid"/>. If none - returns <see cref="IEnumerable{T}.Empty"/>
        /// </summary>
        /// <param name="grid"><see cref="Grid"/> object to find orphan nodes</param>
        /// <returns>Collection of (<see cref="INode"/> node, <see cref="int"/> NodeNumber)</returns>
        public static IEnumerable<(INode Node, int Num)> FindOrphanNodes(this Grid grid)
        {
            // Check nodes
            switch (grid.Nodes.Count)
            {
                case 0:
                    Logger.LogWarning($"Nodes count is equals 0!");
                    return Enumerable.Empty<(INode Node, int num)>();
                case 1:
                    Logger.LogWarning($"Grid graph is not connected. Only one node is presented");
                    return new List<(INode Node, int num)>() { (grid.Nodes[0], grid.Nodes[0].Num) }.AsEnumerable();
            }
            // Check branches
            if (grid.Branches.Count == 0)
            {
                Logger.LogWarning($"Branhces count is equals 0!");
                var outList = new List<(INode Node, int num)>();
                foreach (var item in grid.Nodes)
                    outList.Add((item, item.Num));
                return outList.AsEnumerable();
            }

            // Branches unique nums (from start and end)
            var branchesStart = grid.Branches.Select(b => b.Start).Concat(grid.Branches.Select(b => b.End)).Distinct();
            // Nodes list for excluding
            var nodes = grid.Nodes.Select(n=>n.Num).Except(branchesStart).ToList();



            if (nodes.Count > 0)
            {
                var resultOut = new List<(INode Node, int num)>();
                foreach (var item in nodes)
                    resultOut.Add(grid.Nodes.Where(n => n.Num == item).Select(n => (n, n.Num)).First());
                return resultOut.AsEnumerable();
            }

            return Enumerable.Empty<(INode Node, int num)>();
        }

        /// <summary>
        /// Find all orphan nodes in <see cref="Grid"/>. If none - returns <see cref="IEnumerable{T}.Empty"/>
        /// </summary>
        /// <param name="grid"><see cref="SolvableGrid"/> object to find orphan nodes</param>
        /// <returns>Collection of (<see cref="INode"/> node, <see cref="int"/> NodeNumber)</returns>
        public static IEnumerable<(INode Node, int Num)> FindOrphanNodes(this SolvableGrid grid) => FindOrphanNodes(grid.Grid);

        #endregion
    }

    #region [IEqualityComparer interface block]

    /// <summary>
    /// IEqualityComparer for <see cref="IBranch"/>
    /// </summary>
    internal class BranchEqualityComparer : IEqualityComparer<IBranch>
    {
        public bool Equals(IBranch b1, IBranch b2)
        {
            if (b1 == null || b2 == null) return false;
            return (b1.Start == b2.Start) & (b1.End == b2.End);
        }

        public int GetHashCode(IBranch obj)
        {
            return obj.Start.GetHashCode() ^ obj.End.GetHashCode() ^ obj.GetType().GetHashCode();
        }
    }

    #endregion [IEqualityComparer interface block]
}
