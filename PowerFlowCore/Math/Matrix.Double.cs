using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerFlowCore.Algebra
{
    public static partial class MatrixDouble
    {
        #region Create

        public static double[,] Create(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Arguments are equal or less then zero");

            return new double[rows, cols];
        }

        public static double[,] CreateFromArray(double[,][,] submatricies)
        {
            _ = submatricies ?? throw new ArgumentNullException(nameof(submatricies));

            var rowspans = new int[submatricies.GetLength(0)];
            var colspans = new int[submatricies.GetLength(1)];

            for (int i = 0; i < rowspans.Length; i++)
            {
                for (int j = 0; j < colspans.Length; j++)
                {
                    rowspans[i] = Math.Max(rowspans[i], submatricies[i, j].RowsCount());
                    colspans[j] = Math.Max(colspans[j], submatricies[i, j].ColumnsCount());
                }
            }
            var res = Create(rowspans.Sum(), colspans.Sum());

            int rowOffset = 0;
            for (int i = 0; i < rowspans.Length; i++)
            {
                int colOffset = 0;
                for (int j = 0; j < colspans.Length; j++)
                {
                    SetSubMatrix(res, rowOffset, colOffset, submatricies[i, j]);
                    colOffset += colspans[j];
                }
                rowOffset += rowspans[i];
            }
            return res;
        }

        public static double[,] SameAs(this double[,] mat)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));

            return mat.SubMatrix(0, 0, mat.GetLength(0) - 1, mat.GetLength(1) - 1);
        }

        #endregion

        #region Construct

        public static double[,] ConcatVectorRow(this double[,] target, double[] row)
        {
            _ = row ?? throw new ArgumentNullException(nameof(row));
            _ = target ?? throw new ArgumentNullException(nameof(target));

            var colDim =  target.GetLength(1);
            if (colDim != row.Length) throw new ArgumentException("Matrix column dimention and vector length are not equal", nameof(row));

            var res = Create(target.GetLength(0) + 1, colDim);

            SetSubMatrix(res, 0, 0, target);
            var lastRow = target.GetLength(0);
            for (int j = 0; j < colDim; j++)
                res[lastRow, j] = row[j];

            return res;
        }

        public static double[,] ConcatVectorColumn(double[,] target, double[] column)
        {
            _ = column ?? throw new ArgumentNullException(nameof(column));
            _ = target ?? throw new ArgumentNullException(nameof(target));

            var rowDim = target.GetLength(0);
            if (rowDim != column.Length) throw new ArgumentException("Matrix row dimention and vector length are not equal", nameof(column));

            var res = Create(rowDim, target.GetLength(1) + 1);
            SetSubMatrix(res, 0, 0, target);
            var lastCol = target.GetLength(1);
            for (int j = 0; j < rowDim; j++)
                res[j, lastCol] = column[j];

            return res;
        }

        #endregion

        #region Copy

        public static double[,] Copy(this double[,] mat)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));

            var res = Create(mat.GetLength(0), mat.GetLength(1));

            for (int i = 0; i < mat.GetLength(0); i++)
                for (int j = 0; j < mat.GetLength(1); j++)
                    res[i, j] = mat[i, j];
            return res;
        }

        public static void CopyTo(this double[,] mat, double[,] target)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));
            _ = target ?? throw new ArgumentNullException(nameof(target));

            if (mat.GetLength(0) != target.GetLength(0) || mat.GetLength(1) != target.GetLength(1))
                throw new ArgumentException("Dimensions are not same", nameof(target));

            for (int i = 0; i < mat.GetLength(0); i++)
                for (int j = 0; j < mat.GetLength(1); j++)
                    target[i, j] = mat[i, j];
        }

        #endregion

        #region Dimensions

        public static int RowsCount(this double[,] mat)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));
            return mat.GetLength(0);
        }

        public static int ColumnsCount(this double[,] mat)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));
            return mat.GetLength(1);
        }

        public static bool IsSquare(this double[,] mat)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));

            return mat.GetLength(0) == mat.GetLength(1);
        }

        #endregion

        #region Get components

        public static (int, int) GetDimension(this double[,] mat) => (mat.GetLength(0), mat.GetLength(1));

        public static double[] GetRow(this double[,] mat, int index)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));
            if(index < 0 || index >= mat.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(index));

            var row = new double[mat.ColumnsCount()];

            for (int i = 0; i < row.Length; i++)
                row[i] = mat[index, i];
            return row;
        }
        public static double[] GetColumn(this double[,] mat, int index)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));
            if (index < 0 || index >= mat.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(index));

            var col = new double[mat.RowsCount()];

            for (int i = 0; i < col.Length; i++)
                col[i] = mat[i, index];
            return col;
        }

        public static double[,] SubMatrix(this double[,] mat, int startRow, int startCol, int endRow, int endCol)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));
            if (startRow < 0 || startRow >= mat.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(startRow));
            if (endRow < 0 || endRow >= mat.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(endRow));
            if (startCol < 0 || startCol >= mat.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(startCol));
            if (endCol < 0 || endCol >= mat.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(endCol));

            var rows = endRow - startRow;
            var cols = endCol - startCol;

            var res = Create(rows + 1 , cols + 1);
            for (int i = 0; i <= rows; i++)
                for (int j = 0; j <= cols; j++)
                    res[i, j] = mat[i, j];

            return res;
        }

        public static double[,] GetUpperTriangle(this double[,] mat)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));

            var res = Create(mat.RowsCount(), mat.ColumnsCount());

            Parallel.For(0, mat.RowsCount(), i =>
            {
                for (int j = 0; j < mat.ColumnsCount(); j++)
                {
                    if (j >= i)
                        res[i, j] = mat[i, j];
                }
            });

            return res;
        }
        public static double[,] GetLowerTriangle(this double[,] mat)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));

            var res = Create(mat.RowsCount(), mat.ColumnsCount());

            Parallel.For(0, mat.RowsCount(), i =>
            {
                for (int j = 0; j < mat.ColumnsCount(); j++)
                {
                    if (i >= j)
                        res[i, j] = mat[i, j];
                }
            });

            return res;
        }

        #endregion

        #region Set components

        public static void SetRow(this double[,] target, int rowIndex, double[] row)
        {
            _ = row ?? throw new ArgumentNullException(nameof(row));
            _ = target ?? throw new ArgumentNullException(nameof(target));
            if (rowIndex < 0 || rowIndex >= target.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            if (target.GetLength(1) != row.Length) 
                throw new ArgumentException("Matrix column dimention and vector length are not equal", nameof(row));

            Parallel.For(0, row.Length, j =>
            {
                target[rowIndex, j] = row[j];
            });                
        }

        public static void SetColumn(this double[,] target, int colIndex, double[] col)
        {
            _ = col ?? throw new ArgumentNullException(nameof(col));
            _ = target ?? throw new ArgumentNullException(nameof(target));
            if (colIndex < 0 || colIndex >= target.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(colIndex));
            if (target.GetLength(0) != col.Length)
                throw new ArgumentException("Matrix row dimention and vector length are not equal", nameof(col));

            Parallel.For(0, col.Length, i =>
            {
                target[i, colIndex] = col[i];
            });
                
        }

        internal static void SetSubMatrix(double[,] target, int rowOffset, int colOffset, double[,] submatrix)
        {
            var rows = submatrix.RowsCount();
            var cols = submatrix.ColumnsCount();

            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < cols; j++)
                    target[rowOffset + i, colOffset + j] = submatrix[i, j];
            });                
        }

        #endregion

        #region Manipulate

        public static void SwapRow(this double[,] target, int i, int j)
        {
            if (i < 0 || i >= target.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(i));
            if (j < 0 || j >= target.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(j));

            var tmp_i = target.GetRow(i);
            target.SetRow(i, target.GetRow(j));
            target.SetRow(j, tmp_i);
        }
        public static void SwapColumn(this double[,] target, int i, int j)
        {
            if (i < 0 || i >= target.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(i));
            if (j < 0 || j >= target.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(j));

            var tmp_i = target.GetColumn(i);
            target.SetColumn(i, target.GetColumn(j));
            target.SetColumn(j, tmp_i);
        }

        public static double[,] Transpose(this double[,] target)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            var res = Create(target.GetLength(1), target.GetLength(0));

            Parallel.For(0, target.GetLength(0), i =>
            {
                res.SetColumn(i, target.GetRow(i));
            });

            return res;
        }

        #endregion

        #region Arithmetics

        public static double[,] PointwiseAdd(this double[,] target, double value)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Add(value));
            });

            return target;
        }
        public static double[,] PointwiseAdd(this double[,] target, double[] other, bool asRow = true)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (asRow)
            {
                if (target.GetLength(1) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), i =>
                {
                    target.SetRow(i, target.GetRow(i).Add(other));
                });
            }
            else
            {
                if (target.GetLength(0) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), j =>
                {
                    target.SetColumn(j, target.GetRow(j).Add(other));
                });
            }

            return target;
        }
        public static double[,] PointwiseAdd(this double[,] target, double[,] other)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (target.GetDimension() != other.GetDimension())
                throw new ArgumentException("Matricies dimensions are not equal", nameof(other));

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Add(other.GetRow(i)));
            });

            return target;
        }

        public static double[,] PointwiseSubstract(this double[,] target, double value)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Substract(value));
            });

            return target;
        }
        public static double[,] PointwiseSubstract(this double[,] target, double[] other, bool asRow = true)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (asRow)
            {
                if (target.GetLength(1) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), i =>
                {
                    target.SetRow(i, target.GetRow(i).Substract(other));
                });
            }
            else
            {
                if (target.GetLength(0) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), j =>
                {
                    target.SetColumn(j, target.GetRow(j).Substract(other));
                });
            }

            return target;
        }
        public static double[,] PointwiseSubstract(this double[,] target, double[,] other)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (target.GetDimension() != other.GetDimension())
                throw new ArgumentException("Matricies dimensions are not equal", nameof(other));

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Substract(other.GetRow(i)));
            });

            return target;
        }

        public static double[,] PointwiseDivide(this double[,] target, double value)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            if(value == 0) return target;

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Divide(value));
            });

            return target;
        }
        public static double[,] PointwiseDivide(this double[,] target, double[] other, bool asRow = true)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (asRow)
            {
                if (target.GetLength(1) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), i =>
                {
                    target.SetRow(i, target.GetRow(i).Divide(other));
                });
            }
            else
            {
                if (target.GetLength(0) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), j =>
                {
                    target.SetColumn(j, target.GetRow(j).Divide(other));
                });
            }

            return target;
        }
        public static double[,] PointwiseDivide(this double[,] target, double[,] other)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (target.GetDimension() != other.GetDimension())
                throw new ArgumentException("Matricies dimensions are not equal", nameof(other));

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Divide(other.GetRow(i)));
            });

            return target;
        }

        public static double[,] PointwiseMultiply(this double[,] target, double value)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Multiply(value));
            });

            return target;
        }
        public static double[,] PointwiseMultiply(this double[,] target, double[] other, bool asRow = true)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (asRow)
            {
                if (target.GetLength(1) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), i =>
                {
                    target.SetRow(i, target.GetRow(i).Multiply(other));
                });
            }
            else
            {
                if (target.GetLength(0) != other.Length)
                    throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

                Parallel.For(0, target.GetLength(0), j =>
                {
                    target.SetColumn(j, target.GetRow(j).Multiply(other));
                });
            }

            return target;
        }
        public static double[,] PointwiseMultiply(this double[,] target, double[,] other)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (target.GetDimension() != other.GetDimension())
                throw new ArgumentException("Matricies dimensions are not equal", nameof(other));

            Parallel.For(0, target.GetLength(0), i =>
            {
                target.SetRow(i, target.GetRow(i).Multiply(other.GetRow(i)));
            });

            return target;
        }

        public static double[,] Multiply(this double[,] target, double value)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            var res = Create(target.GetLength(0), target.GetLength(1));

            Parallel.For(0, target.GetLength(0), i =>
            {
                res.SetRow(i, target.GetRow(i).Multiply(value));
            });

            return res;
        }
        public static double[] Multiply(this double[,] target, double[] other)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (target.GetLength(1) != other.Length)
                throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

            var res = new double[target.GetLength(0)];

            Parallel.For(0, target.GetLength(0), i =>
            {
                res[i] = target.GetRow(i).DotProduct(other);
            });

            return res;
        }
        public static double[,] Multiply(this double[,] target, double[,] other)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = other ?? throw new ArgumentNullException(nameof(other));

            if (target.GetLength(1) != other.GetLength(0))
                throw new ArgumentException("Target rows count is not equals to other columns count", nameof(other));

            var res = new double[target.GetLength(0), other.GetLength(1)];

            Parallel.For(0, target.GetLength(0), i =>
            {
                for (int j = 0; j < other.GetLength(1); j++)
                res[i, j] = target.GetRow(i).DotProduct(other.GetColumn(j));
            });

            return res;
        }

        #endregion

        #region Mapping

        public static double[,] Map(this double[,] mat, Func<double, double> func)
        {
            _ = mat ?? throw new ArgumentNullException(nameof(mat));

            double[,] res = MatrixDouble.Create(mat.RowsCount(), mat.ColumnsCount());

            for (int i = 0; i < res.RowsCount(); i++)
                for (int j = 0; j < res.ColumnsCount(); j++)
                    res[i, j] = func(mat[i, j]);

            return res;
        }

        #endregion

        #region Special matricies

        public static double[,] Ones(int rowsCount, int colsCount)
        {
            if (rowsCount <= 0) throw new ArgumentOutOfRangeException(nameof(rowsCount));
            if (colsCount <= 0) throw new ArgumentOutOfRangeException(nameof(colsCount));

            var mat = new double[rowsCount, colsCount];
            Parallel.For(0, rowsCount, i =>
            {
                for (int j = 0; j < colsCount; j++)
                    mat[i, j] = 1.0;
            });                
            return mat;
        }
        public static double[,] Eye(int rowsCount, int colsCount)
        {
            if (rowsCount <= 0) throw new ArgumentOutOfRangeException(nameof(rowsCount));
            if (colsCount <= 0) throw new ArgumentOutOfRangeException(nameof(colsCount));

            int n = Math.Min(rowsCount, colsCount);
            var mat = new double[rowsCount, colsCount];
            Parallel.For(0, n, i =>
            {
                mat[i, i] = 1.0;
            });
                
            return mat;
        }

        #endregion

        #region String Format

        public static string ToStringFormat(this double[,] mat)
        {
            if (mat == null || mat.RowsCount() == 0 || mat.ColumnsCount() == 0)
                return "[]";

            var rows = mat.RowsCount();
            var cols = mat.ColumnsCount();

            var sb = new StringBuilder();

            for (int i = 0; i < rows; i++)
            {
                sb.Append("|  ");
                for (int j = 0; j < cols; j++)
                {
                    sb.Append($"{mat[i, j]}  ");
                }
                sb.AppendLine("|");
            }

            return sb.ToString();
        }


        #endregion
    }
}
