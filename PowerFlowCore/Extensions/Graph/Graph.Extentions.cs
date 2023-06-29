using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Extension methods to work with <see cref="Grid"/> graphs
    /// </summary>
    public static class Graph
    {
        #region Connectivity

        /// <summary>
        /// Inspect <see cref="Grid"/> graph connectivity. If not connected - show islands.
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
            if (grid.Branches.Count == 0)
            {
                Logger.LogWarning($"Branhces count is equals 0!");
                return false;
            }

            var components = new List<List<int>>();
            var visited = new HashSet<int>();

            // Get connected components using BFS
            for (int i = 0; i < grid.Nodes.Count; i++)
            {
                if (!visited.Contains(i))
                    components.Add(BFSConnectedComponent(grid, i, ref visited));
            }

            if (components.Count > 1)
            {
                Logger.LogWarning($"Grid has {components.Count} independent islands:");
                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i].Count > 4)
                    {
                        Logger.LogWarning($"Island {i + 1}: {components[i].Count} nodes ({grid.Nodes[components[i][0]].Num}, {grid.Nodes[components[i][1]].Num} ... {grid.Nodes[components[i][components[i].Count - 1]].Num})");
                    }
                    else
                    {
                        var nodes = components[i].Select(i => grid.Nodes[i].Num).ToArray();
                        Logger.LogWarning($"Island {i + 1}: {components[i].Count} nodes ({string.Join(", ", nodes)})");
                    }
                }
                Logger.LogCritical($"Grid graph is not connected.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return the connected components in grid as collection of grids.
        /// Each grid is connected component.
        /// </summary>
        public static IEnumerable<Grid> GetGridIslands(this Grid grid)
        {
            var components = new List<List<int>>();
            var visited = new HashSet<int>();

            // Validate the graph parameter
            _ = grid ?? throw new ArgumentNullException(nameof(grid));

            // Check nodes
            switch (grid.Nodes.Count)
            {
                case 0:
                    Logger.LogWarning($"Nodes count is equals 0!");
                    return Enumerable.Empty<Grid>();

                case 1:
                    Logger.LogWarning($"Grid graph is not connected. Only one node is presented");
                    return Enumerable.Empty<Grid>();
            }
            if (grid.Branches.Count == 0)
            {
                Logger.LogWarning($"Branhces count is equals 0!");
                return Enumerable.Empty<Grid>();
            }

            // Get connected components using BFS
            for (int i = 0; i < grid.Nodes.Count; i++)
            {
                if (!visited.Contains(i))
                    components.Add(BFSConnectedComponent(grid, i, ref visited));
            }

            var res = new List<Grid>(components.Count);

            for (int i = 0; i < components.Count; i++)
            {
                var nodes = components[i].Select(num => grid.Nodes[num]).ToArray();
                var nums = nodes.Select(n => n.Num).ToArray();
                var branches = grid.Branches.Where(br => nums.Contains(br.Start) || nums.Contains(br.End)).ToArray();

                res.Add(new Grid(nodes, branches));
                if (res[i].Nodes == null)
                {
                    if (components[i].Count > 4)
                    {
                        Logger.LogCritical($"Check the input data on island {i + 1}! (nodes: {grid.Nodes[components[i][0]].Num}, {grid.Nodes[components[i][1]].Num} ... {grid.Nodes[components[i][components[i].Count - 1]].Num})");
                    }
                    else
                    {
                        var IslandNodes = components[i].Select(i => grid.Nodes[i].Num).ToArray();
                        Logger.LogWarning($"Check the input data on island {i + 1}! ({string.Join(", ", IslandNodes)})");
                    }
                }
            }

            return res;
        }

        #region [Private methods]

        /// <summary>
        /// Breadth first search grid graph
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/></param>
        /// <returns>List of visited Nodes indexes</returns>
        private static List<int> BreadthFirstWalk(Grid grid)
        {
            var visited = new HashSet<int>() { 0 };
            var queue = new Queue<int>();
            var listOfNodes = new List<int>(grid.Nodes.Count) { 0 };

            queue.Enqueue(0);

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                foreach (var adjacent in Neighbours(grid, current))
                {
                    if (!visited.Contains(adjacent))
                    {
                        listOfNodes.Add(adjacent);
                        visited.Add(adjacent);
                        queue.Enqueue(adjacent);
                    }
                }
            }

            return listOfNodes;
        }

        /// <summary>
        /// Look neighbours of node by its index in list
        /// </summary>
        /// <param name="grid">Graph to search with</param>
        /// <param name="nind">Index of Node in nodes list</param>
        /// <returns>List of neighbours nodes indexes</returns>
        private static int[] Neighbours(Grid grid, int nind)
        {
            var neighbours = new int[grid.Ysp.RowPtr[nind + 1] - grid.Ysp.RowPtr[nind] - 1];

            for (int j = 0, i = grid.Ysp.RowPtr[nind]; i < grid.Ysp.RowPtr[nind + 1]; i++)
            {
                if (nind != grid.Ysp.ColIndex[i])
                    neighbours[j++] = grid.Ysp.ColIndex[i];
            }

            return neighbours;
        }

        /// <summary>
        /// Find a connected component and return from a source vertex in a graph.
        /// </summary>
        private static List<int> BFSConnectedComponent(Grid grid, int nind, ref HashSet<int> visited)
        {
            var component = new List<int>();
            var queue = new Queue<int>();

            queue.Enqueue(nind);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!visited.Contains(current))
                {
                    component.Add(current);
                    visited.Add(current);

                    foreach (var adjacent in Neighbours(grid, current))
                        if (!visited.Contains(adjacent))
                            queue.Enqueue(adjacent);
                }
            }

            return component;
        }

        #endregion [Private methods]

        #endregion Connectivity

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
            var nodes = grid.Nodes.Select(n => n.Num).Except(branchesStart).ToList();

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
        /// Find all orphan nodes from input enumerable nodes and branches. If none - returns <see cref="IEnumerable{T}.Empty"/>
        /// </summary>
        /// <returns>Collection of Node numbers</returns>
        public static IEnumerable<int> FindOrphanNodes(IEnumerable<INode> nodes, IEnumerable<IBranch> branches)
        {
            // Check nodes
            var renodes = nodes.ToArray();

            switch (renodes.Length)
            {
                case 0:
                    Logger.LogWarning($"Nodes count is equals 0!");
                    return Enumerable.Empty<int>();

                case 1:
                    Logger.LogWarning($"Grid graph is not connected. Only one node is presented");
                    return renodes.Select(n => n.Num).AsEnumerable();
            }
            // Check branches
            if (branches.Count() == 0)
            {
                Logger.LogWarning($"Branhces count is equals 0!");
                return renodes.Select(n => n.Num).AsEnumerable();
            }

            // Nodes list for excluding
            var new_nodes = renodes.Select(n => n.Num)
                                   .Except(branches.Select(b => b.Start)
                                                   .Concat(branches.Select(b => b.End))
                                                   .Distinct());

            if (new_nodes.Any())
                return new_nodes.AsEnumerable();

            return Enumerable.Empty<int>();
        }

        #endregion Orphan Nodes
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