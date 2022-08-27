using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace PowerFlowCore.Algebra
{
    internal static class Matrix
    {
        internal static double[] Solve(this double[,] A, double[] B)
        {
            int n = A.GetLength(0);
            int[] perm;
            double[,] luMatrix;
            (luMatrix, perm, _) = A.MatrixDecompose();
            double[] bp = new double[B.Length];
            for (int i = 0; i < n; ++i)
                bp[i] = B[perm[i]];
            double[] x = HelperSolve(luMatrix, bp);
            return x;
        }

        static (double[,] C, int[] perm, int toggle) MatrixDecompose(this double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = matrix.Copy();
            int[] perm = Enumerable.Range(0, n).ToArray();
            int toggle = 1;

            for (int j = 0; j < n - 1; ++j)
            {
                double colMax = Math.Abs(result[j,j]);
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


        static double[] HelperSolve(double[,] luMatrix, double[] b)
        {
            int n = luMatrix.GetLength(0);
            double[] x = b.Copy();
            for (int i = 1; i < n; ++i)
            {
                for (int j = 0; j < i; ++j)
                    x[i] -= luMatrix[i, j] * x[j];
            }
            x[n - 1] /= luMatrix[n - 1, n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i, j] * x[j];
                x[i] = sum / luMatrix[i, i];
            }
            return x;
        }

        public static double[,] Inverse(this double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = matrix.Copy();
            int[] perm;
            double[,] lum;
            (lum, perm, _) = matrix.MatrixDecompose(); 
            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }
                double[] x = HelperSolve(lum, b);
                for (int j = 0; j < n; ++j)
                    result[j, i] = x[j];
            }
            return result;
        }


        public static double Det(this double[,] matrix)
        {
            int toggle;
            double[,] lum;
            (lum, _, toggle) = matrix.MatrixDecompose();
            if (lum == null)
                throw new Exception("Unable to compute Determinant");
            double result = toggle;
            for (int i = 0; i < lum.Length; ++i)
                result *= lum[i, i];
            return result;
        }
    }
}
