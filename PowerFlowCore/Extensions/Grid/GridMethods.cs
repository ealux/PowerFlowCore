using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Methods for Grid object extension
    /// </summary>
    public static partial class ExtensionsMethods
    {
        /// <summary>
        /// Make full copy of <see cref="Grid"/> object
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <returns>New <see cref="Grid"/> object</returns>
        public static Grid DeepCopy(this Grid grid)
        {           
            // Create empty lists for nodes and branches
            var nodes = new List<INode>(grid.Nodes.Count);         
            var branches = new List<IBranch>(grid.Branches.Count);

            //Fill new Nodes
            foreach (var node in grid.Nodes)
            {
                nodes.Add((INode)(new Node()
                {
                    Num      = node.Num,
                    Num_calc = node.Num_calc,
                    Q_max    = node.Q_max,
                    Q_min    = node.Q_min,
                    S_gen    = node.S_gen,
                    S_load   = node.S_load,
                    Type     = node.Type,
                    U        = node.U,
                    Unom     = node.Unom,
                    Vpre     = node.Vpre,
                    Ysh      = node.Ysh
                }));
            }
            //Fill new Branches
            foreach (var branch in grid.Branches)
            {
                branches.Add((IBranch)(new Branch()
                {
                    Count       = branch.Count,
                    Start       = branch.Start,
                    End         = branch.End,
                    Start_calc  = branch.Start_calc,
                    End_calc    = branch.End_calc,
                    Y           = branch.Y,
                    Ysh         = branch.Ysh,
                    Ktr         = branch.Ktr,
                    I_start     = branch.I_start,
                    I_end       = branch.I_end,
                    S_start     = branch.S_start,
                    S_end       = branch.S_end,
                }));
            }

            Grid new_grid = new Grid(nodes: nodes, branches: branches); // Create new Grid object

            new_grid.Uinit      = grid.Uinit?.Clone() ?? new_grid.Unominal; // Uinit vector 

            //Statistic
            new_grid.PQ_Count    = grid.PQ_Count;
            new_grid.PV_Count    = grid.PV_Count;
            new_grid.Slack_Count = grid.Slack_Count;

            return new_grid;
        }


        /// <summary>
        /// Set custom voltage initial vector
        /// </summary>
        /// <param name="grid">Input <see cref="Grid"/> object</param>
        /// <param name="Uinit">Input <see cref="Vector{Complex}"/> of Uinit valus</param>
        /// <returns>Modified <see cref="Grid"/> object</returns>
        public static Grid SetUinit(this Grid grid, Vector<Complex> Uinit = null)
        {
            // If grid Uinit doesn't exist
            if (grid.Uinit == null)
                grid.Uinit = Vector<Complex>.Build.Dense(grid.Unominal.Count);

            // If input Uinit is null or broken
            if ((Uinit == null) || (Uinit.Count != grid.Unominal.Count))
            {
                for (int i = 0; i < grid.Nodes.Count; i++)
                {
                    switch (grid.Nodes[i].Type)
                    {
                        case NodeType.Slack:
                        case NodeType.PQ:
                            grid.Uinit[i] = grid.Unominal[i];       // Slack and PQ nodes = nominal
                            break;
                        case NodeType.PV:
                            grid.Uinit[i] = grid.Nodes[i].Vpre;     // PV nodes = Vpre
                            break;
                    }
                }
            }
            // If input is normal
            else
                Uinit.CopyTo(grid.Uinit);

            return grid;
        }

    }   


    #region [Helpers classes Node and Branch]

    /// <summary>
    /// Node realese for Healpers
    /// </summary>
    internal sealed class Node : INode
    {
        public int Num { get; set; }
        public int Num_calc { get; set; }
        public NodeType Type { get; set; }
        public Complex U { get; set; }
        public double Vpre { get; set; }
        public Complex Unom { get; set; }
        public Complex S_load { get; set; }
        public int? LoadModelNum { get; set; }
        public Complex S_calc { get; set; }
        public Complex S_gen { get; set; }
        public double? Q_min { get; set; }
        public double? Q_max { get; set; }
        public Complex Ysh { get; set; }
    }

    /// <summary>
    /// Branch realese for Healpers
    /// </summary>
    internal sealed class Branch : IBranch
    {
        public int Start { get; set; }
        public int Start_calc { get; set; }
        public int End { get; set; }
        public int End_calc { get; set; }
        public Complex Y { get; set; }
        public Complex Ysh { get; set; }
        public int Count { get; set; }
        public Complex Ktr { get; set; }
        public Complex S_start { get; set; }
        public Complex S_end { get; set; }
        public Complex I_start { get; set; }
        public Complex I_end { get; set; }
    }

    #endregion
}
