using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PowerFlowCore.Algebra
{
    public class CSCMatrix
    {
        public const double Prec = 1e-6;

        public int Rows;
        public int Cols;
        public int NNZ;

        public double[] Values;
        public int[] RowIndex;
        public int[] ColPtr;

        /// <summary>
        /// Return i-th diagonal element
        /// </summary>
        public double this[int i]
        {
            get
            {
                int ind = Array.IndexOf(RowIndex, i, ColPtr[i], ColPtr[i + 1] - ColPtr[i]);
                if (ind == -1)
                    return 0.0;
                else
                    return Values[ind];
            }
        }



        #region Constructor

        public CSCMatrix()
        {            
        }

        public CSCMatrix(int rows, int cols, int nnz)
        {
            Rows = rows;
            Cols = cols;
            NNZ = nnz;

            Values = new double[nnz];
            RowIndex = new int[nnz];
            ColPtr = new int[cols + 1];
        }

        public CSCMatrix(CSCMatrix matrix)
        {
            Rows = matrix.Rows;
            Cols = matrix.Cols;
            NNZ = matrix.NNZ;

            Values = new double[matrix.Values.Length];
            RowIndex = new int[matrix.RowIndex.Length];
            ColPtr = new int[matrix.ColPtr.Length];

            matrix.Values.CopyTo(Values, 0);
            matrix.RowIndex.CopyTo(RowIndex, 0);
            matrix.ColPtr.CopyTo(ColPtr, 0);
        }

        public CSCMatrix(double[,] matrix)
        {
            Rows = matrix.GetLength(0);
            Cols = matrix.GetLength(1);

            ColPtr = new int[Cols + 1];

            var tmpVals = new List<double>();
            var tmpRowInds = new List<int>();

            for (int j = 0; j < Cols; j++)
            {        
                for (int i = 0; i < Rows; i++)
                {
                    if (Math.Abs(matrix[i, j]) > Prec)
                    {
                        tmpVals.Add(matrix[i, j]);
                        tmpRowInds.Add(i);
                        NNZ++;
                    }
                }
                ColPtr[j+1] = NNZ;
            }
            RowIndex = tmpRowInds.ToArray();
            Values = tmpVals.ToArray();
        }

        #endregion

        #region Create

        public static CSCMatrix Eye(int n)
        {
            var res = new CSCMatrix(n, n, n);

            for (int i = 0; i < n; i++)
            {
                res.Values[i] = 1;
                res.RowIndex[i] = i;
                res.ColPtr[i] = i + 1;
            }

            return res;
        }

        #endregion Create

        #region Transform

        /// <summary>
        /// Create dence matrix from CSC structure
        /// </summary>
        /// <returns>Dence variant of CSC matrix</returns>
        public double[,] ToDence()
        {
            var res = new double[Rows, Cols];

            Parallel.For(0, Cols, j =>
            {
                for (int i = ColPtr[j]; i < ColPtr[j + 1]; i++)
                {
                    res[RowIndex[i], j] = Values[i];
                }
            });

            return res;
        }

        public double[,] ToDiagonal()
        {
            var diag = this.GetDiagonal();
            var res = new double[Rows, Cols];

            for (int i = 0; i < Rows; i++)
            {
                res[i, i] = diag[i];
            }

            return res;
        }

        public CSCMatrix Transpose()
        {
            var res = new CSCMatrix(Cols, Rows, NNZ);

            res.ColPtr = new int[Cols + 1];
            res.RowIndex = new int[NNZ];
            res.Values = new double[NNZ];

            for (int n = 0; n < NNZ; n++)
            {
                res.ColPtr[RowIndex[n]]++;
            }
            for (int col = 0, cumsum = 0; col < Cols; col++)
            {
                int temp = res.ColPtr[col];
                res.ColPtr[col] = cumsum;
                cumsum += temp;
            }
            res.ColPtr[Cols] = NNZ;
            for (int i = 0; i < Cols; i++)
            {
                for (int j = ColPtr[i]; j < ColPtr[i + 1]; j++)
                {
                    int row = RowIndex[j];
                    int dest = res.ColPtr[row];

                    res.RowIndex[dest] = i;
                    res.Values[dest] = Values[j];

                    res.ColPtr[row]++;
                }
            }
            for (int col = 0, last = 0; col <= Cols; col++)
            {
                int temp = res.ColPtr[col];
                res.ColPtr[col] = last;
                last = temp;
            }

            return res;
        }

        internal bool Resize(int size)
        {
            if (size <= 0)
            {
                size = this.ColPtr[Cols];
            }

            Array.Resize(ref this.RowIndex, size);
            Array.Resize(ref this.Values, size);

            return true;
        }

        #endregion

        #region Get components

        public double[] GetRow(int index)
        {
            if (index < 0 || index >= Rows)
                throw new ArgumentOutOfRangeException(nameof(index));

            double[] res = new double[Cols];
            List<int> valInds = new List<int>();

            for (int i = Array.IndexOf(RowIndex, index); i > -1; i = Array.IndexOf(RowIndex, index, i + 1))
            {
                valInds.Add(i);
            }

            foreach (var ind in valInds)
            {
                for (int i = 1; i < ColPtr.Length; i++)
                {
                    if (ColPtr[i] < ind)
                        continue;
                    else if (ColPtr[i] > ind)
                    {
                        res[i - 1] = Values[ind];
                        break;
                    }
                    else if (ColPtr[i] == ind)
                    {
                        res[i] = Values[ind];
                        break;
                    }
                }
            }
            return res;
        }

        public double[] GetColumn(int index)
        {
            if (index < 0 || index >= Cols)
                throw new ArgumentOutOfRangeException(nameof(index));

            double[] res = new double[Rows];
            int nonZeroCount = ColPtr[index + 1] - ColPtr[index];
            int position = ColPtr[index];

            for (int j = 0; j < nonZeroCount; j++)
            {
                res[RowIndex[position + j]] = Values[position + j];
            }            

            return res;
        }

        public double[] GetDiagonal()
        {
            double[] res = new double[Rows];

            Parallel.For(0, Rows, i =>
            {
                var k = ColPtr[i + 1] - ColPtr[i];
                int j = Array.IndexOf(RowIndex, i, ColPtr[i], k);

                if (j != -1)
                    res[i] = Values[j];
            });

            return res;
        }

        #endregion Get components

        #region Manipulations

        public CSCMatrix PointwiseInverse()
        {
            var res = new CSCMatrix(this);

            Parallel.For(0, res.Values.Length, i => res.Values[i] = 1 / res.Values[i]);

            return res;
        }

        #endregion

        #region Norm

        public double L1Norm()
        {
            double norm = 0.0, max;
            int p;

            for (int j = 0; j < Cols; j++)
            {
                for (max = 0.0, p = ColPtr[j]; p < ColPtr[j+1]; p++)
                {
                    max += Math.Abs(Values[p]);
                }
                norm = Math.Max(norm, max);
            }
            return norm;
        }

        public double InfinityNorm()
        {
            double norm = 0.0;

            var cumsum = new double[Rows];

            for (int j = 0; j < Cols; j++)
            {
                for (int i = ColPtr[j]; i < ColPtr[j + 1]; i++)
                {
                    cumsum[RowIndex[i]] += Math.Abs(Values[i]);
                }
            }

            norm = Math.Max(norm, cumsum.Max());

            return norm;
        }

        public double FrobeniusNorm()
        {
            double sum, norm = 0.0;

            for (int i = 0; i < NNZ; i++)
            {
                sum = Math.Abs(Values[i]);
                norm += sum * sum;
            }

            return Math.Sqrt(norm);
        }

        #endregion

        #region Arithmetics

        public CSCMatrix Multiply(double scalar)
        {
            if (scalar == 0.0)
                throw new ArgumentException($"Scalar has 0.0 value!", nameof(scalar));

            var res = new CSCMatrix(this);

            Parallel.For(0, res.Values.Length, i => res.Values[i] *= scalar);

            return res;
        }

        public double[] Multiply(double[] vector)
        {
            if (this.Cols != vector.Length)
                throw new ArgumentException($"Invalid length of input vector. Length must be: {this.Cols}", nameof(vector));
            var res = new double[Rows];

            Parallel.For(0, Rows, j =>
            {
                for (int i = ColPtr[j]; i < ColPtr[j + 1]; i++)
                {
                    res[RowIndex[i]] += Values[i] * vector[j];
                }
            });

            return res;
        }        

        public double[] MultiplyAdd(double[] vector, double[] addVector)
        {
            if (this.Cols != vector.Length || addVector.Length != vector.Length)
                throw new ArgumentException($"Invalid length of input vectors. Length must be: {this.Cols}", nameof(vector));
            var res = new double[Rows];
            addVector.CopyTo(res, 0);

            Parallel.For(0, Rows, j =>
            {
                for (int i = ColPtr[j]; i < ColPtr[j + 1]; i++)
                {
                    res[RowIndex[i]] += Values[i] * vector[j];
                }
            });

            return res;
        }

        public double[] TransposeMultiply(double[] vector)
        {
            if (this.Rows != vector.Length)
                throw new ArgumentException($"Invalid length of input vector. Length must be: {this.Rows}", nameof(vector));
            var res = new double[vector.Length];

            Parallel.For(0, Cols, i =>
            {
                for (int j = ColPtr[i]; j < ColPtr[i + 1]; j++)
                {
                    res[i] += Values[j] * vector[RowIndex[j]];
                }
            });

            return res;
        }

        public double[] TransposeMultiplyAdd(double[] vector, double[] addVector)
        {
            if (this.Rows != vector.Length || addVector.Length != vector.Length)
                throw new ArgumentException($"Invalid length of input vectors. Length must be: {this.Rows}", nameof(vector));
            var res = new double[vector.Length];
            addVector.CopyTo(res, 0);

            Parallel.For(0, Cols, i =>
            {
                for (int j = ColPtr[i]; j < ColPtr[i + 1]; j++)
                {
                    res[i] += Values[j] * vector[RowIndex[j]];
                }
            });

            return res;
        }



        #endregion
    }
}
