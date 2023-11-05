using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PowerFlowCore.Algebra
{
    /// <summary>
    /// Represents sparse vector structure with nonzero Indexes and Values arrays
    /// </summary>
    public struct SparseVector
    {
        /// <summary>
        /// Non-zero precision
        /// </summary>
        public const double Prec = 1e-16;

        /// <summary>
        /// Actual vector length
        /// </summary>
        public int Length;

        /// <summary>
        /// Array of nonzero values position indexes
        /// </summary>
        public int[] Indexes;

        /// <summary>
        /// Array of nonzero values
        /// </summary>
        public double[] Values;


        public double this[int i] 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var pos = Array.IndexOf(Indexes, i);
                if (pos == -1)
                {
                    return 0;
                }
                else
                {
                    return Values[pos];
                }
            }
        }

        #region Constructor

        public SparseVector(int count, int length)
        {
            Indexes = new int[count];
            Values = new double[count];
            Length = length;
        }

        public SparseVector(Dictionary<int, double> dict, int length)
        {
            if (length < 0)
                throw new ArgumentException("Input length < 0", nameof(length));

            var count = dict.Keys.Count;

            Indexes = new int[count];
            Values = new double[count];

            dict.Keys.CopyTo(Indexes, 0);
            dict.Values.CopyTo(Values, 0);

            Length = length;
        }

        public SparseVector(Dictionary<int, double> dict1, int length1, Dictionary<int, double> dict2, int length2)
        {
            if (length1 < 0 || length2 < 0)
                throw new ArgumentException("Input length < 0");

            var count1 = dict1.Keys.Count;
            var count2 = dict2.Keys.Count;

            Indexes = new int[count1 + count2];
            Values = new double[count1 + count2];

            Length = length1 + length2;

            if(count1 != 0 & count2 == 0)
            {
                dict1.Keys.CopyTo(Indexes, 0);
                dict1.Values.CopyTo(Values, 0);
            }
            else if (count1 == 0 & count2 != 0)
            {
                dict2.Keys.CopyTo(Indexes, 0);
                for (int i = 0; i < count2; i++)
                    Indexes[i] += length1;
                dict2.Values.CopyTo(Values, 0);
            }
            else if (count1 != 0 & count2 != 0)
            {
                dict1.Keys.CopyTo(Indexes, 0);
                dict2.Keys.CopyTo(Indexes, count1);
                for (int i = count1; i < count1 + count2; i++)
                    Indexes[i] += length1;
                dict1.Values.CopyTo(Values, 0);
                dict2.Values.CopyTo(Values, count1);
            }
        }

#if (NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)
        internal SparseVector(Span<int> vec1_inds, Span<double> vec1_vals, int length1, Span<int> vec2_inds, Span<double> vec2_vals, int length2)
#else
        public SparseVector(int[] vec1_inds, double[] vec1_vals, int length1, int[] vec2_inds, double[] vec2_vals, int length2)
#endif
        {
            if (length1 < 0 || length2 < 0)
                throw new ArgumentException("Input length < 0");

            var count1 = vec1_inds.Length;
            var count2 = vec2_inds.Length;

            Length = length1 + length2;

            Indexes = new int[count1 + count2];
            Values = new double[count1 + count2];

#if (NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)

            if (count1 != 0 & count2 == 0)
            {
                vec1_inds.CopyTo(Indexes);
                vec1_vals.CopyTo(Values);
            }
            else if (count1 == 0 & count2 != 0)
            {
                vec2_inds.CopyTo(Indexes);
                for (int i = 0; i < count2; i++)
                    Indexes[i] += length1;
                vec2_vals.CopyTo(Values);
            }
            else if (count1 != 0 & count2 != 0)
            {
                vec1_inds.CopyTo(Indexes.AsSpan().Slice(0, count1));
                vec2_inds.CopyTo(Indexes.AsSpan().Slice(count1, count2));
                for (int i = count1; i < count1 + count2; i++)
                    Indexes[i] += length1;
                vec1_vals.CopyTo(Values.AsSpan().Slice(0, count1));
                vec2_vals.CopyTo(Values.AsSpan().Slice(count1, count2));
            }
#else
            if (count1 != 0 & count2 == 0)
            {
                Array.Copy(vec1_inds, 0, Indexes, 0, count1);
                Array.Copy(vec1_vals, 0, Values, 0, count1);
            }
            else if (count1 == 0 & count2 != 0)
            {
                Array.Copy(vec2_inds, 0, Indexes, 0, count2);
                for (int i = 0; i < count2; i++)
                    Indexes[i] += length1;
                Array.Copy(vec2_vals, 0, Values, 0, count2);
            }
            else if (count1 != 0 & count2 != 0)
            {

                Array.Copy(vec1_inds, 0, Indexes, 0, count1);
                Array.Copy(vec2_inds, 0, Indexes, count1, count2);
                for (int i = count1; i < count1 + count2; i++)
                    Indexes[i] += length1;
                Array.Copy(vec1_vals, 0, Values, 0, count1);
                Array.Copy(vec2_vals, 0, Values, count1, count2);
            }
#endif
        }

        /// <summary>
        /// Create sparse vector from dense
        /// </summary>
        /// <param name="vector">Input dense vector</param>
        public SparseVector(double[] vector)
        {
            if (vector.Length == 0)
                throw new ArgumentException("Input vector has 0-length", nameof(vector));

            var tmpVals = new List<double>();
            var tmpInds = new List<int>();

            for (int i = 0; i < vector.Length; i++)
            {
                if (Math.Abs(vector[i]) >= Prec)
                {
                    tmpVals.Add(vector[i]);
                    tmpInds.Add(i);
                }
            }

            Indexes = tmpInds.ToArray();
            Values = tmpVals.ToArray();
            Length = vector.Length;
        }

        #endregion Constructor

        /// <summary>
        /// Create dense-structure vector by sparse scheme
        /// </summary>
        /// <returns>Dense variant of initial vector</returns>
        public double[] ToDense()
        {
            var res = new double[Length];

            for (int i = 0; i < Indexes.Length; i++)
                res[Indexes[i]] = Values[i];
            return res;
        }

        /// <summary>
        /// Merge with another sparse vector accounting source dense vectors length
        /// </summary>
        /// <param name="vector">Other sparse vector</param>
        /// <returns>New sparse vector</returns>
        public SparseVector Concat(SparseVector vector)
        {
            var res = new SparseVector(Values.Length + vector.Values.Length, Length + vector.Length);
            
            if(Indexes.Length != 0 & vector.Indexes.Length == 0)
            {
                Array.Copy(Indexes, 0, res.Indexes, 0, Indexes.Length);
                Array.Copy(Values, 0, res.Values, 0, Values.Length);
            }
            else if(Indexes.Length == 0 & vector.Indexes.Length != 0)
            {
                var tmpInds = new int[vector.Indexes.Length];
                for (int i = 0; i < vector.Indexes.Length; i++)
                    tmpInds[i] = vector.Indexes[i] + Length;
                Array.Copy(tmpInds, 0, res.Indexes, 0, vector.Indexes.Length);
                Array.Copy(vector.Values, 0, res.Values, 0, vector.Values.Length);
            }
            else if(Indexes.Length != 0 & vector.Indexes.Length != 0)
            {
                Array.Copy(Indexes, 0, res.Indexes, 0, Indexes.Length);

                var tmpInds = new int[vector.Indexes.Length];
                for (int i = 0; i < vector.Indexes.Length; i++)
                    tmpInds[i] = vector.Indexes[i] + Length;

                Array.Copy(tmpInds, 0, res.Indexes, Indexes.Length, vector.Indexes.Length);

                Array.Copy(Values, 0, res.Values, 0, Values.Length);
                Array.Copy(vector.Values, 0, res.Values, Values.Length, vector.Values.Length);
            }

            return res;
        }
    }
        
}