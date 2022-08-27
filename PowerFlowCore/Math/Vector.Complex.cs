using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Algebra
{
    internal static class VectorComplex
    {
        #region Create

        public static Complex[] Create(int count)
        {
            if (count <= 0) throw new ArgumentException("Count value is zero or less", nameof(count));
            return new Complex[count];
        }

        public static Complex[] Create(IEnumerable<Complex> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.ToArray();
        }

        public static Complex[] Create(Complex[] source, Func<Complex, Complex> func)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Map(func);
        }

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

        public static Complex[] Copy(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Create(vec);
        }

        public static void CopyTo(this Complex[] vec, Complex[] other)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            if (vec.Length != other.Length) throw new ArgumentException("Length is not same", nameof(other));

            other = Create(vec);
        }

        #endregion

        #region Get components

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

        #endregion

        #region Manipulate        

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

        public static Complex Conjugate(this Complex value) => new Complex(value.Real, -value.Imaginary);

        #endregion

        #region Norm

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

        public static double L1Norm(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            var res = 0d;
            for (int i = 0; i < vec.Length; i++)
                res += vec[i].Magnitude;
            return res;
        }

        public static double L2Norm(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            return vec.Aggregate(Complex.Zero, (a, b) =>
            {
                if (a.Magnitude > b.Magnitude)
                {
                    var r = b.Magnitude / a.Magnitude;
                    return a.Magnitude * Math.Sqrt(1 + (r * r));
                }

                if (b != 0.0)
                {
                    var r = a.Magnitude / b.Magnitude;
                    return b.Magnitude * Math.Sqrt(1 + (r * r));
                }

                return 0d;

            }).Magnitude;
        }

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
        public static Complex[] Negative(this Complex[] vec) => vec.Map(i => -i);

        public static Complex[] Inverse(this Complex[] vec) => vec.Map(i => i.Magnitude == 0 ? Complex.Zero : -i);

        public static Complex[] Add(this Complex[] vec, double value) => vec.Map(i => i + value);              
        public static Complex[] Add(this Complex[] vec, Complex value) => vec.Map(i => i + value);              
        public static Complex[] Add(this Complex[] vec, double[] other)   => vec.Map(other, (i, j) => i + j);    
        public static Complex[] Add(this Complex[] vec, Complex[] other)   => vec.Map(other, (i, j) => i + j);    
        
        public static Complex[] Substract(this Complex[] vec, double value) => vec.Map(i => i - value);                     
        public static Complex[] Substract(this Complex[] vec, Complex value) => vec.Map(i => i - value);                     
        public static Complex[] Substract(this Complex[] vec, double[] other) => vec.Map(other, (i, j) => i - j);      
        public static Complex[] Substract(this Complex[] vec, Complex[] other) => vec.Map(other, (i, j) => i - j);      
        
        public static Complex[] Multiply(this Complex[] vec, double value) => vec.Map(i => i * value);                            
        public static Complex[] Multiply(this Complex[] vec, Complex value) => vec.Map(i => i * value);                            
        public static Complex[] Multiply(this Complex[] vec, double[] other) => vec.Map(other, (i, j) => i * j);          
        public static Complex[] Multiply(this Complex[] vec, Complex[] other) => vec.Map(other, (i, j) => i * j);          
        
        public static Complex[] Divide(this Complex[] vec, double value) => vec.Map(i => value == 0 ? Complex.Zero : i / value);                   
        public static Complex[] Divide(this Complex[] vec, Complex value) => vec.Map(i => value.Magnitude == 0 ? Complex.Zero : i / value);                   
        public static Complex[] Divide(this Complex[] vec, double[] other) => vec.Map(other, (i, j) => j == 0 ? Complex.Zero : i / j);
        public static Complex[] Divide(this Complex[] vec, Complex[] other) => vec.Map(other, (i, j) => j.Magnitude == 0 ? Complex.Zero : i / j);

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

        public static Complex Maximum(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Max();
        }
        public static Complex Minimum(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return vec.Min();
        }
        public static int MaximumIndex(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Array.IndexOf(vec, vec.Max());
        }
        public static int MinimumIndex(this Complex[] vec)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            return Array.IndexOf(vec, vec.Min());
        }

        #endregion

        #region Mapping

        public static double[] Map(this Complex[] vec, Func<Complex, double> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new double[0];

            var res = new double[vec.Length];

            for (int i = 0; i < vec.Length; i++)
                res[i] = func(vec[i]);
            return res;
        }
        public static Complex[] Map(this Complex[] vec, Func<Complex, Complex> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length == 0)
                return new Complex[0];

            Complex[] res = new Complex[vec.Length];

            for (int i = 0; i < vec.Length; i++)
                res[i] = func(vec[i]);
            return res;
        }
        public static Complex[] Map(this Complex[] vec, Complex[] other, Func<Complex, Complex, Complex> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            Complex[] res = new Complex[vec.Length];

            for (int i = 0; i < vec.Length; i++)
                res[i] = func(vec[i], other[i]);
            return res;
        }
        public static Complex[] Map(this Complex[] vec, double[] other, Func<Complex, double, Complex> func)
        {
            _ = vec ?? throw new ArgumentNullException(nameof(vec));
            _ = other ?? throw new ArgumentNullException(nameof(vec));

            if (vec.Length != other.Length)
                throw new ArgumentException("Vector dimensions should be same!", nameof(other));

            Complex[] res = new Complex[vec.Length];

            for (int i = 0; i < vec.Length; i++)
                res[i] = func(vec[i], other[i]);
            return res;
        }

        #endregion

        #region String format

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
