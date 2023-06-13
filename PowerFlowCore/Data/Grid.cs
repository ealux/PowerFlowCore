using PowerFlowCore.Algebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Complex[,] Y { get => Ysp.ToDense(); }

        /// <summary>
        /// Internal compressed Admittance matrix
        /// </summary>
        internal CSRMatrixComplex Ysp { get; private set; }



        /// <summary>
        /// Vector of nominal Voltages
        /// </summary>
        public Complex[] Unominal => VectorComplex.Create(this.Nodes.Count, (i) => this.Nodes[i].Unom);

        /// <summary>
        /// Vector of Initial Voltages for iteration procedure start
        /// </summary>
        public Complex[] Uinit { get; set; }

        /// <summary>
        /// Vector of calculated Voltages
        /// </summary>
        public Complex[] Ucalc => VectorComplex.Create(this.Nodes.Count, (i) => this.Nodes[i].U);



        /// <summary>
        /// Vector of Powers in Nodes (Generation - Load)
        /// </summary>
        public Complex[] S => VectorComplex.Create(this.Nodes.Count, (i) => this.Nodes[i].S_gen - this.Nodes[i].S_calc);

        /// <summary>
        /// Internal sparse Vector of Powers in Nodes (Generation - Load)
        /// </summary>
        internal SparseVectorComplex Ssp { get; set; }



        /// <summary>
        /// Collection of Static Load Models to be applied
        /// </summary>
        public Dictionary<int, CompositeLoadModel> LoadModels { get; set; } = new Dictionary<int, CompositeLoadModel>();


        /// <summary>
        /// Collection of  <see cref="SolverType"/> and corresponded <see cref="CalculationOptions"/>
        /// </summary>
        internal Queue<(SolverType Name, CalculationOptions Options)> Solvers { get; set; } = new Queue<(SolverType, CalculationOptions)>();


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

        #region Constructors

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
            if (!ExtensionMethods.ValidateInput(nodes, branches))
                return;
            InitParameters(nodes, branches);        // Create grid
        }

        /// <summary>
        /// Calculate initial parameters for Power Flow task computation based on network topology and characteristics
        /// </summary>
        /// <param name="converter"><see cref="IConverter"/> object that incupsulate <see cref="IEnumerable{T}"/> Nodes and Branches</param>
        public Grid(IConverter converter) : this(converter.Nodes, converter.Branches) { }

        #endregion

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

            // Initial values
            if (setInitialByNominal)
            {
                //Uinit vectors filling; PQ, PV and Slack nodes count    
                this.Uinit = VectorComplex.Create(this.Nodes.Count);

                // Set initial voltages(flat start)
                var slackAngle = this.Nodes[PQ_Count + PV_Count].Unom.Phase;
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    var node = this.Nodes[i];
                    if (node.S_calc == Complex.Zero) node.S_calc = node.S_load;  // Set load for calculus

                    switch (node.Type)
                    {
                        case NodeType.PQ:
                            Uinit[i] = Complex.FromPolarCoordinates(node.Unom.Magnitude, slackAngle);  //PQ-type case: Inital voltage is equal to nominal voltage level
                            break;
                        case NodeType.PV:
                            Uinit[i] = Complex.FromPolarCoordinates(node.Vpre, slackAngle);  //PV-type case: Inital voltage is equal to user-preset voltage level
                            break;
                        case NodeType.Slack:
                            Uinit[i] = node.Unom;  //Slack-type case: Inital voltage is equal to nominal voltage level (constant)
                            break;
                    }
                }
            }
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

            // Nodes count
            PQ_Count = PV_Count = Slack_Count = 0;
            foreach (var node in renodes)
            {
                switch (node.Type)
                {
                    case NodeType.PQ:
                        this.PQ_Count++;
                        break;
                    case NodeType.PV:
                        var vpreN = node.Vpre <= 0.0;
                        if (vpreN)
                        {
                            Logger.LogWarning($"Voltage of PV Node {node.Num} is less or equals 0.0! Node type is changed to PQ");
                            node.Type = NodeType.PQ;
                            this.PQ_Count++;
                        }
                        else
                        {
                            this.PV_Count++;
                        }
                        break;
                    case NodeType.Slack:
                        this.Slack_Count++;
                        break;
                }
            }

            //Nodes sort and renumber
            this.Nodes = renodes.OrderBy(_n => _n.Type)
                                .ThenBy(_n => _n.Num)
                                .ToList();           

            // Calc nums
            this.Nodes.ForEach(_n => _n.Num_calc = counter++);

            // Branches to list
            this.Branches = rebranches.ToList();

            //// Transform breakers branches
            //foreach (var item in this.Branches.Where(b => b.Y == 0.0))
            //    item.Y = 1 / new Complex(0, 0.001);

            var numNodes = this.Nodes.Select(n=>n.Num).ToArray();

            Parallel.For(0, this.Branches.Count, i =>
            {
                var br = this.Branches[i];
                br.Start_calc = this.Nodes[Array.IndexOf(numNodes, br.Start)].Num_calc;
                br.End_calc = this.Nodes[Array.IndexOf(numNodes, br.End)].Num_calc;
            });

            #endregion [Nodes and Branhces transformation]


            #region [Y calculation]

            //Calculation of admittance matrix
            this.Ysp = Calc_Y(this.Nodes, this.Branches); // Sparse Y            
            this.Ssp = new SparseVectorComplex(this.S);   // Sparse S

            #endregion [Y calculation]            
        }


        /// <summary>
        /// Admittance matrix calculation
        /// </summary>
        /// <param name="nodes">Collection of (transformed) <see cref="INode"/></param>
        /// <param name="branches">Collection of (transformed) <see cref="IBranch"/></param>
        /// <returns><see cref=Complex[,]"/> -> Admittance matrix with <see cref="Complex"/> data</returns>
        private CSRMatrixComplex Calc_Y(List<INode> nodes, List<IBranch> branches)
        {           
            //Initialize admittance matrix
            var Y = new Dictionary<int, Complex>[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                Y[i] = new Dictionary<int, Complex>();
            }

            for (int i = 0; i < branches.Count; i++)
            {
                var start = branches[i].Start_calc;
                var end = branches[i].End_calc;
                var y = branches[i].Y;
                var ysh = branches[i].Ysh;
                var kt = branches[i].Ktr.Magnitude <= 0 ? Complex.One : branches[i].Ktr;

                if (!Y[start].ContainsKey(start))
                    Y[start].Add(start, Complex.Zero);
                if (!Y[start].ContainsKey(end))
                    Y[start].Add(end, Complex.Zero);
                if (!Y[end].ContainsKey(start))
                    Y[end].Add(start, Complex.Zero);
                if (!Y[end].ContainsKey(end))
                    Y[end].Add(end, Complex.Zero);

                if (kt == 1) // Condition for non-Transformer branches
                {
                    Y[start][end] -= (y / kt);
                    Y[end][start] -= (y / kt);
                    Y[start][start] += (y + ysh / 2);
                    Y[end][end] += (y + ysh / 2);
                }
                else if (nodes[start].Unom.Magnitude > nodes[end].Unom.Magnitude    // Condition for Transformer branches. At Start node Unom higher 
                        | nodes[start].Unom.Magnitude == nodes[end].Unom.Magnitude) // Voltage-added Transformers
                {
                    Y[start][end] -= (y / kt);
                    Y[end][start] -= (y / Complex.Conjugate(kt));
                    Y[start][start] += (y + ysh);
                    Y[end][end] += (y / (kt * Complex.Conjugate(kt)));
                }
                else if (nodes[start].Unom.Magnitude < nodes[end].Unom.Magnitude)   // Condition for Transformer branches. At End node Unom higher
                {
                    Y[start][end] -= (y / Complex.Conjugate(kt));
                    Y[end][start] -= (y / kt);
                    Y[start][start] += (y / (kt * Complex.Conjugate(kt)));
                    Y[end][end] += (y + ysh);
                }
            }

            // Add shunt conductivities
            for (int i = 0; i < nodes.Count; i++)
            {
                Y[i][i] += nodes[i].Ysh.Conjugate();  
            }

            return CSRMatrixComplex.CreateFromRows(Y, nodes.Count);
        }


        #endregion [Build Scheme]       

        #region [Internal Helper methods]

        /// <summary>
        /// Set to <see cref="Grid.Id"/>
        /// </summary>
        /// <param name="id">Id to set</param>
        internal Grid WithId(string id)
        {
            this.Id = id;
            return this;
        }

        #endregion

    }
}
