using System;
using System.Collections.Generic;
using System.Linq;
using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Algebra
{
    public class SparseVectorComplex
    {
        public const double Prec = 1e-16;

        public int Length;

        public int[] Indexes;
        public Complex[] Values;

        public Complex this[int i] 
        { 
            get
            {
                var pos = Array.IndexOf(Indexes, i);
                if (pos == -1)
                {
                    return Complex.Zero;
                }
                else
                {
                    return Values[pos];
                }
            }
        }


        #region Constructor

        private SparseVectorComplex()
        {
        }

        public SparseVectorComplex(int count, int length)
        {
            Indexes = new int[count];
            Values = new Complex[count];
            Length = length;
        }

        public SparseVectorComplex(Dictionary<int, Complex> dict, int length)
        {
            if (length < 0)
                throw new ArgumentException("Input length < 0", nameof(length));

            Indexes = dict.Keys.ToArray();
            Values = dict.Values.ToArray();
            Length = length;
        }

        public SparseVectorComplex(double[] vector)
        {
            if (vector.Length == 0)
                throw new ArgumentException("Input vector has 0-length", nameof(vector));

            var tmpVals = new List<Complex>();
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

        public SparseVectorComplex(Complex[] vector)
        {
            if (vector.Length == 0)
                throw new ArgumentException("Input vector has 0-length", nameof(vector));

            var tmpVals = new List<Complex>();
            var tmpInds = new List<int>();

            for (int i = 0; i < vector.Length; i++)
            {
                if (Math.Abs(vector[i].Magnitude) >= Prec)
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

        public SparseVectorComplex Concat(SparseVectorComplex vector)
        {
            var res = new SparseVectorComplex(Values.Length + vector.Values.Length, Length + vector.Length);
            
            if(Indexes.Length != 0 & vector.Indexes.Length == 0)
            {
                Array.Copy(Indexes, 0, res.Indexes, 0, Indexes.Length);
                Array.Copy(Values, 0, res.Values, 0, Values.Length);
            }
            else if(Indexes.Length == 0 & vector.Indexes.Length != 0)
            {
                var tmpInds = vector.Indexes.Select(i => i + Length).ToArray();
                Array.Copy(tmpInds, 0, res.Indexes, 0, vector.Indexes.Length);
                Array.Copy(vector.Values, 0, res.Values, 0, vector.Values.Length);
            }
            else if(Indexes.Length != 0 & vector.Indexes.Length != 0)
            {
                Array.Copy(Indexes, 0, res.Indexes, 0, Indexes.Length);
                var tmpInds = vector.Indexes.Select(i => i + Length).ToArray();
                Array.Copy(tmpInds, 0, res.Indexes, Indexes.Length, vector.Indexes.Length);

                Array.Copy(Values, 0, res.Values, 0, Values.Length);
                Array.Copy(vector.Values, 0, res.Values, Values.Length, vector.Values.Length);
            }

            return res;
        }
    }
}