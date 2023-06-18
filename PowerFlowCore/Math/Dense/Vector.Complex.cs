using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Algebra
{
    public static class VectorComplex
    {
        #region Create

        /// <summary>
        /// Create new vector of <paramref name="count"/> dimension with default (<see cref="Complex.Zero"/>) values
        /// </summary>
        /// <param name="count">Values count</param>
        /// <returns>New vector of <paramref name="count"/> dimension</returns>
        public static Complex[] Create(int count)
        {
            if (count <= 0) throw new ArgumentException("Count value is zero or less", nameof(count));
            return new Complex[count];
        }

        /// <summary>
        /// Create new vector from <see cref="IEnumerable{T}"/> source
        /// </summary>
        /// <param name="source"><see cref="IEnumerable{T}"/> values source</param>
        /// <returns>New vector of <paramref name="source"/> dimension and values</returns>
        public static Complex[] Create(IEnumerable<Complex> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.ToArray();
        }

        /// <summary>
        /// Create new vector from <see cref="Complex[]"/> source with applied <paramref name="func"/>
        /// </summary>
        /// <param name="source"><see cref="Complex[]"/> values source</param>
        /// <param name="func">Func to be applied to <paramref name="source"/> values</param>
        /// <returns>New vector of <paramref name="source"/> dimension</returns>
        public static Complex[] Create(Complex[] source, Func<Complex, Complex> func)
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
        public static Complex[] Create(int count, Func<int, Complex> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (count <= 0) throw new ArgumentException(nameof(count));

            Complex[] res = new Complex[count];

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
        public static Complex[] Copy(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Create(vec);
        }

        /// <summary>
        /// Elementwise copy of source <paramref name="vec"/> to <paramref name="other"/> vector
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="other">Target vector</param>
        public static void CopyTo(this Complex[] vec, Complex[] other)
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
        public static Complex[] SubVector(this Complex[] vec, int count)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (count < 0) throw new ArgumentException("Count value is less then zero", nameof(count));
            if (count > vec.Length) throw new ArgumentException("Count value is more then vector length", nameof(count));

            var res = new Complex[count];

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
        public static Complex[] SubVector(this Complex[] vec, int startIndex, int endIndexorCount)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (startIndex < 0 || startIndex >= vec.Length) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (startIndex >= endIndexorCount)
            {
                if (startIndex + endIndexorCount <= vec.Length)
                {
                    var l = endIndexorCount;
                    var output = new Complex[l];
                    for (int i = 0; i < l; i++)
                        output[i] = vec[i + startIndex];
                    return output;
                }
            }

            var length = endIndexorCount - startIndex;
            var res = new Complex[length + 1];
            for (int i = 0; i <= length; i++)
                res[i] = vec[i + startIndex];
            return res;
        }

        /// <summary>
        /// Create vector with real part of source vector values
        /// </summary>
        public static double[] Real(this Complex[] vec) => vec.Map(i => i.Real);

        /// <summary>
        /// Create vector with imaginary part of source vector values
        /// </summary>
        public static double[] Imaginary(this Complex[] vec) => vec.Map(i => i.Real);

        #endregion

        #region Manipulate        

        /// <summary>
        /// Swap values in vector by specific index
        /// </summary>
        /// <param name="vec">Vector to swap values</param>
        /// <param name="i">First position to swap</param>
        /// <param name="j">Second position to swap</param>
        /// <returns>Source vector copy with swapped values</returns>
        public static Complex[] Swap(this Complex[] vec, int i, int j)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (i < 0 || i >= vec.Length) throw new ArgumentOutOfRangeException(nameof(i));
            if (j < 0 || j >= vec.Length) throw new ArgumentOutOfRangeException(nameof(j));
            if (i == j)
                return vec;

            (vec[i], vec[j]) = (vec[j], vec[i]);
            return vec;
        }

        /// <summary>
        /// Return conjugate value of complex number
        /// </summary>
        public static Complex Conjugate(this Complex value) => new Complex(value.Real, -value.Imaginary);

        /// <summary>
        /// Create new vector with conjugated values
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>New vector with <paramref name="vec"/> dimension and conjugated values</returns>
        public static Complex[] Conjugate(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            return Create(vec).Map(i => i.Conjugate());
        }

        #endregion

        #region Norm

        /// <summary>
        /// Calculate P norm of vector
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="p">Norm factor</param>
        /// <returns>P-norm value</returns>
        public static double Norm(this Complex[] vec, double p)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (p < 0d) throw new ArgumentOutOfRangeException(nameof(p));

            if (p == 1d) return vec.L1Norm();
            if (p == 2d) return vec.L2Norm();
            if (double.IsPositiveInfinity(p)) return vec.InfinityNorm();

            var res = 0d;
            for (var index = 0; index < vec.Length; index++)
            {
                res += Math.Pow(vec[index].Magnitude, p);
            }
            return Math.Pow(res, 1.0 / p);
        }

        /// <summary>
        /// Calculate vector L1-norm (sum of values magnitudes)
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>L1-norm value</returns>
        public static double L1Norm(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            var res = 0d;
            for (int i = 0; i < vec.Length; i++)
                res += vec[i].Magnitude;
            return res;
        }

        /// <summary>
        /// Calculate vector L2-norm
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>L2-norm value</returns>
        public static double L2Norm(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            return vec.Aggregate(Complex.Zero, (a, b) =>
            {
                if (a.Magnitude > b.Magnitude)
                {
                    var r = b.Magnitude / a.Magnitude;
                    return a.Magnitude * Math.Sqrt(1 + r * r);
                }

                if (b != 0.0)
                {
                    var r = a.Magnitude / b.Magnitude;
                    return b.Magnitude * Math.Sqrt(1 + r * r);
                }

                return 0d;

            }).Magnitude;
        }

        /// <summary>
        /// Calculate vector infinity norm (maximum of values magnitude)
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <returns>Infinity norm value</returns>
        public static double InfinityNorm(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return 0d;

            if (vec.Length == 1)
                return vec[0].Magnitude;

            return vec.Select(x => x.Magnitude).Max();
        }

        #endregion

        #region Arithmetics

        /// <summary>
        /// Return vector where values are taken with the opposite sign
        /// </summary>
        public static Complex[] Negative(this Complex[] vec) => vec.Map(i => -i);

        /// <summary>
        /// Return vector where values are inversed from source one
        /// </summary>
        public static Complex[] Inverse(this Complex[] vec) => vec.Map(i => i.Magnitude == 0 ? Complex.Zero : 1 / i);

        /// <summary>
        /// Add scalar to vector values
        /// </summary>
        public static Complex[] Add(this Complex[] vec, double value) => vec.Map(i => i + value);
        /// <summary>
        /// Add scalar to vector values
        /// </summary>
        public static Complex[] Add(this Complex[] vec, Complex value) => vec.Map(i => i + value);
        /// <summary>
        /// Two vectors pointwise sum
        /// </summary>
        public static Complex[] Add(this Complex[] vec, double[] other) => vec.Map(other, (i, j) => i + j);
        /// <summary>
        /// Two vectors pointwise sum
        /// </summary>
        public static Complex[] Add(this Complex[] vec, Complex[] other) => vec.Map(other, (i, j) => i + j);

        /// <summary>
        /// Substract scalar from vector values
        /// </summary>
        public static Complex[] Substract(this Complex[] vec, double value) => vec.Map(i => i - value);
        /// <summary>
        /// Substract scalar from vector values
        /// </summary>
        public static Complex[] Substract(this Complex[] vec, Complex value) => vec.Map(i => i - value);
        /// <summary>
        /// Two vectors pointwise substract
        /// </summary>
        public static Complex[] Substract(this Complex[] vec, double[] other) => vec.Map(other, (i, j) => i - j);
        /// <summary>
        /// Two vectors pointwise substract
        /// </summary>
        public static Complex[] Substract(this Complex[] vec, Complex[] other) => vec.Map(other, (i, j) => i - j);

        /// <summary>
        /// Pointwise multiplication of vector by scalar
        /// </summary>
        public static Complex[] Multiply(this Complex[] vec, double value) => vec.Map(i => i * value);
        /// <summary>
        /// Pointwise multiplication of vector by scalar
        /// </summary>
        public static Complex[] Multiply(this Complex[] vec, Complex value) => vec.Map(i => i * value);
        /// <summary>
        /// Two vectors pointwise multiplication
        /// </summary>
        public static Complex[] Multiply(this Complex[] vec, double[] other) => vec.Map(other, (i, j) => i * j);
        /// <summary>
        /// Two vectors pointwise multiplication
        /// </summary>
        public static Complex[] Multiply(this Complex[] vec, Complex[] other) => vec.Map(other, (i, j) => i * j);

        /// <summary>
        /// Pointwise division of vector by scalar 
        /// </summary>
        public static Complex[] Divide(this Complex[] vec, double value) => vec.Map(i => value == 0 ? Complex.Zero : i / value);
        /// <summary>
        /// Pointwise division of vector by scalar 
        /// </summary>
        public static Complex[] Divide(this Complex[] vec, Complex value) => vec.Map(i => value.Magnitude == 0 ? Complex.Zero : i / value);
        /// <summary>
        /// Two vectors pointwise division
        /// </summary>
        public static Complex[] Divide(this Complex[] vec, double[] other) => vec.Map(other, (i, j) => j == 0 ? Complex.Zero : i / j);
        /// <summary>
        /// Two vectors pointwise division
        /// </summary>
        public static Complex[] Divide(this Complex[] vec, Complex[] other) => vec.Map(other, (i, j) => j.Magnitude == 0 ? Complex.Zero : i / j);

        /// <summary>
        /// Two vectors dot product (sum of pointwise multiplications)
        /// </summary>
        public static Complex DotProduct(this Complex[] vec, double[] other)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            Complex res = Complex.Zero;

            for (int i = 0; i < vec.Length; i++)
                res += vec[i] * other[i];
            return res;
        }
        /// <summary>
        /// Two vectors dot product (sum of pointwise multiplications)
        /// </summary>
        public static Complex DotProduct(this Complex[] vec, Complex[] other)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            Complex res = Complex.Zero;

            for (int i = 0; i < vec.Length; i++)
                res += vec[i] * other[i];
            return res;
        }

        #endregion

        #region MinMax

        /// <summary>
        /// Find maximum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Complex Maximum(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Max();
        }

        /// <summary>
        /// Find minimum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Complex Minimum(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Min();
        }

        /// <summary>
        /// Find index of maximum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaximumIndex(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Array.IndexOf(vec, vec.Max());
        }

        /// <summary>
        /// Find index of minimum value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinimumIndex(this Complex[] vec)
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
        public static Complex[] Map(this int[] vec, Func<int, Complex> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new Complex[0];

            Complex[] res = new Complex[vec.Length];

            Parallel.For(0, vec.Length, i =>
            {
                res[i] = func(vec[i]);
            });

            return res;
        }

        /// <summary>
        /// Apply special <paramref name="func"/> to vector values
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="func">Func to be applied</param>
        /// <returns>Vector with applied <paramref name="func"/></returns>
        public static double[] Map(this Complex[] vec, Func<Complex, double> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new double[0];

            var res = new double[vec.Length];

            Parallel.For(0, vec.Length, i => res[i] = func(vec[i]));

            return res;
        }

        /// <summary>
        /// Apply special <paramref name="func"/> to vector values
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="func">Func to be applied</param>
        /// <returns>Vector with applied <paramref name="func"/></returns>
        public static Complex[] Map(this Complex[] vec, Func<Complex, Complex> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new Complex[0];

            Complex[] res = new Complex[vec.Length];

            Parallel.For(0, vec.Length, i =>
            {
                res[i] = func(vec[i]);
            });

            return res;
        }

        /// <summary>
        /// Apply special <paramref name="func"/> pointwise to source and other vectors
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="other">Other vector</param>
        /// <param name="func">Func to be applied</param>
        /// <returns>Vector with applied <paramref name="func"/></returns>
        public static Complex[] Map(this Complex[] vec, Complex[] other, Func<Complex, Complex, Complex> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            Complex[] res = new Complex[vec.Length];

            Parallel.For(0, vec.Length, i =>
            {
                res[i] = func(vec[i], other[i]);
            });

            return res;
        }

        /// <summary>
        /// Apply special <paramref name="func"/> pointwise to source and other vectors
        /// </summary>
        /// <param name="vec">Source vector</param>
        /// <param name="other">Other vector</param>
        /// <param name="func">Func to be applied</param>
        /// <returns>Vector with applied <paramref name="func"/></returns>
        public static Complex[] Map(this Complex[] vec, double[] other, Func<Complex, double, Complex> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            Complex[] res = new Complex[vec.Length];

            Parallel.For(0, vec.Length, i =>
            {
                res[i] = func(vec[i], other[i]);
            });

            return res;
        }

        #endregion

        #region Special vectors

        /// <summary>
        /// Create vector of special dimension where all values are equal <see cref="Complex.One"/>
        /// </summary>
        public static Complex[] One(int count)
        {
            if (count <= 0) throw new ArgumentException("Count is equals or lwss then zero", nameof(count));
            return new Complex[count].Add(1.0);
        }

        #endregion

        #region String format

        /// <summary>
        /// Vector string representation
        /// </summary>
        public static string ToStringFormat(this Complex[] vec)
        {
            if (vec == null) return "[]";

            var sb = new StringBuilder();

            sb.Append("[ ");
            for (int i = 0; i < vec.Length; i++)
                sb.Append($"({vec[i].Real}, {vec[i].Imaginary}) ");
            sb.AppendLine("]");

            return sb.ToString();
        }

        #endregion
    }
}
