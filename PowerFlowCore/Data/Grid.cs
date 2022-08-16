using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// PF-problem basic Grid object from network topology and characteristics
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Collection of <see cref="INode">
        /// <para>Sorted from PQ, through PV nodes, up to Slack nodes at the end</para> 
        /// <para>Also sorted by nominal voltage level (magnitude)</para>
        /// </summary>
        public List<INode> Nodes { get; set; }

        /// <summary>
        /// Collection of <see cref="IBranch"/>
        /// </summary>
        public List<IBranch> Branches { get; set; }


        /// <summary>
        /// Admittance matrix
        /// </summary>
        public Matrix<Complex> Y { get; set; }



        /// <summary>
        /// Vector of nominal Voltages
        /// </summary>
        public Vector<Complex> Unominal => Vector<Complex>.Build.Dense(this.Nodes.Count, (i) => this.Nodes[i].Unom);

        /// <summary>
        /// Vector of Initial Voltages for iteration procedure start
        /// </summary>
        public Vector<Complex> Uinit { get; set; }

        /// <summary>
        /// Vector of calculated Voltages
        /// </summary>
        public Vector<Complex> Ucalc => Vector<Complex>.Build.Dense(this.Nodes.Count, (i) => this.Nodes[i].U);



        /// <summary>
        /// Vector of Powers in Nodes (Generation - Load)
        /// </summary>
        public Vector<Complex> S => Vector<Complex>.Build.Dense(this.Nodes.Count, (i) => this.Nodes[i].S_gen - this.Nodes[i].S_calc);


        /// <summary>
        /// Collection of Static Load Models to be applied
        /// </summary>
        public Dictionary<int, CompositeLoadModel> LoadModels { get; set; } = new Dictionary<int, CompositeLoadModel>();


        /// <summary>
        /// PQ nodes amount
        /// </summary>
        public int PQ_Count { get; set; }

        /// <summary>
        /// PV nodes amount
        /// </summary>
        public int PV_Count { get; set; }

        /// <summary>
        /// Slack nodes amount
        /// </summary>
        public int Slack_Count { get; set; }

        /// <summary>
        /// Grid identifier
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Private ctor
        /// </summary>
        private Grid() { }

        /// <summary>
        /// Calculate initial parameters for Power Flow task computation based on network topology and characteristics
        /// </summary>
        /// <param name="nodes">Enumerable source of <see cref="INode"/> collection</param>
        /// <param name="branches">Enumerable source of <see cref="IBranch"/> collection</param>
        public Grid(IEnumerable<INode> nodes, IEnumerable<IBranch> branches)
        {
            this.Id = Guid.NewGuid().ToString();    // Set id
            InitParameters(nodes, branches);        // Create grid
        }

        /// <summary>
        /// Calculate initial parameters for Power Flow task computation based on network topology and characteristics
        /// </summary>
        /// <param name="converter"><see cref="IConverter"/> object that incupsulate <see cref="IEnumerable{T}"/> Nodes and Branches</param>
        public Grid(IConverter converter) : this(converter.Nodes, converter.Branches) { }


        #region [Build Scheme]

        /// <summary>
        /// Initialize (or re-initialize) Grid parameters
        /// </summary>
        /// <param name="nodes">Enumerable source of raw-view Nodes</param>
        /// <param name="branches">Enumerable source of raw-view Branches</param>
        internal void InitParameters(IEnumerable<INode> nodes, 
                                     IEnumerable<IBranch> branches, 
                                     bool setInitialByNominal = true)
        {
            //Initial calc
            ReBuildNodesBranches(renodes: nodes, rebranches: branches);


            //S and Uinit vectors filling; PQ, PV and Slack nodes count    
            this.Uinit    = Vector<Complex>.Build.Dense(this.Nodes.Count);

            // Reset PV nodes to PQ modes
            foreach (var node in Nodes)
            {
                if (node.Type == NodeType.PV)
                {
                    var vpreN = node.Vpre == 0.0;

                    if (vpreN)
                    {
                        node.Type = NodeType.PQ;
                        ReBuildNodesBranches(renodes: nodes, rebranches: branches); //Rebuilding Nodes
                        continue;
                    }
                }
            }

            //Count nodes and Set initial voltages
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                var node = this.Nodes[i];
                if(node.S_calc == Complex.Zero) node.S_calc = node.S_load;  // Set load for calculus

                switch (node.Type)
                {                                      
                    case NodeType.PQ:
                        this.PQ_Count++;
                        if (setInitialByNominal == true) Uinit[i] = node.Unom;  //PQ-type case: Inital voltage is equal to nominal voltage level
                        break;
                    case NodeType.PV:
                        this.PV_Count++;
                        if (setInitialByNominal == true) Uinit[i] = node.Vpre;  //PV-type case: Inital voltage is equal to user-preset voltage level
                        break;
                    case NodeType.Slack:
                        this.Slack_Count++;
                        if (setInitialByNominal == true) Uinit[i] = node.Unom;  //Slack-type case: Inital voltage is equal to nominal voltage level (constant)
                        break;
                }
            }

            // TODO: CHECKS for GRID
        }


        /// <summary>
        /// Rebuild nodes and branches list on node type changes 
        /// </summary>
        /// <param name="renodes">Old node collection to be rebuild</param>
        /// <param name="rebranches">Old branch collection to be rebuild</param>
        private void ReBuildNodesBranches(IEnumerable<INode> renodes, IEnumerable<IBranch> rebranches)
        {
            #region [Nodes and Branhces transformation]

            //Counter for nodes renumber
            int counter = 0;

            //Nodes sort and renumber
            this.Nodes = renodes.OrderBy(_n => _n.Type).ThenBy(_n => _n.Unom.Magnitude).ToList();
            this.Nodes.ForEach(_n => _n.Num_calc = counter++);

            // Branches to list
            this.Branches = rebranches.ToList();

            // Transform breakers branches
            foreach (var item in this.Branches.Where(b => b.Y == 0.0))
                item.Y = 1 / new Complex(0, 0.001);

            // Parallel branches amount
            for (int i = 0; i < this.Branches.Count; i++)
            {
                for (int j = 0; j < this.Branches.Count; j++)
                {
                    var br = this.Branches[i];
                    var other = this.Branches[j];

                    if (i != j) if ((br.Start == other.Start & br.End == other.End) | (br.Start == other.End & br.End == other.Start)) br.Count++;
                }
            }

            // Create internal numbers for calculation
            this.Nodes.ForEach(_n =>
                {
                    this.Branches.ForEach(b =>
                      {
                          b.Count = 1;
                          if (b.Start == _n.Num)
                          {
                              b.Start_calc = _n.Num_calc;
                          }
                          else if (b.End == _n.Num)
                          {
                              b.End_calc = _n.Num_calc;
                          }
                      });
                });            

            #endregion [Nodes and Branhces transformation]


            #region [Y calculation]

            //Calculation of admittance matrix
            this.Y = Calc_Y(this.Nodes, this.Branches);

            #endregion [Y calculation]


            PQ_Count = PV_Count = Slack_Count = 0;
        }


        /// <summary>
        /// Admittance matrix calculation
        /// </summary>
        /// <param name="nodes">Collection of (transformed) <see cref="INode"/></param>
        /// <param name="branches">Collection of (transformed) <see cref="IBranch"/></param>
        /// <returns><see cref="Matrix{Complex}"/> -> Admittance matrix with <see cref="Complex"/> data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Matrix<Complex> Calc_Y(List<INode> nodes, List<IBranch> branches)
        {
            //Initialize admittance matrix
            var Y = Matrix<Complex>.Build.Dense(nodes.Count, nodes.Count);

            for (int i = 0; i < branches.Count; i++)
            {
                var start = branches[i].Start_calc;
                var end   = branches[i].End_calc;
                var y     = branches[i].Y;
                var ysh   = branches[i].Ysh;
                var kt    = branches[i].Ktr.Magnitude <= 0 ? Complex.One : branches[i].Ktr;

                if (kt == 1) // Condition for non-Transformer branches
                {
                    Y[start, end]   +=  (y / kt);
                    Y[end, start]   +=  (y / kt);
                    Y[start, start] += -(y + ysh / 2);
                    Y[end, end]     += -(y + ysh / 2);
                }
                else if (nodes[start].Unom.Magnitude > nodes[end].Unom.Magnitude    // Condition for Transformer branches. At Start node Unom higher 
                        | nodes[start].Unom.Magnitude == nodes[end].Unom.Magnitude) // Voltage-added Transformers
                {
                    Y[start, end]   +=  (y / kt);
                    Y[end, start]   +=  (y / Complex.Conjugate(kt));
                    Y[start, start] += -(y + ysh);
                    Y[end, end]     += -(y / (kt * Complex.Conjugate(kt)));
                }
                else if (nodes[start].Unom.Magnitude < nodes[end].Unom.Magnitude)   // Condition for Transformer branches. At End node Unom higher
                {
                    Y[start, end]   +=  (y / Complex.Conjugate(kt));
                    Y[end, start]   +=  (y / kt);
                    Y[start, start] += -(y / (kt * Complex.Conjugate(kt)));
                    Y[end, end]     += -(y + ysh);
                }
            }

            // Add shunt conductivities
            for (int i = 0; i < nodes.Count; i++) 
                Y[i, i] += -nodes[i].Ysh;

            return -Y;
        }



        #endregion [Build Scheme]       

    }
}
