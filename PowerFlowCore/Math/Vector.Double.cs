using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerFlowCore.Algebra
{
    internal static class VectorDouble
    {
        #region Create

        public static double[] Create(int count)
        {
            if (count <= 0) throw new ArgumentException("Count value is zero or less", nameof(count));
            return new double[count];
        }

        public static double[] Create(IEnumerable<double> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.ToArray();
        }

        public static double[] Create(double[] source, Func<double, double> func)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Map(func);
        }

        public static double[] Create(int count, Func<int, double> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (count <= 0) throw new ArgumentException(nameof(count));

            double[] res = new double[count];

            for (int i = 0; i < count; i++)
                res[i] = func(i);
            return res;
        }

        #endregion

        #region Copy

        public static double[] Copy(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Create(vec);
        }

        public static void CopyTo(this double[] vec, double[] other)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (vec.Length != other.Length) throw new ArgumentException("Length is not same", nameof(other));

            other = Create(vec);
        }

        #endregion

        #region Get components

        public static double[] SubVector(this double[] vec, int count)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (count < 0) throw new ArgumentException("Count value is less then zero", nameof(count));
            if (count > vec.Length) throw new ArgumentException("Count value is more then vector length", nameof(count));

            var res = new double[count];

            for (int i = 0; i < count; i++)
                res[i] = vec[i];
            return res;
        }

        public static double[] SubVector(this double[] vec, int startIndex, int endIndexorCount)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (startIndex < 0 || startIndex >= vec.Length) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (startIndex >= endIndexorCount)
            {
                if(startIndex + endIndexorCount <= vec.Length)
                {
                    var l = endIndexorCount;
                    var output = new double[l];
                    for (int i = 0; i < l; i++)
                        output[i] = vec[i + startIndex];
                    return output;
                }
            }

            var length = endIndexorCount - startIndex;
            var res = new double[length + 1];
            for (int i = 0; i <= length; i++)
                res[i] = vec[i + startIndex];
            return res;
        }

        #endregion

        #region Manipulate        

        public static double[] Swap(this double[] vec, int i, int j)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (i < 0 || i >= vec.Length) throw new ArgumentOutOfRangeException(nameof(i));
            if (j < 0 || j >= vec.Length) throw new ArgumentOutOfRangeException(nameof(j));
            if (i == j)
                return vec;

            (vec[i], vec[j]) = (vec[j], vec[i]);
            return vec;
        }

        #endregion

        #region Norm

        public static double Norm(this double[] vec, double p)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (p < 0d) throw new ArgumentOutOfRangeException(nameof(p));

            if (p == 1d) return vec.L1Norm();
            if (p == 2d) return vec.L2Norm();
            if (double.IsPositiveInfinity(p)) return vec.InfinityNorm();

            var res = 0d;
            for (var index = 0; index < vec.Length; index++)
            {
                res += Math.Pow(Math.Abs(vec[index]), p);
            }
            return Math.Pow(res, 1.0 / p);
        }

        public static double L1Norm(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            var res = 0d;
            for (int i = 0; i < vec.Length; i++)
                res += Math.Abs(vec[i]);
            return res;
        }


        public static double L2Norm(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            var res = 0d;
            for (int i = 0; i < vec.Length; i++)
                res += vec[i] * vec[i];
            return Math.Sqrt(res);
        }


        public static double InfinityNorm(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return 0d;

            if (vec.Length == 1)
                return vec[0];

            return vec.Select(x => Math.Abs(x)).Max();
        }

        #endregion

        #region Arithmetics

        public static double[] Negative(this double[] vec) => vec.Map(i => -i);

        public static double[] Inverse(this double[] vec) => vec.Map(i => i == 0d ? 0d : 1 / i);

        public static double[] Add(this double[] vec, double value) => vec.Map(i => i + value);
        public static double[] Add(this double[] vec, double[] other) => vec.Map(other, (i, j) => i + j);

        public static double[] Substract(this double[] vec, double value) => vec.Map(i => i - value);
        public static double[] Substract(this double[] vec, double[] other) => vec.Map(other, (i, j) => i - j);

        public static double[] Multiply(this double[] vec, double value) => vec.Map(i => i * value);
        public static double[] Multiply(this double[] vec, double[] other) => vec.Map(other, (i, j) => i * j);

        public static double[] Divide(this double[] vec, double value) => vec.Map(i => i / value);
        public static double[] Divide(this double[] vec, double[] other) => vec.Map(other, (i, j) => j == 0d ? 0d : i / j);

        public static double DotProduct(this double[] vec, double[] other)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            double res = 0d;

            for (int i = 0; i < vec.Length; i++)
                res += vec[i] * other[i];
            return res;
        }

        #endregion

        #region MinMax

        public static double Maximum(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Max();
        }
        public static double Minimum(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Min();
        }
        public static int MaximumIndex(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Array.IndexOf(vec, vec.Max());
        }
        public static int MinimumIndex(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Array.IndexOf(vec, vec.Min());
        }

        #endregion

        #region Mapping

        public static double[] Map(this double[] vec, Func<double, double> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new double[0];

            double[] res = new double[vec.Length];

            Parallel.For(0, vec.Length, i =>
            {
                res[i] = func(vec[i]);
            });

            return res;
        }

        public static double[] Map(this double[] vec, double[] other, Func<double, double, double> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            double[] res = new double[vec.Length];

            Parallel.For(0, vec.Length, i =>
            {
                res[i] = func(vec[i], other[i]);
            });
                
            return res;
        }

        #endregion

        #region Special vectors

        public static double[] One(int count)
        {
            if (count <= 0) throw new ArgumentException("Count is equals or lwss then zero", nameof(count));
            return new double[count].Add(1.0);
        }

        #endregion

        #region String format

        public static string ToStringFormat(this double[] vec)
        {
            if (vec == null)
                return "[]";

            var sb = new StringBuilder();

            sb.Append("[  ");
            sb.Append(string.Join(" ", vec));
            sb.AppendLine(" ]");

            return sb.ToString();
        }

        #endregion
    }
}
