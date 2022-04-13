using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// PF-problem basic Parameters Description calculated from network topology and characteristics
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Transformed (renumbered) collection of Nodes
        /// Sorted from PQ, through PV nodes, up to Slack nodes at the end
        /// Also sorted by nominal voltage level (magnitude)
        /// </summary>
        public List<INode> Nodes { get; set; }

        /// <summary>
        /// Transformed (renumbered) collection of Branches
        /// </summary>
        public List<IBranch> Branches { get; set; }


        /// <summary>
        /// Admittance matrix
        /// </summary>
        public Matrix<Complex> Y { get; set; }



        /// <summary>
        /// Vector of nominal Voltage levels
        /// </summary>
        public Vector<Complex> Unominal { get; set; }

        /// <summary>
        /// Vector of Initial (nominal) Voltage levels (for Voltage Angle and Voltage Magnitude decomposition)
        /// </summary>
        public Vector<Complex> Uinit { get; set; }

        /// <summary>
        /// Vector of calculated Voltages
        /// </summary>
        public Vector<Complex> Ucalc { get; set; }



        /// <summary>
        /// Vector of Powers in Nodes (Load - Generation)
        /// </summary>
        public Vector<Complex> S { get; set; }



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
        /// Calculate initial parameters for Power Flow task computation based on network topology and characteristics
        /// </summary>
        /// <param name="nodes">Enumerable source of raw-view Nodes</param>
        /// <param name="branches">Enumerable source of raw-view Branches</param>
        public Grid(IEnumerable<INode> nodes, IEnumerable<IBranch> branches) => InitParameters(nodes, branches); //ctor               



        #region [Build Scheme]


        /// <summary>
        /// Initialize (or re-initialize) parameters
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
            this.Unominal = Vector<Complex>.Build.Dense(this.Nodes.Count);
            this.Uinit    = Vector<Complex>.Build.Dense(this.Nodes.Count);
            this.Ucalc    = Vector<Complex>.Build.Dense(this.Nodes.Count);

            // Set nominal voltages
            for (int i = 0; i < Nodes.Count; i++)
                this.Unominal[i] = Nodes[i].Unom;


            //Empty S vector of Nodes's count capacity
            this.S = Vector<Complex>.Build.Dense(this.Nodes.Count);

            // Reset PV nodes to PQ modes
            foreach (var node in Nodes)
            {
                if (node.Type == NodeType.PV)
                {
                    var vpreN = node.Vpre  == 0.0;
                    var qminN = node.Q_min == 0.0;
                    var qmaxN = node.Q_max == 0.0;

                    if ((vpreN & qminN) | (qminN & qmaxN))
                    {
                        node.Type = NodeType.PQ;                        
                        ReBuildNodesBranches(renodes: nodes, rebranches: branches); //Rebuilding Nodes
                    }
                    else if (!qminN & qmaxN)
                    {
                        node.Type = NodeType.PQ;
                        node.S_gen = new Complex(node.S_gen.Real, node.Q_min);
                        ReBuildNodesBranches(renodes: nodes, rebranches: branches); //Rebuilding Nodes
                    }
                    else if (qminN & !qmaxN)
                    {
                        node.Type = NodeType.PQ;
                        ReBuildNodesBranches(renodes: nodes, rebranches: branches); //Rebuilding Nodes
                    }
                }
            }

            //Count nodes and Set initial voltages
            foreach (var node in this.Nodes)
            {               
                switch (node.Type)
                {
                    case NodeType.PQ:
                        this.PQ_Count++;
                        if (setInitialByNominal == true) Uinit[node.Num_calc] = node.Unom;  //PQ-type case: Inital voltage is equal to nominal voltage level
                        break;
                    case NodeType.PV:
                        this.PV_Count++;
                        if (setInitialByNominal == true) Uinit[node.Num_calc] = node.Vpre;  //PV-type case: Inital voltage is equal to user-preset voltage level
                        break;
                    case NodeType.Slack:
                        this.Slack_Count++;
                        if (setInitialByNominal == true) Uinit[node.Num_calc] = node.Unom;  //Slack-type case: Inital voltage is equal to nominal voltage level (constant)
                        break;
                    default:
                        break;
                }                

                //Set node power injection
                S[node.Num_calc] = new Complex(node.S_gen.Real - node.S_load.Real, 
                                               node.S_gen.Imaginary - node.S_load.Imaginary);
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

            //Nodes sort and renumber - !Дописать принцип
            this.Nodes = renodes.OrderBy(_n => _n.Type).ThenBy(_n => _n.Unom.Magnitude).ToList();
            this.Nodes.ForEach(_n => _n.Num_calc = counter++);

            //!ветви
            this.Branches = rebranches.ToList();

            //!переименование номеров ветви
            this.Nodes.ForEach(_n => 
                { this.Branches.ForEach(b => 
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


            for (int i = 0; i < this.Branches.Count; i++)
            {
                for (int j = 0; j < this.Branches.Count; j++)
                {
                    var br = this.Branches[i];
                    var other = this.Branches[j];

                    if (i != j) if ((br.Start == other.Start & br.End == other.End) | (br.Start == other.End & br.End == other.Start)) br.Count++;
                }
            }

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
        /// <param name="nodes">Collection of (transformed) Nodes</param>
        /// <param name="branches">Collection of (transformed) Branches</param>
        /// <returns>Matrix complex -> Admittance matrix</returns>
        private Matrix<Complex> Calc_Y(List<INode> nodes, List<IBranch> branches)
        {
            //Initialize admittance matrix
            var Y = Matrix<Complex>.Build.Dense(nodes.Count, nodes.Count);

            for (int i = 0; i < branches.Count; i++)
            {
                var start = branches[i].Start_calc;
                var end = branches[i].End_calc;
                var y = branches[i].Y;
                var ysh = branches[i].Ysh;
                var kt = branches[i].Ktr <= 0 ? 1 : branches[i].Ktr; if (kt > 1) kt = 1 / kt;

                Y[start, end] += (y / kt);
                Y[end, start] += (y / kt);

                if (kt == 1) //Condition for non-Transformer branches
                {
                    Y[start, start] += -(y + ysh / 2);
                    Y[end, end] += -(y + ysh / 2);
                }
                else if (nodes[start].Unom.Magnitude > nodes[end].Unom.Magnitude) //Condition for Transformer branches. At Start node Unom higher
                {
                    Y[start, start] += -(y + ysh);
                    Y[end, end] += -(y / (kt * kt));
                }
                else if (nodes[start].Unom.Magnitude < nodes[end].Unom.Magnitude) //Condition for Transformer branches. At End node Unom higher
                {
                    Y[start, start] += -(y / (kt * kt));
                    Y[end, end] += -(y + ysh);
                }
            }

            for (int i = 0; i < nodes.Count; i++) Y[i, i] += -nodes[i].Ysh; //шунт в узле

            Y = -Y;

            return Y;
        }

        #endregion [Build Scheme]       



        #region [Solvers]
        


        #region [Newton-Raphson]

        /// <summary>
        /// Jacobian calculation on each iteration
        /// </summary>
        /// <param name="U">U vector for Jacobian matrix calculation on each iteration</param>
        /// <returns>Matrix double -> Jacobian numeric matrix</returns>
        public Matrix<double> Jacobian_Polar(Vector<Complex> U)
        {
            var dim = PQ_Count + PV_Count;

            var G = Y.Real();
            var B = Y.Imaginary();

            var Um = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            var P_Delta = Matrix<double>.Build.Dense(dim, dim);
            var P_V = Matrix<double>.Build.Dense(dim, PQ_Count);
            var Q_Delta = Matrix<double>.Build.Dense(PQ_Count, dim);
            var Q_V = Matrix<double>.Build.Dense(PQ_Count, PQ_Count);

            //P_Delta
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (i != j) P_Delta[i, j] = -Um[i] * Um[j] * Y[i, j].Magnitude * Math.Sin(Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        P_Delta[i, j] = -Y[i, j].Magnitude * Math.Pow(Um[i], 2) * Math.Sin(Y[i, j].Phase);
                        for (int k = 0; k < dim + Slack_Count; k++) P_Delta[i, j] += Um[i] * Um[k] * Y[i, k].Magnitude * Math.Sin(Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }

            }

            //P_V
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < PQ_Count; j++)
                {
                    if (i != j) P_V[i, j] = Um[i] * Y[i, j].Magnitude * Math.Cos(Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        P_V[i, j] = Um[i] * Y[i, j].Magnitude * Math.Cos(Y[i, j].Phase); ;
                        for (int k = 0; k < dim + Slack_Count; k++) P_V[i, j] += Um[k] * Y[i, k].Magnitude * Math.Cos(Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }

            }

            //Q_Delta
            for (int i = 0; i < PQ_Count; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (i != j) Q_Delta[i, j] = -Um[i] * Um[j] * Y[i, j].Magnitude * Math.Cos(Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        Q_Delta[i, j] = -Y[i, j].Magnitude * Math.Pow(Um[i], 2) * Math.Cos(Y[i, j].Phase);
                        for (int k = 0; k < dim + Slack_Count; k++) Q_Delta[i, j] += Um[i] * Um[k] * Y[i, k].Magnitude * Math.Cos(Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }

            }

            //Q_V
            for (int i = 0; i < PQ_Count; i++)
            {
                for (int j = 0; j < PQ_Count; j++)
                {
                    if (i != j) Q_V[i, j] = -Um[i] * Y[i, j].Magnitude * Math.Sin(Y[i, j].Phase + Uph[j] - Uph[i]);
                    else
                    {
                        Q_V[i, j] = -Um[i] * Y[i, j].Magnitude * Math.Sin(Y[i, j].Phase);
                        for (int k = 0; k < dim + Slack_Count; k++) Q_V[i, j] -= Um[k] * Y[i, k].Magnitude * Math.Sin(Y[i, k].Phase + Uph[k] - Uph[i]);
                    }
                }

            }

            var res = Matrix<double>.Build.DenseOfMatrixArray(new[,] { { P_Delta, P_V }, { Q_Delta, Q_V } });

            return res;
        }


        /// <summary>
        /// Power residuals (dP, dQ)
        /// </summary>
        /// <param name="U">U vector for Power residuals vector calculation on each iteration</param>
        /// <returns>Vector double -> Power residuals numeric vector</returns>
        public Vector<double> Resuduals_Polar(Vector<Complex> U)
        {
            var dim = PQ_Count + PV_Count;

            var G = Y.Real();
            var B = Y.Imaginary();

            var Um = U.Map(u => u.Magnitude);
            var Uph = U.Map(u => u.Phase);

            var dP = Vector<double>.Build.Dense(dim);
            var dQ = Vector<double>.Build.Dense(PQ_Count);


            //dP
            for (int i = 0; i < dim; i++)
            {
                dP[i] = S[i].Real;
                for (int j = 0; j < dim + Slack_Count; j++) dP[i] -= Um[i] * Um[j] * Y[i, j].Magnitude * Math.Cos(Y[i, j].Phase + Uph[j] - Uph[i]);
            }

            //dQ
            for (int i = 0; i < PQ_Count; i++)
            {
                dQ[i] = S[i].Imaginary;
                for (int j = 0; j < dim + Slack_Count; j++) dQ[i] -= -Um[i] * Um[j] * Y[i, j].Magnitude * Math.Sin(Y[i, j].Phase + Uph[j] - Uph[i]);
            }

            return Vector<double>.Build.DenseOfEnumerable(dP.Concat(dQ));
        }


        /// <summary>
        /// Newton-Raphson Solver (use for non-PV schemes types)
        /// </summary>
        /// <param name="initialGuess">Vector complex -> Initial voltage values vector</param>
        /// <param name="accuracy">Minimal power residual threshold to stop computing</param>
        /// <param name="voltageConvergence">Minimal voltage convergence threshold to stop computing</param>
        /// <param name="iterations">Maximum  number iterations</param>
        /// <returns></returns>
        public Vector<Complex> NewtonRaphsonSolver(Vector<Complex> initialGuess,
                                                    double accuracy = 1e-12,
                                                    double voltageConvergence = 1e-12,
                                                    int iterations = 1500,
                                                    double voltageRatio = 0.5)
        {
            var dim = PQ_Count + PV_Count;

            //throw if "accuracy" criteria is less or equal zero
            if (accuracy <= 0) throw new ArgumentException("Iterations amount can not be less or equal 0!");

            var Um = initialGuess.Map(x => x.Magnitude).ToArray();
            var ph = initialGuess.Map(x => x.Phase).ToArray();

            var U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u1, u2) => Complex.FromPolarCoordinates(u1, u2)));

            //PreCalc (G-s)
            //U = GaussSeidelSolver(U, accuracy: 0.001, iterations: 5);
            //Um = U.Map(x => x.Magnitude).ToArray();
            //ph = U.Map(x => x.Phase).ToArray();

            for (int i = 0; i < iterations; i++)
            {
                var dPQ = Resuduals_Polar(U);   //Power residuals                       
                var J = Jacobian_Polar(U);      //Jacoby matrix                

                //Calculation of increments
                var dx = J.Solve(-dPQ);

                //Voltage residual
                var Udx = Vector<double>.Build.Dense(PQ_Count);

                //Update voltage levels
                for (int j = 0; j < PQ_Count + PV_Count; j++) ph[j] -= dx[j];
                for (int j = 0; j < PQ_Count; j++) { Um[j] -= dx[j + PQ_Count]; Udx[j] = Math.Abs(dx[j + PQ_Count]); }
                U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u1, u2) => Complex.FromPolarCoordinates(u1, u2)));


                //Checks               
                //CheckVoltage(Uinit: initialGuess, U: U, ratio: voltageRatio); //Check voltage difference towards the nominal one

                if (dPQ.InfinityNorm() <= accuracy) //Power residual check
                {
                    Console.WriteLine($"N-R iterations: {i}" + $" of {iterations} (Power residual criteria)");
                    //Update voltage levels
                    for (int n = 0; n < Nodes.Count; n++) Nodes[n].U = U[n];
                    return U;
                }
                if (Udx.InfinityNorm() <= voltageConvergence) //Voltage check
                {
                    Console.WriteLine($"N-R iterations: {i}" + $" of {iterations} (Voltage convergence criteria)");
                    //Update voltage levels
                    for (int n = 0; n < Nodes.Count; n++) Nodes[n].U = U[n];
                    return U;
                }
            }

            //Update voltage levels
            for (int n = 0; n < Nodes.Count; n++) Nodes[n].U = U[n];
            return U;
        }

        #endregion [Newton-Raphson]

        #endregion


    }
}
