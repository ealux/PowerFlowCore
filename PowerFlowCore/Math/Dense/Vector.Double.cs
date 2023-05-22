using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerFlowCore.Algebra
{
    public static class VectorDouble
    {
        #region Create

        /// <summary>
        /// Create new vector of <paramref name="count"/> dimension with default (zero) values
        /// </summary>
        /// <param name="count">Values count</param>
        /// <returns>New vector of <paramref name="count"/> dimension</returns>
        public static double[] Create(int count)
        {
            if (count <= 0) throw new ArgumentException("Count value is zero or less", nameof(count));
            return new double[count];
        }

        /// <summary>
        /// Create new vector from <see cref="IEnumerable{T}"/> source
        /// </summary>
        /// <param name="source"><see cref="IEnumerable{T}"/> values source</param>
        /// <returns>New vector of <paramref name="source"/> dimension and values</returns>
        public static double[] Create(IEnumerable<double> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.ToArray();
        }

        /// <summary>
        /// Create new vector from <see cref="double[]"/> source with applied <paramref name="func"/>
        /// </summary>
        /// <param name="source"><see cref="double[]"/> values source</param>
        /// <param name="func">Func to be applied to <paramref name="source"/> values</param>
        /// <returns>New vector of <paramref name="source"/> dimension</returns>
        public static double[] Create(double[] source, Func<double, double> func)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Map(func);
        }

        /// <summary>
        /// Create new vector of <paramref name="count"/> dimension with applied <paramref name="func"/>
        /// </summary>
        /// <param name="count">Values count</param>
        /// <param name="func">Func to be applied to values</param>
        /// <returns>New vector of <paramref name="count"/> dimension</returns>
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

        /// <summary>
        /// Elementwise copy of source <paramref name="vec"/>
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>New vector same as source</returns>
        public static double[] Copy(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Create(vec);
        }

        /// <summary>
        /// Elementwise copy of source <paramref name="vec"/> to <paramref name="other"/> vector
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="other">Target vector</param>
        public static void CopyTo(this double[] vec, double[] other)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (vec.Length != other.Length) throw new ArgumentException("Length is not same", nameof(other));

            vec.CopyTo(other, 0);
        }

        #endregion

        #region Get components

        /// <summary>
        /// Create copy of source <paramref name="vec"/> with specific <paramref name="count"/>
        /// </summary>
        /// <remarks>Copy from  first element</remarks>
        /// <param name="vec">Source vector</param>
        /// <param name="count">Number of values</param>
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

        /// <summary>
        /// Create copy of source <paramref name="vec"/> with values from <paramref name="startIndex"/> to <paramref name="endIndexorCount"/>
        /// </summary>
        /// <remarks>If <paramref name="endIndexorCount"/> is less or equals <paramref name="startIndex"/>, it will be used as amount of values to copy</remarks>
        /// <param name="vec">Source vector</param>
        /// <param name="startIndex">Index to start from</param>
        /// <param name="endIndexorCount">Index to end or values amount</param>
        public static double[] SubVector(this double[] vec, int startIndex, int endIndexorCount)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (startIndex < 0 || startIndex >= vec.Length) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (startIndex >= endIndexorCount)
            {
                if (startIndex + endIndexorCount <= vec.Length)
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

        /// <summary>
        /// Swap values in vector by specific index
        /// </summary>
        /// <param name="vec">Vector to swap values</param>
        /// <param name="i">First position to swap</param>
        /// <param name="j">Second position to swap</param>
        /// <returns>Source vector copy with swapped values</returns>
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

        /// <summary>
        /// Calculate P norm of vector
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="p">Norm factor</param>
        /// <returns>P-norm value</returns>
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

        /// <summary>
        /// Calculate vector L1-norm (sum of absolute values)
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>L1-norm value</returns>
        public static double L1Norm(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            var res = 0d;
            for (int i = 0; i < vec.Length; i++)
                res += Math.Abs(vec[i]);
            return res;
        }

        /// <summary>
        /// Calculate vector L2-norm (sum of values squares)
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>L2-norm value</returns>
        public static double L2Norm(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            var res = 0d;
            for (int i = 0; i < vec.Length; i++)
                res += vec[i] * vec[i];
            return Math.Sqrt(res);
        }

        /// <summary>
        /// Calculate vector infinity norm (maximum of absolute values)
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>Infinity norm value</returns>
        public static double InfinityNorm(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return 0d;

            if (vec.Length == 1)
                return Math.Abs(vec[0]);

            return vec.Select(x => Math.Abs(x)).Max();
        }

        #endregion

        #region Arithmetics

        /// <summary>
        /// Return vector where values are taken with the opposite sign
        /// </summary>
        public static double[] Negative(this double[] vec) => vec.Map(i => -i);

        /// <summary>
        /// Return vector where values are inversed from source one
        /// </summary>
        public static double[] Inverse(this double[] vec) => vec.Map(i => i == 0d ? 0d : 1 / i);

        /// <summary>
        /// Add scalar to vector values
        /// </summary>
        public static double[] Add(this double[] vec, double value) => vec.Map(i => i + value);
        /// <summary>
        /// Two vectors pointwise sum
        /// </summary>
        public static double[] Add(this double[] vec, double[] other) => vec.Map(other, (i, j) => i + j);

        /// <summary>
        /// Substract scalar from vector values
        /// </summary>
        public static double[] Substract(this double[] vec, double value) => vec.Map(i => i - value);
        /// <summary>
        /// Two vectors pointwise substract
        /// </summary>
        public static double[] Substract(this double[] vec, double[] other) => vec.Map(other, (i, j) => i - j);

        /// <summary>
        /// Pointwise multiplication of vector by scalar
        /// </summary>
        public static double[] Multiply(this double[] vec, double value) => vec.Map(i => i * value);
        /// <summary>
        /// Two vectors pointwise multiplication
        /// </summary>
        public static double[] Multiply(this double[] vec, double[] other) => vec.Map(other, (i, j) => i * j);

        /// <summary>
        /// Pointwise division of vector by scalar 
        /// </summary>
        public static double[] Divide(this double[] vec, double value) => vec.Map(i => i / value);
        /// <summary>
        /// Two vectors pointwise division
        /// </summary>
        public static double[] Divide(this double[] vec, double[] other) => vec.Map(other, (i, j) => j == 0d ? 0d : i / j);

        /// <summary>
        /// Two vectors dot product (sum of pointwise multiplications)
        /// </summary>
        public static double DotProduct(this double[] vec, double[] other)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            double res = 0d;

            Parallel.For(0, vec.Length, i => res += vec[i] * other[i]);
			
            return res;
        }

        #endregion

        #region MinMax

        /// <summary>
        /// Find maximum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Maximum(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Max();
        }

        /// <summary>
        /// Find minimum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Minimum(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Min();
        }

        /// <summary>
        /// Find index of maximum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaximumIndex(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Array.IndexOf(vec, vec.Max());
        }

        /// <summary>
        /// Find index of minimum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinimumIndex(this double[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Array.IndexOf(vec, vec.Min());
        }

        #endregion

        #region Mapping

        /// <summary>
        /// Apply special <paramref name="func"/> to vector values position (<see cref="int"/>) enumerator
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="func">Func to be applied</param>
        /// <returns>Vector with results of applied <paramref name="func"/></returns>
        public static double[] Map(this int[] vec, Func<int, double> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new double[0];

            double[] res = new double[vec.Length];

            Parallel.For(0, vec.Length, i => res[i] = func(vec[i]));

            return res;
        }

        /// <summary>
        /// Apply special <paramref name="func"/> to vector values
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="func">Func to be applied</param>
        /// <returns>Vector with applied <paramref name="func"/></returns>
        public static double[] Map(this double[] vec, Func<double, double> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new double[0];

            double[] res = new double[vec.Length];

            Parallel.For(0, vec.Length, i => res[i] = func(vec[i]));

            return res;
        }

        /// <summary>
        /// Apply special <paramref name="func"/> pointwise to source and other vectors
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="other">Other vector</param>
        /// <param name="func">Func to be applied</param>
        /// <returns>Vector with applied <paramref name="func"/></returns>
        public static double[] Map(this double[] vec, double[] other, Func<double, double, double> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            double[] res = new double[vec.Length];

            Parallel.For(0, vec.Length, i => res[i] = func(vec[i], other[i]));

            return res;
        }

        #endregion

        #region Special vectors

        /// <summary>
        /// Create vector of special dimension where all values are equal 1.0
        /// </summary>
        public static double[] One(int count)
        {
            if (count <= 0) throw new ArgumentException("Count is equals or lwss then zero", nameof(count));
            return new double[count].Add(1.0);
        }

        #endregion

        #region String format

        /// <summary>
        /// Vector string representation
        /// </summary>
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
