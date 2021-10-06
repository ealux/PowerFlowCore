using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra.Solvers;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// PF-task basic Parameters Description calculated from network topology and characteristics
    /// </summary>
    class NetDescriptionAlternative
    {       
        /// <summary>
        /// Transformed (renumbered) collection of Nodes
        /// Sorted from PQ, through PV nodes, up to Slack nodes at the end
        /// Also sorted by nominal voltage level (magnitude)
        /// </summary>
        public List<INode> Nodes { get; private set; }

        /// <summary>
        /// Transformed (renumbered) collection of Branches
        /// </summary>
        public List<IBranch> Branches { get; private set; }

        /// <summary>
        /// Admittance matrix
        /// </summary>
        public Matrix<Complex> Y { get; private set; }

        /// <summary>
        /// Vector of Initial Voltage levels (for Voltage Angle and Voltage Magnitude decomposition)
        /// </summary>
        public Vector<Complex> U_init { get; set; }

        /// <summary>
        /// Vector of calculated Voltages
        /// </summary>
        Vector<Complex> U_calc { get; set; }

        /// <summary>
        /// Vector of Powers in Nodes (Load - Generation)
        /// </summary>
        public Vector<Complex> S { get; private set; }

        /// <summary>
        /// Vector of Powers in Nodes
        /// </summary>
        public Matrix<Complex> S_calc { get; private set; }

        /// <summary>
        /// PQ nodes amount
        /// </summary>
        public int PQ_Count { get; private set; }

        /// <summary>
        /// PV nodes amount
        /// </summary>
        public int PV_Count { get; private set; }

        /// <summary>
        /// Slack nodes amount
        /// </summary>
        public int Slack_Count { get; private set; }



        /// <summary>
        /// Calculate initial parameters for PF task computation based on network topology and characteristics
        /// </summary>
        /// <param name="nodes">Enumerable source of raw-view Nodes</param>
        /// <param name="branches">Enumerable source of raw-view Branches</param>
        public NetDescriptionAlternative(IEnumerable<INode> nodes, IEnumerable<IBranch> branches) => InitParameters(nodes, branches);


        #region [Build Scheme]


        /// <summary>
        /// Initialize (or re-initialize) parameters
        /// </summary>
        /// <param name="nodes">Enumerable source of raw-view Nodes</param>
        /// <param name="branches">Enumerable source of raw-view Branches</param>
        private void InitParameters(IEnumerable<INode> nodes, IEnumerable<IBranch> branches, bool setUtoU_init = false)
        {
            //Initial calc
            ReBuildNodesBranches(renodes: nodes, rebranches: branches);


            #region [S and U_init vectors filling; PQ, PV and Slack nodes count]            

            //U_init vector of Nodes's count capacity
            if (setUtoU_init == false)
                this.U_init = Vector<Complex>.Build.Dense(this.Nodes.Count);
            else
                for (int i = 0; i < Nodes.Count; i++) this.U_init[i] = Nodes[i].U;
            //Empty S vector of Nodes's count capacity
            this.S = Vector<Complex>.Build.Dense(this.Nodes.Count);


            foreach (var node in Nodes)
            {
                if (node.Type == NodeType.PV)
                {
                    var vpreN = node.Vpre == 0.0;
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
                    //else if(!vpreN & !qminN & !qmaxN)
                    //{
                    //    node.S_gen = new Complex(node.S_gen.Real, node.S_gen.Real*Math.Tan(Math.Acos(0.85)));//(node.Q_min + node.Q_max)/2);
                    //}
                }
            }

            //Iterate nodes
            foreach (var node in this.Nodes)
            {
                switch (node.Type)
                {
                    case NodeType.PQ:
                        this.PQ_Count++;
                        if (setUtoU_init == false) U_init[node.Num_calc] = node.Unom;  //PQ-type case: Inital voltage is equal to nominal voltage level
                        break;
                    case NodeType.PV:
                        this.PV_Count++;
                        if (setUtoU_init == false) U_init[node.Num_calc] = node.Vpre;  //PV-type case: Inital voltage is equal to user-preset voltage level
                        break;
                    case NodeType.Slack:
                        this.Slack_Count++;
                        if (setUtoU_init == false) U_init[node.Num_calc] = node.Unom;  //Slack-type case: Inital voltage is equal to nominal voltage level (constant)
                        break;
                    default:
                        break;
                }

                //
                S[node.Num_calc] = new Complex(-node.S_load.Real + node.S_gen.Real, -node.S_load.Imaginary + node.S_gen.Imaginary);
            }

            #endregion [S and U_init vectors filling; PQ, PV and Slack nodes count]
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="renodes"></param>
        /// <param name="rebranches"></param>
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
            this.Nodes.ForEach(_n => { this.Branches.ForEach(b => { if (b.Start == _n.Num) b.Start_calc = _n.Num_calc; else if (b.End == _n.Num) b.End_calc = _n.Num_calc; }); });

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

            //Y = -Y;

            return Y;
        }

        #endregion [Build Scheme]       


        /// <summary>
        /// Jacobian
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
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
                    if (i != j) P_Delta[i, j] = Um[i] * Um[j] * (G[i, j] * Math.Sin(Uph[j] - Uph[i]) + B[i, j] * Math.Cos(Uph[j] - Uph[i]));
                }
                for (int k = 0; k < dim + Slack_Count; k++)
                {
                    if (k != i)
                    {
                        P_Delta[i, i] -= Um[i] * Um[k] * (G[i, k] * Math.Sin(Uph[k] - Uph[i]) + B[i, k] * Math.Cos(Uph[k] - Uph[i]));
                    }
                }
            }

            //P_V
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < PQ_Count; j++)
                {
                    if (i != j) P_V[i, j] = Um[i] * (-G[i, j] * Math.Cos(Uph[j] - Uph[i]) + B[i, j] * Math.Sin(Uph[j] - Uph[i]));
                    else
                    {
                        P_V[i, j] = -2 * Um[i] * G[i, j];
                        for (int k = 0; k < dim + Slack_Count; k++) if (k != i) P_V[i, j] += Um[k] * (-G[i, k] * Math.Cos(Uph[k] - Uph[i]) + B[i, k] * Math.Sin(Uph[k] - Uph[i]));
                    }
                }

            }

            //Q_Delta
            for (int i = 0; i < PQ_Count; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (i != j) Q_Delta[i, j] = -Um[i] * Um[j] * (-G[i, j] * Math.Cos(Uph[j] - Uph[i]) + B[i, j] * Math.Sin(Uph[j] - Uph[i]));
                    else
                    {
                        for (int k = 0; k < dim + Slack_Count; k++)
                        {
                            if (k != i)
                            {
                                Q_Delta[i, j] += Um[i] * Um[k] * (-G[i, k] * Math.Cos(Uph[k] - Uph[i]) + B[i, k] * Math.Sin(Uph[k] - Uph[i]));
                            }
                        }
                    }
                }

            }

            //Q_V
            for (int i = 0; i < PQ_Count; i++)
            {
                for (int j = 0; j < PQ_Count; j++)
                {
                    if (i != j) Q_V[i, j] = -Um[i] * (-G[i, j] * Math.Sin(Uph[j] - Uph[i]) - B[i, j] * Math.Cos(Uph[j] - Uph[i]));
                }
                Q_V[i, i] = 2 * Um[i] * B[i, i];
                for (int k = 0; k < dim + Slack_Count; k++)
                {
                    if (k != i)
                    {
                        Q_V[i, i] += Um[k] * (G[i, k] * Math.Sin(Uph[k] - Uph[i]) + B[i, k] * Math.Cos(Uph[k] - Uph[i]));
                    }
                }
            }

            var res = Matrix<double>.Build.DenseOfMatrixArray(new[,] { { P_Delta, P_V }, { Q_Delta, Q_V } });

            return res;
        }


        /// <summary>
        /// Residuals (dP, dQ) - Sample 1
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
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
                dP[i] = -S[i].Real;
                for (int j = 0; j < dim + Slack_Count; j++) dP[i] += Um[i] * Um[j] * (-G[i, j] * Math.Cos(Uph[j] - Uph[i]) + B[i, j] * Math.Sin(Uph[j] - Uph[i]));
            }

            //dQ
            for (int i = 0; i < PQ_Count; i++)
            {
                dQ[i] = -S[i].Imaginary;
                for (int j = 0; j < dim + Slack_Count; j++) dQ[i] += Um[i] * Um[j] * (G[i, j] * Math.Sin(Uph[j] - Uph[i]) + B[i, j] * Math.Cos(Uph[j] - Uph[i]));
            }

            return Vector<double>.Build.DenseOfEnumerable(dP.Concat(dQ));
        }


        /// <summary>
        /// Gauss-Seidel solver
        /// </summary>
        /// <param name="initialGuess"></param>
        /// <param name="accuracy"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public Vector<Complex> GaussSeidelSolver(Vector<Complex> initialGuess, double accuracy = 1e-6, int iterations = 1500)
            {

                var Um = initialGuess.Map(x => x.Magnitude).ToArray();
                var ph = initialGuess.Map(x => x.Phase).ToArray();

                var U = Vector<Complex>.Build.Dense(initialGuess.Count);
                var Uold = Vector<Complex>.Build.Dense(initialGuess.Count);
                var dU = Vector<Complex>.Build.Dense(initialGuess.Count);

                var diff = 1000000.0;

                U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u1, u2) => Complex.FromPolarCoordinates(u1, u2)));
                U.CopyTo(Uold);

                var sum = new Complex();

                int counter = 0;

                while (diff > accuracy | counter <= iterations)
                {
                    //Nodes iterator
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        //PQ
                        if (Nodes[i].Type == NodeType.PQ)
                        {
                            for (int j = 0; j < Nodes.Count; j++) if (i != j) sum += -Y[i, j] * U[j];
                            U[i] = (1 / -Y[i, i]) * ((S[i].Conjugate() / U[i].Conjugate()) - sum);

                            dU = U - Uold;
                            U = Uold + dU;
                        }

                        sum = 0;

                        Um = U.Map(u => u.Magnitude).ToArray();
                        ph = U.Map(u => u.Phase).ToArray();

                        //PV
                        if (Nodes[i].Type == NodeType.PV)
                        {
                            Um[i] = U[i].Magnitude;

                            for (int j = 0; j < Nodes.Count; j++) sum += -Y[i, j] * U[j];

                            var Q_new = -(U[i].Conjugate() * sum).Imaginary; //new Q element

                            sum = 0;

                            S[i] = new Complex(S[i].Real, Q_new); //new S element

                            for (int j = 0; j < Nodes.Count; j++) if (i != j) sum += -Y[i, j] * U[j];
                            U[i] = (1 / -Y[i, i]) * ((S[i].Conjugate() / U[i].Conjugate()) - sum);

                            U[i] = Complex.FromPolarCoordinates(Uold[i].Magnitude, U[i].Phase);
                        }

                        Um = U.Map(u => u.Magnitude).ToArray();
                        ph = U.Map(u => u.Phase).ToArray();
                    }

                    counter++;

                    U.CopyTo(Uold);
                    diff = dU.AbsoluteMaximum().Real;
                }

                if (PV_Count != 0)
                {
                    bool flag = false;

                    for (int pv = PQ_Count; pv < PQ_Count + PV_Count; pv++)
                    {
                        var qmin = Nodes[pv].Q_min;
                        var qmax = Nodes[pv].Q_max;

                        var q = -Nodes[pv].S_load.Imaginary + S[pv].Imaginary;

                        if (q <= qmin)
                        {
                            q = qmin;
                            Nodes[pv].S_gen = new Complex(Nodes[pv].S_gen.Real, q);
                            Nodes[pv].Type = NodeType.PQ;
                            flag = true;

                        }
                        else if (q >= qmax)
                        {
                            q = qmax;
                            Nodes[pv].S_gen = new Complex(Nodes[pv].S_gen.Real, q);
                            Nodes[pv].Type = NodeType.PQ;
                            flag = true;
                        }
                    }

                    if (flag == true)
                    {
                        InitParameters(Nodes, Branches);
                        var new_U = GaussSeidelSolver(U, accuracy: accuracy, iterations: iterations);
                        return new_U;
                    }

                }

                return U;
            }


        /// <summary>
        /// Linear solver
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        public Vector<Complex> NewtonRaphsonSolver(Vector<Complex> initialGuess, double accuracy = 1e-12, double voltageConvergence = 1e-12, int iterations = 1500)
        {
            var dim = PQ_Count + PV_Count;

            if (accuracy <= 0) throw new ArgumentException("Iterations amount can not be less or equal 0!");

            var Um = initialGuess.Map(x => x.Magnitude).ToArray();
            var ph = initialGuess.Map(x => x.Phase).ToArray();

            var U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u1, u2) => Complex.FromPolarCoordinates(u1, u2)));

            U = GaussSeidelSolver(U, accuracy: 0.001, iterations: 3);
            Um = U.Map(x => x.Magnitude).ToArray();
            ph = U.Map(x => x.Phase).ToArray();

            var bb = S;

            for (int i = 0; i < iterations; i++)
            {
                var dPQ = Resuduals_Polar(U);   //Power residuals                       
                var J = Jacobian_Polar(U);      //Jacoby matrix

                //Calculation of increments
                var dx = -J.Solve(dPQ);

                //Voltage residual
                var Udx = Vector<double>.Build.Dense(PQ_Count);

                //Update voltage levels
                for (int j = 0; j < PQ_Count + PV_Count; j++) ph[j] += dx[j];
                for (int j = 0; j < PQ_Count; j++) { Um[j] += dx[j + PQ_Count]; Udx[j] = Math.Abs(dx[j + PQ_Count]); }
                U = Vector<Complex>.Build.DenseOfEnumerable(Um.Zip(ph, (u1, u2) => Complex.FromPolarCoordinates(u1, u2)));

                //Update voltage levels
                for (int n = 0; n < Nodes.Count; n++) Nodes[n].U = U[n];

                //Checks
                if (dPQ.InfinityNorm() <= accuracy) //Power residual check
                {
                    Console.WriteLine($"N-R iterations: {i}" + $" of {iterations} (Power residual criteria)");
                    break;
                }
                //if (Udx.InfinityNorm() <= voltageConvergence) //Voltage residual check
                //{
                //    Console.WriteLine($"N-R iterations: {i}" + $" of {iterations} (Voltage residual criteria)");
                //    break;
                //}
            }

            //Regulate PV nodes
            #region [PV options]

            if (PV_Count != 0)
            {
                bool flag = false;

                for (int pv = PQ_Count; pv < PQ_Count + PV_Count; pv++)
                {
                    //#region [Update PV node Q power]
                    //var sum = new Complex();
                    //for (int j = 0; j < dim + Slack_Count; j++) sum += U[j] * Y[pv, j].Conjugate();
                    //var Q_new = (U[pv].Conjugate() * sum).Imaginary; //new Q element

                    //S[pv] = new Complex(S[pv].Real, Q_new);           //new S element

                    //#endregion [Update PV node Q power]

                    //var qmin = Nodes[pv].Q_min;
                    //var qmax = Nodes[pv].Q_max;

                    //var q = Nodes[pv].S_load.Imaginary - S[pv].Imaginary;

                    //if (q <= qmin)
                    //{
                    //    q = qmin;
                    //    Nodes[pv].S_gen = new Complex(Nodes[pv].S_gen.Real, q);
                    //    Nodes[pv].Type = NodeType.PQ;
                    //    flag = true;

                    //}
                    //else if (q >= qmax)
                    //{
                    //    q = qmax;
                    //    Nodes[pv].S_gen = new Complex(Nodes[pv].S_gen.Real, q);
                    //    Nodes[pv].Type = NodeType.PQ;
                    //    flag = true;
                    //}
                }

                if (flag == true)
                {
                    InitParameters(Nodes, Branches);
                    var new_U = NewtonRaphsonSolver(U, accuracy: accuracy, voltageConvergence: voltageConvergence, iterations: iterations);
                    return new_U;
                }
            }

            #endregion [PV options]

            Console.WriteLine($"iterations: {iterations}" + $" of {iterations} (Iteration criteria)");
            return U;
        }


        #region Additional

        #region Transorm parametrs for Rectangle

        //FOR INITIAL AND ITERATION STATEMENT
        private Vector<double> UFromComplexToDouble_Rect(Vector<Complex> U)
        {
            var U1 = U.Real();
            var U2 = U.Imaginary();

            var res = U1.ToColumnMatrix().Stack(U2.ToColumnMatrix()).Column(0);

            return res;
        }


        //FOR CALCULATION
        private Vector<Complex> UFromDoubleToComplex_Rect(Vector<double> U)
        {
            int count = this.U_init.Count;

            var U1 = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(U.ToArray(), 0, count).ToArray());
            var U2 = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(U.ToArray(), count, count).ToArray());

            var res = Vector<Complex>.Build.DenseOfEnumerable(U1.Zip(U2, (u1, u2) => new Complex(u1, u2)));

            return res;
        }

        #endregion Transorm parametrs for Rectangle

        #region Transorm parametrs for Polar

        //FOR INITIAL AND ITERATION STATEMENT
        private Vector<double> UFromComplexToDouble_Polar(Vector<Complex> U)
        {
            var Um = U.Map(x => x.Magnitude);
            var Uph = U.Map(x => x.Phase);

            var res = Um.ToColumnMatrix().Stack(Uph.ToColumnMatrix()).Column(0);

            return res;
        }


        //FOR CALCULATION
        private Vector<Complex> UFromDoubleToComplex_Polar(Vector<double> U)
        {
            int count = this.U_init.Count;

            var U1 = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(U.ToArray(), 0, count).ToArray());
            var U2 = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(U.ToArray(), count, count).ToArray());

            var res = Vector<Complex>.Build.DenseOfEnumerable(U1.Zip(U2, (u1, u2) => new Complex(u1 * Math.Cos(u2), u1 * Math.Sin(u2))));

            return res;
        }

        #endregion Transorm parametrs for Polar
        

        /// <summary>
        /// Vector of resuduals in Rectagle coordinates
        /// </summary>
        /// <param name="URealImag"></param>
        /// <returns></returns>
        private Vector<double> RectangleResidualModel(Vector<double> URealImag, Vector<double> X)
        {

            var U = UFromDoubleToComplex_Rect(URealImag);    //Vector of complex U

            var res = Vector<double>.Build.Dense(2 * this.U_init.Count); //Пустой вектор результатов

            var G = this.Y.Real();
            var B = this.Y.Imaginary();

            var P = this.S.Real();
            var Q = this.S.Imaginary();

            var U1 = U.Real();
            var U2 = U.Imaginary();

            var position = 0;   //Conter for vactor position

            for (int i = 0; i < PQ_Count + PV_Count; i++) //dP
            {
                double dP = 0.0;

                for (int j = 0; j < G.RowCount - Slack_Count; j++) //
                {
                    if (i != j)
                    {
                        dP += -G[j, i] * (U1[i] * U1[j] + U2[i] * U2[j]) + B[j, i] * (U1[i] * U2[j] + U1[i] * U2[j]);
                    }
                    else
                    {
                        dP += P[i] - G[i, i] * (U1[i] * U1[i] + U2[i] * U2[i]);
                    }
                }

                res[position++] = dP;
            }

            for (int i = 0; i < PQ_Count; i++) //dQ
            {
                double dQ = 0.0;

                for (int j = 0; j < G.RowCount - Slack_Count; j++)
                {
                    if (i != j)
                    {
                        dQ += G[j, i] * (U1[i] * U2[j] + U1[i] * U2[j]) + B[j, i] * (U1[i] * U1[j] + U2[i] * U2[j]);
                    }
                    else
                    {
                        dQ += Q[i] + B[i, i] * (U1[i] * U1[i] + U2[i] * U2[i]);
                    }
                }

                res[position++] = dQ;
            }

            for (int i = PQ_Count; i < G.RowCount - Slack_Count; i++)  // dE
            {
                double dE = 0.0;

                for (int j = 0; j < G.RowCount; j++) //
                {
                    if (i != j)
                    {
                        dE += Math.Pow(this.U_init[i].Magnitude, 2) - (U1[i] * U1[i] + U2[i] * U2[i]);
                    }
                }

                res[position++] = dE;
            }

            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="U"></param>
        /// <returns></returns>
        private Vector<double> PolarResidualModel(Vector<double> UMagnitudePhase, Vector<double> X)
        {
            var res = Vector<double>.Build.Dense(2 * this.U_init.Count); //Пустой вектор результатов

            //var YY = this.Y.Conjugate();

            var G = this.Y.Real();
            var B = this.Y.Imaginary();

            var P = this.S.Real();
            var Q = this.S.Imaginary();

            int count = this.U_init.Count;
            var Um = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(UMagnitudePhase.ToArray(), 0, count).ToArray());
            var Uph = Vector<double>.Build.DenseOfArray(new ArraySegment<double>(UMagnitudePhase.ToArray(), count, count).ToArray());

            var position = 0;   //Conter for vactor position

            for (int i = 0; i < PQ_Count + PV_Count; i++) //dP
            {
                double dP = 0.0;

                for (int j = 0; j < G.RowCount - Slack_Count; j++) //
                {
                    if (i != j)
                    {
                        dP += -Um[i] * (G[j, i] * Math.Cos(Uph[i] - Uph[j]) + B[j, i] * Math.Sin(Uph[i] - Uph[j]));
                    }
                    else
                    {
                        dP += P[i] - Um[i] * G[i, i];
                    }
                }

                res[position++] = dP;
            }

            for (int i = 0; i < PQ_Count; i++) //dQ
            {
                double dQ = 0.0;

                for (int j = 0; j < G.RowCount - Slack_Count; j++)
                {
                    if (i != j)
                    {
                        dQ += Um[i] * (B[j, i] * Math.Cos(Uph[i] - Uph[j]) - G[j, i] * Math.Sin(Uph[i] - Uph[j]));
                    }
                    else
                    {
                        dQ += Q[i] + Um[i] * B[i, i];
                    }
                }

                res[position++] = dQ;
            }


            return res;
        }

        #endregion Additional

    }
}
