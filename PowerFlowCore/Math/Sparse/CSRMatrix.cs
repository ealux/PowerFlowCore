using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace PowerFlowCore.Algebra
{
    public class CSRMatrix
    {
        public const double Prec = 1e-16;

        public int Rows;
        public int Cols;
        public int NNZ;

        public double[] Values;
        public int[] ColIndex;
        public int[] RowPtr;

        /// <summary>
        /// Return i-th diagonal element
        /// </summary>
        public double this[int i]
        {
            get
            {
                int ind = Array.IndexOf(ColIndex, i, RowPtr[i], RowPtr[i + 1] - RowPtr[i]);
                if (ind == -1)
                    return 0.0;
                else
                    return Values[ind];
            }
        }


        #region Constructor

        private CSRMatrix()
        { }

        private CSRMatrix(int rows, int cols, int nnz)
        {
            Rows = rows;
            Cols = cols;
            NNZ = nnz;

            Values = new double[nnz];
            ColIndex = new int[nnz];
            RowPtr = new int[rows + 1];
        }

        private CSRMatrix(CSRMatrix matrix)
        {
            Rows = matrix.Rows;
            Cols = matrix.Cols;
            NNZ = matrix.NNZ;

            Values = new double[matrix.Values.Length];
            ColIndex = new int[matrix.ColIndex.Length];
            RowPtr = new int[matrix.RowPtr.Length];

            matrix.Values.CopyTo(Values, 0);
            matrix.ColIndex.CopyTo(ColIndex, 0);
            matrix.RowPtr.CopyTo(RowPtr, 0);
        }

        public CSRMatrix(double[,] matrix)
        {
            Rows = matrix.GetLength(0);
            Cols = matrix.GetLength(1);

            RowPtr = new int[Rows + 1];

            var tmpVals = new List<double>();
            var tmpColInds = new List<int>();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (Math.Abs(matrix[i, j]) > Prec)
                    {
                        tmpVals.Add(matrix[i, j]);
                        tmpColInds.Add(j);
                        NNZ++;
                    }
                }
                RowPtr[i + 1] = NNZ;
            }
            ColIndex = tmpColInds.ToArray();
            Values = tmpVals.ToArray();
        }

        #endregion Constructor

        #region Create

        public static CSRMatrix Eye(int n)
        {
            var res = new CSRMatrix(n, n, n);

            for (int i = 0; i < n; i++)
            {
                res.Values[i] = 1;
                res.ColIndex[i] = i;
                res.RowPtr[i + 1] = i + 1;
            }

            return res;
        }

        public static CSRMatrix CreateFromRows(SparseVector[] rows)
        {
            var cnt = rows[0].Indexes.Length;

            for (int i = 1; i < rows.Length; i++)
            {
                if(rows[i].Length != rows[0].Length)
                    throw new ArgumentException("Rows have different length vectors!");
                cnt += rows[i].Indexes.Length;
            }

            var res = new CSRMatrix(rows.Length, rows[0].Length, 0);

            res.ColIndex = new int[cnt];
            res.Values = new double[cnt];
            res.RowPtr = new int[res.Rows + 1];

            for (int i = 0, offset = 0; i < res.Rows; i++)
            {
                Array.Copy(rows[i].Indexes, 0, res.ColIndex, offset, rows[i].Indexes.Length);
                Array.Copy(rows[i].Values, 0, res.Values, offset, rows[i].Values.Length);
                res.NNZ += rows[i].Indexes.Length;
                res.RowPtr[i + 1] = res.NNZ;

                offset += rows[i].Indexes.Length;
            }

            return res;
        }

        public static CSRMatrix CreateFromRows(Dictionary<int, double>[] rows, int columns)
        {
            if (rows.Length == 0)
                throw new ArgumentException("Rows have no values!");
            if (columns <= 0)
                throw new ArgumentException("Columns count is 0!");

            var res = new CSRMatrix(rows.Length, columns, 0);

            var tmpColInds = new List<int>();
            var tmpVals = new List<double>();
            var tmpRowPtr = new int[res.Rows + 1];

            for (int i = 0; i < res.Rows; i++)
            {
                tmpColInds.AddRange(rows[i].Keys);
                tmpVals.AddRange(rows[i].Values);
                res.NNZ += rows[i].Count;
                tmpRowPtr[i + 1] = res.NNZ;
            }

            res.ColIndex = tmpColInds.ToArray();
            res.Values = tmpVals.ToArray();
            res.RowPtr = tmpRowPtr;

            return res;
        }

        public static CSRMatrix CreateFromRows(IList<SparseVector> rows)
        {
            if (rows.Any(r => r.Length != rows[0].Length))
                throw new ArgumentException("Rows have different length vectors!");

            var res = new CSRMatrix(rows.Count, rows[0].Length, 0);

            var tmpColInds = new List<int>();
            var tmpVals = new List<double>();
            var tmpRowPtr = new int[res.Rows + 1];

            for (int i = 0; i < res.Rows; i++)
            {
                tmpColInds.AddRange(rows[i].Indexes);
                tmpVals.AddRange(rows[i].Values);
                res.NNZ += rows[i].Indexes.Length;
                tmpRowPtr[i + 1] = res.NNZ;
            }

            res.ColIndex = tmpColInds.ToArray();
            res.Values = tmpVals.ToArray();
            res.RowPtr = tmpRowPtr;

            return res;
        }

        #endregion Create

        #region Transform

        /// <summary>
        /// Create dence matrix from CSR structure
        /// </summary>
        /// <returns>Dence variant of CSR matrix</returns>
        public double[,] ToDence()
        {
            var res = new double[Rows, Cols];

            Parallel.For(0, Rows, i =>
            {
                for (int j = RowPtr[i]; j < RowPtr[i + 1]; j++)
                {
                    res[i, ColIndex[j]] = Values[j];
                }
            });

            return res;
        }

        public double[,] ToDiagonal()
        {
            var diag = GetDiagonal();
            var res = new double[Rows, Cols];

            for (int i = 0; i < Rows; i++)
            {
                res[i, i] = diag[i];
            }

            return res;
        }

        internal bool Resize(int size)
        {
            if (size <= 0)
            {
                size = this.RowPtr[Rows];
            }

            Array.Resize(ref this.ColIndex, size);
            Array.Resize(ref this.Values, size);

            return true;
        }

        public CSRMatrix Transpose()
        {
            var res = new CSRMatrix(Cols, Rows, NNZ);

            res.RowPtr = new int[Cols + 1];
            res.ColIndex = new int[NNZ];
            res.Values = new double[NNZ];

            for (int n = 0; n < NNZ; n++)
            {
                res.RowPtr[ColIndex[n]]++;
            }
            for (int col = 0, cumsum = 0; col < Cols; col++)
            {
                int temp = res.RowPtr[col];
                res.RowPtr[col] = cumsum;
                cumsum += temp;
            }
            res.RowPtr[Cols] = NNZ;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = RowPtr[i]; j < RowPtr[i + 1]; j++)
                {
                    int col = ColIndex[j];
                    int dest = res.RowPtr[col];

                    res.ColIndex[dest] = i;
                    res.Values[dest] = Values[j];

                    res.RowPtr[col]++;
                }
            }
            for (int col = 0, last = 0; col <= Cols; col++)
            {
                int temp = res.RowPtr[col];
                res.RowPtr[col] = last;
                last = temp;
            }

            return res;
        }

        public CSCMatrix ToCSC()
        {
            var res = new CSCMatrix(Cols, Rows, NNZ);

            var T = this.Transpose();
            res.ColPtr = T.RowPtr;
            res.Values = T.Values;
            res.RowIndex = T.ColIndex;

            return res;
        }

        #endregion

        #region Get components

        public double[] GetRow(int index)
        {
            if (index < 0 || index >= Rows)
                throw new ArgumentOutOfRangeException(nameof(index));

            double[] res = new double[Cols];
            int nonZeroCount = RowPtr[index + 1] - RowPtr[index];
            int position = RowPtr[index];

            for (int j = 0; j < nonZeroCount; j++)
            {
                res[ColIndex[position + j]] = Values[position + j];
            }
            return res;
        }

        public double[] GetColumn(int index)
        {
            if (index < 0 || index >= Cols)
                throw new ArgumentOutOfRangeException(nameof(index));

            double[] res = new double[Rows];
            List<int> valInds = new List<int>();

            for (int i = Array.IndexOf(ColIndex, index); i > -1; i = Array.IndexOf(ColIndex, index, i + 1))
            {
                valInds.Add(i);
            }

            foreach (var ind in valInds)
            {
                for (int i = 1; i < RowPtr.Length; i++)
                {
                    if (RowPtr[i] < ind)
                        continue;
                    else if (RowPtr[i] > ind)
                    {
                        res[i - 1] = Values[ind];
                        break;
                    }
                    else if (RowPtr[i] == ind)
                    {
                        res[i] = Values[ind];
                        break;
                    }
                }
            }

            return res;
        }

        public double[] GetDiagonal()
        {
            double[] res = new double[Rows];

            Parallel.For(0, Rows, i =>
            {
                var k = RowPtr[i + 1] - RowPtr[i];
                int j = Array.IndexOf(ColIndex, i, RowPtr[i], k);

                if (j != -1)
                    res[i] = Values[j];
            });

            return res;
        }

        #endregion Get components

        #region Manipulations

        public CSRMatrix PointwiseInverse()
        {
            var res = new CSRMatrix(this);

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
                for (max = 0.0, p = RowPtr[j]; p < RowPtr[j + 1]; p++)
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
                for (int i = RowPtr[j]; i < RowPtr[j + 1]; i++)
                {
                    cumsum[ColIndex[i]] += Math.Abs(Values[i]);
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

        public CSRMatrix Multiply(double scalar)
        {
            if (scalar == 0.0)
                throw new ArgumentException($"Scalar has 0.0 value!", nameof(scalar));

            var res = new CSRMatrix(this);

            Parallel.For(0, res.Values.Length, i => res.Values[i] *= scalar);

            return res;
        }

        public double[] Multiply(double[] vector)
        {
            if (Cols != vector.Length)
                throw new ArgumentException($"Invalid length of input vector. Length must be: {Cols}", nameof(vector));
            var res = new double[Rows];

            Parallel.For(0, Rows, i =>
            {
                for (int j = RowPtr[i]; j < RowPtr[i + 1]; j++)
                {
                    int k = ColIndex[j];
                    res[i] += Values[j] * vector[k];
                }
            });

            return res;
        }

        public double[] MultiplyAdd(double[] vector, double[] addVector)
        {
            if (Cols != vector.Length || addVector.Length != vector.Length)
                throw new ArgumentException($"Invalid length of input vectors. Length must be: {Cols}", nameof(vector));
            var res = new double[Rows];
            addVector.CopyTo(res, 0);

            Parallel.For(0, Rows, i =>
            {
                for (int j = RowPtr[i]; j < RowPtr[i + 1]; j++)
                {
                    int k = ColIndex[j];
                    res[i] += Values[j] * vector[k];
                }
            });

            return res;
        }

        public double[] TransposeMultiply(double[] vector)
        {
            if (Rows != vector.Length)
                throw new ArgumentException($"Invalid length of input vector. Length must be: {Rows}", nameof(vector));
            var res = new double[Cols];

            Parallel.For(0, Cols, i =>
            {
                for (int j = RowPtr[i]; j < RowPtr[i + 1]; j++)
                {
                    int k = ColIndex[j];
                    res[k] += Values[j] * vector[i];
                }
            });

            return res;
        }

        public double[] TransposeMultiplyAdd(double[] vector, double[] addVector)
        {
            if (Rows != vector.Length || addVector.Length != vector.Length)
                throw new ArgumentException($"Invalid length of input vectors. Length must be: {Rows}", nameof(vector));
            var res = new double[Cols];
            addVector.CopyTo(res, 0);

            Parallel.For(0, Cols, i =>
            {
                for (int j = RowPtr[i]; j < RowPtr[i + 1]; j++)
                {
                    int k = ColIndex[j];
                    res[k] += Values[j] * vector[i];
                }
            });

            return res;
        }

        public double[,] Multiply(double[,] matrix)
        {
            if (Cols != matrix.GetLength(0))
                throw new ArgumentException($"Invalid dimension of input matrix", nameof(matrix));
            var res = new double[Rows, matrix.GetLength(1)];

            Parallel.For(0, matrix.GetLength(1), j =>
            {
                var k = this.Multiply(matrix.GetColumn(j));
                for (int i = 0; i < k.Length; i++)
                {
                    res[i, j] = k[i];
                }
            });

            return res;
        }

        public double[,] Multiply(CSRMatrix matrix)
        {
            if (Cols != matrix.Rows)
                throw new ArgumentException($"Invalid dimension of input matrix", nameof(matrix));
            var res = new double[Rows, matrix.Cols];

            Parallel.For(0, matrix.Cols, j =>
            {
                var k = Multiply(matrix.GetColumn(j));
                for (int i = 0; i < k.Length; i++)
                {
                    res[i, j] = k[i];
                }
            });

            return res;
        }

        #endregion Arithmetics
    }
}