using System;

namespace PowerFlowCore.Algebra
{
    internal class SparseLU
    {
        readonly int n;

        SymbolicFactorization S;
        CSCMatrix L, U;
        int[] pinv; // partial pivoting

        double[] temp; // workspace

        #region Static methods

        /// <summary>
        /// Creates a LU factorization.
        /// </summary>
        /// <param name="A">Column-compressed matrix, must be square.</param>
        /// <param name="order">Ordering method to use (natural or A+A').</param>
        /// <param name="tol">Partial pivoting tolerance (form 0.0 to 1.0).</param>
        /// <param name="progress">Report progress (range from 0.0 to 1.0).</param>
        public static SparseLU Create(CSCMatrix A, int order, double tol)
        {
            return Create(A, AMD.Generate(A, order), tol);
        }

        /// <summary>
        /// Creates a LU factorization.
        /// </summary>
        /// <param name="A">Column-compressed matrix, must be square.</param>
        /// <param name="p">Fill-reducing column permutation.</param>
        /// <param name="tol">Partial pivoting tolerance (form 0.0 to 1.0).</param>
        public static SparseLU Create(CSCMatrix A, int[] p, double tol)
        {     
            int n = A.Cols;

            // Ensure tol is in range.
            tol = Math.Min(Math.Max(tol, 0.0), 1.0);

            var C = new SparseLU(n);

            // Ordering and symbolic analysis
            C.SymbolicAnalysis(A, p);

            // Numeric Cholesky factorization
            C.Factorize(A, tol);

            return C;
        }

        #endregion

        private SparseLU(int n)
        {
            this.n = n;
            this.temp = new double[n];
        }

        /// <summary>
        /// Gets the number of nonzeros in both L and U factors together.
        /// </summary>
        public int NonZerosCount
        {
            get { return (L.NNZ + U.NNZ - n); }
        }

        /// <summary>
        /// Solves a system of linear equations, <c>Ax = b</c>.
        /// </summary>
        /// <param name="input">The right hand side vector, <c>b</c>.</param>
        /// <param name="result">The left hand side vector, <c>x</c>.</param>
        public void Solve(double[] input, double[] result)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (result == null) throw new ArgumentNullException(nameof(result));

            var x = this.temp;

            Permutations.ApplyInverse(pinv, input, x, n); // x = b(p)

            SolveLower(L, x); // x = L\x.

            SolveUpper(U, x); // x = U\x.

            Permutations.ApplyInverse(S.q, x, result, n); // b(q) = x
        }

        /// <summary>
        /// Solves a system of linear equations, <c>A'x = b</c>.
        /// </summary>
        /// <param name="input">The right hand side vector, <c>b</c>.</param>
        /// <param name="result">The left hand side vector, <c>x</c>.</param>
        public void SolveTranspose(double[] input, double[] result)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (result == null) throw new ArgumentNullException(nameof(result));

            var x = this.temp;

            Permutations.Apply(S.q, input, x, n); // x = Q'*b

            SolveUpperTranspose(U, x); // x = U'\x.

            SolveLowerTranspose(L, x); // x = L'\x.

            Permutations.Apply(pinv, x, result, n); // b = P'*x
        }

        /// <summary>
        /// [L,U,pinv] = lu(A, [q lnz unz]). lnz and unz can be guess.
        /// </summary>
        private void Factorize(CSCMatrix A, double tol)
        {
            int[] q = S.q;

            int i;
            int lnz = S.lnz;
            int unz = S.unz;

            this.L = new CSCMatrix(n, n, lnz);
            this.U = new CSCMatrix(n, n, unz);
            this.pinv = new int[n];

            // Workspace
            var x = this.temp;
            var xi = new int[2 * n];

            for (i = 0; i < n; i++)
            {
                // No rows pivotal yet.
                pinv[i] = -1;
            }

            lnz = unz = 0;

            int ipiv, top, p, col;
            double pivot;
            double a, t;

            int[] li, ui;
            int[] lp = L.ColPtr;
            int[] up = U.ColPtr;
            double[] lx, ux;

            double current = 0.0;
            double step = n / 100.0;

            // Now compute L(:,k) and U(:,k)
            for (int k = 0; k < n; k++)
            {
                // Triangular solve
                lp[k] = lnz; // L(:,k) starts here
                up[k] = unz; // U(:,k) starts here

                if (lnz + n > L.Values.Length) L.Resize(2 * L.Values.Length + n);
                if (unz + n > U.Values.Length) U.Resize(2 * U.Values.Length + n);

                li = L.RowIndex;
                ui = U.RowIndex;
                lx = L.Values;
                ux = U.Values;
                col = q != null ? (q[k]) : k;
                top = SolveSp(L, A, col, xi, x, pinv, true);  // x = L\A(:,col)

                // Find pivot
                ipiv = -1;
                a = -1;
                for (p = top; p < n; p++)
                {
                    i = xi[p]; // x(i) is nonzero
                    if (pinv[i] < 0) // Row i is not yet pivotal
                    {
                        if ((t = Math.Abs(x[i])) > a)
                        {
                            a = t; // Largest pivot candidate so far
                            ipiv = i;
                        }
                    }
                    else // x(i) is the entry U(pinv[i],k)
                    {
                        ui[unz] = pinv[i];
                        ux[unz++] = x[i];
                    }
                }

                if (ipiv == -1 || a <= 0.0)
                {
                    //throw new Exception("No pivot element found.");
                }

                if (pinv[col] < 0 && Math.Abs(x[col]) >= a * tol)
                {
                    ipiv = col;
                }

                // Divide by pivot
                pivot = x[ipiv]; // the chosen pivot
                ui[unz] = k; // last entry in U(:,k) is U(k,k)
                ux[unz++] = pivot;
                pinv[ipiv] = k; // ipiv is the kth pivot row
                li[lnz] = ipiv; // first entry in L(:,k) is L(k,k) = 1
                lx[lnz++] = 1.0;
                for (p = top; p < n; p++) // L(k+1:n,k) = x / pivot
                {
                    i = xi[p];
                    if (pinv[i] < 0) // x(i) is an entry in L(:,k)
                    {
                        li[lnz] = i; // save unpermuted row in L
                        lx[lnz++] = x[i] / pivot; // scale pivot column
                    }
                    x[i] = 0.0; // x [0..n-1] = 0 for next k
                }
            }

            // Finalize L and U
            lp[n] = lnz;
            up[n] = unz;
            li = L.RowIndex; // fix row indices of L for final pinv
            for (p = 0; p < lnz; p++)
            {
                li[p] = pinv[li[p]];
            }

            // Remove extra space from L and U
            L.Resize(0);
            U.Resize(0);
        }

        /// <summary>
        /// Symbolic ordering and analysis for LU.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="p">Permutation.</param>
        private void SymbolicAnalysis(CSCMatrix A, int[] p)
        {
            var sym = this.S = new SymbolicFactorization();

            // Fill-reducing ordering
            sym.q = p;

            // Guess nnz(L) and nnz(U)
            sym.unz = sym.lnz = 4 * (A.ColPtr[n]) + n;
        }

        /// <summary>
        /// Solve Gx=b(:,k), where G is either upper (lo=false) or lower (lo=true)
        /// triangular.
        /// </summary>
        /// <param name="G">lower or upper triangular matrix in column-compressed form</param>
        /// <param name="B">right hand side, b=B(:,k)</param>
        /// <param name="k">use kth column of B as right hand side</param>
        /// <param name="xi">size 2*n, nonzero pattern of x in xi[top..n-1]</param>
        /// <param name="x">size n, x in x[xi[top..n-1]]</param>
        /// <param name="pinv">mapping of rows to columns of G, ignored if null</param>
        /// <param name="lo">true if lower triangular, false if upper</param>
        /// <returns>top, -1 if error</returns>
        private int SolveSp(CSCMatrix G, CSCMatrix B, int k, int[] xi, double[] x, int[] pinv, bool lo)
        {
            if (xi == null || x == null) return -1;

            var gp = G.ColPtr;
            var gi = G.RowIndex;
            var gx = G.Values;

            var bp = B.ColPtr;
            var bi = B.RowIndex;
            var bx = B.Values;

            int n = G.Cols;

            // xi[top..n-1]=Reach(B(:,k))
            int top = GraphHelper.Reach(gp, gi, bp, bi, n, k, xi, pinv);

            int j, J, p, q, px;

            for (p = top; p < n; p++)
            {
                x[xi[p]] = 0; // clear x
            }

            for (p = bp[k]; p < bp[k + 1]; p++)
            {
                x[bi[p]] = bx[p]; // scatter B
            }

            for (px = top; px < n; px++)
            {
                j = xi[px]; // x(j) is nonzero
                J = pinv != null ? (pinv[j]) : j; // j maps to col J of G
                if (J < 0) continue; // column J is empty
                x[j] /= gx[lo ? (gp[J]) : (gp[J + 1] - 1)]; // x(j) /= G(j,j)
                p = lo ? (gp[J] + 1) : (gp[J]); // lo: L(j,j) 1st entry
                q = lo ? (gp[J + 1]) : (gp[J + 1] - 1); // up: U(j,j) last entry
                for (; p < q; p++)
                {
                    x[gi[p]] -= gx[p] * x[j]; // x(i) -= G(i,j) * x(j)
                }
            }

            // Return top of stack.
            return top;
        }

        #region SolverHelpers
        
        /// <summary>
        /// Solve a lower triangular system by forward elimination, Lx=b.
        /// </summary>
        /// <param name="L">Lower triangulated sparse matrix</param>
        /// <param name="x">Temporal solve vector</param>
        /// <returns></returns>
        public static void SolveLower(CSCMatrix L, double[] x)
        {
            int p, j, k, n = L.Cols;

            var lp = L.ColPtr;
            var li = L.RowIndex;
            var lx = L.Values;

            for (j = 0; j < n; j++)
            {
                x[j] /= lx[lp[j]];

                k = lp[j + 1];

                for (p = lp[j] + 1; p < k; p++)
                {
                    x[li[p]] -= lx[p] * x[j];
                }
            }
        }
        

        /// <summary>
        /// Solve L'x=b where x and b are dense.
        /// </summary>
        /// <param name="L">Lower triangulated sparse matrix</param>
        /// <param name="x">Temporal solve vector</param>
        /// <returns></returns>
        public static void SolveLowerTranspose(CSCMatrix L, double[] x)
        {
            int p, j, k, n = L.Cols;

            var lp = L.ColPtr;
            var li = L.RowIndex;
            var lx = L.Values;

            for (j = n - 1; j >= 0; j--)
            {
                k = lp[j + 1];

                for (p = lp[j] + 1; p < k; p++)
                {
                    x[j] -= lx[p] * x[li[p]];
                }

                x[j] /= lx[lp[j]];
            }
        }


        /// <summary>
        /// Solve an upper triangular system by backward elimination, Ux=b.
        /// </summary>
        /// <param name="U">Upper triangulated sparse matrix</param>
        /// <param name="x">Result vector</param>
        /// <returns></returns>
        public static void SolveUpper(CSCMatrix U, double[] x)
        {
            int p, j, k, n = U.Cols;

            var up = U.ColPtr;
            var ui = U.RowIndex;
            var ux = U.Values;

            for (j = n - 1; j >= 0; j--)
            {
                x[j] /= ux[up[j + 1] - 1];

                k = up[j + 1] - 1;

                for (p = up[j]; p < k; p++)
                {
                    x[ui[p]] -= ux[p] * x[j];
                }
            }
        }


        /// <summary>
        /// Solve U'x=b where x and b are dense.
        /// </summary>
        /// <param name="U">Upper triangulated sparse matrix</param>
        /// <param name="x">Result vector</param>
        /// <returns></returns>
        public static void SolveUpperTranspose(CSCMatrix U, double[] x)
        {
            int p, j, k, n = U.Cols;

            var up = U.ColPtr;
            var ui = U.RowIndex;
            var ux = U.Values;

            for (j = 0; j < n; j++)
            {
                k = up[j + 1] - 1;

                for (p = up[j]; p < k; p++)
                {
                    x[j] -= ux[p] * x[ui[p]];
                }

                x[j] /= ux[up[j + 1] - 1];
            }
        }

        #endregion
    }
}
