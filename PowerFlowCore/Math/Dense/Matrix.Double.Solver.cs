using System;
using System.Linq;
using System.Threading.Tasks;

namespace PowerFlowCore.Algebra
{
    public static partial class MatrixDouble
    {
        /// <summary>
        /// Solves linear system of type Ax=B
        /// </summary>
        /// <param name="A">Source coefficients matrix</param>
        /// <param name="B">Free-values vector</param>
        /// <returns>Vector of system solutions</returns>
        public static double[] Solve(this double[,] A, double[] B)
        {
            int n = A.GetLength(0);
            int[] perm;
            double[,] lum;
            (lum, perm, _) = A.Decompose();
            double[] bp = perm.Map(i => B[i]);
            return HelperSolve(lum, bp);
        }

        /// <summary>
        /// LU source <paramref name="matrix"/> decomposition
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <returns>
        /// <para>C - LU decomposed matrix</para>
        /// <para>perm - vector of rows permutaion indexes</para>
        /// <para>toggle - sign chanhe on decomposition</para>
        /// </returns>
        static (double[,] C, int[] perm, int toggle) Decompose(this double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = matrix.Copy();
            int[] perm = Enumerable.Range(0, n).ToArray();
            int toggle = 1;

            for (int j = 0; j < n - 1; ++j)
            {
                double colMax = Math.Abs(result[j, j]);
                int pRow = j;
                for (int i = j + 1; i < n; ++i)
                {
                    if (result[i, j] > colMax)
                    {
                        colMax = result[i, j];
                        pRow = i;
                    }
                }
                if (pRow != j)
                {
                    result.SwapRow(pRow, j);
                    int tmp = perm[pRow];
                    perm[pRow] = perm[j];
                    perm[j] = tmp;
                    toggle = -toggle;
                }
                Parallel.For(j + 1, n, i =>
                {
                    result[i, j] /= result[j, j];
                    for (int k = j + 1; k < n; ++k)
                        result[i, k] -= result[i, j] * result[j, k];
                });                
            } 

            return (result, perm, toggle);
        }

        /// <summary>
        /// Hepler method for matrix operations
        /// </summary>
        /// <param name="lum">LU decomposed matrix</param>
        /// <param name="b">Vector to solve with</param>
        static double[] HelperSolve(double[,] lum, double[] b)
        {
            int n = lum.GetLength(0);
            double[] x = b.Copy();
                        
            for (int i = 1; i < n; ++i)
            {
                for (int j = 0; j < i; ++j)
                    x[i] -= lum[i, j] * x[j];
            }
            x[n - 1] /= lum[n - 1, n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= lum[i, j] * x[j];
                x[i] = sum / lum[i, i];
            }
            return x;
        }

        /// <summary>
        /// Inverse source <paramref name="matrix"/>
        /// </summary>
        /// <remarks>If <paramref name="matrix"/> is not square or Determinant is equals zero - throw <see cref="Exception"/></remarks>
        /// <param name="matrix">Source matrix</param>
        public static double[,] Inverse(this double[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            if (n != m) throw new Exception("Matrix is not square. Unable to inverse.");

            double[,] result = matrix.Copy();
            int[] perm;
            double[,] lum;
            int toggle;
            (lum, perm, toggle) = matrix.Decompose();

            // Determinant ckecks
            double det = toggle;
            for (int i = 0; i < lum.RowsCount(); ++i)
                det *= lum[i, i];
            if (det == 0d) throw new Exception("Matrix determinant is equals zero. Unable to inverse.");

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                Parallel.For(0, n, j =>
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                });
                double[] x = HelperSolve(lum, b);
                Parallel.For(0, n, j => result[j, i] = x[j]);
            }
            return result;
        }

        /// <summary>
        /// Find source <paramref name="matrix"/> determinant value
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        public static double Det(this double[,] matrix)
        {
            int toggle;
            double[,] lum;
            (lum, _, toggle) = matrix.Decompose();
            if (lum == null)
                throw new Exception("Unable to compute Determinant");
            double result = toggle;
            for (int i = 0; i < lum.RowsCount(); ++i)
                result *= lum[i, i];
            return result;
        }
    }
}
