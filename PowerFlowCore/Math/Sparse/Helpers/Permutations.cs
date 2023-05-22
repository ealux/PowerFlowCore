﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerFlowCore.Algebra
{
    internal static class Permutations
    {
        /// <summary>
        /// Permutes a vector, x=P*b.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="b">Input vector.</param>
        /// <param name="x">Output vector, x=P*b.</param>
        /// <param name="n">Length of p, b and x.</param>
        /// <remarks>
        /// p = null denotes identity.
        /// </remarks>
        public static void Apply(int[] p, double[] b, double[] x, int n)
        {
            if (p == null)
            {
                for (int k = 0; k < n; k++)
                {
                    x[k] = b[k];
                }
            }
            else
            {
                for (int k = 0; k < n; k++)
                {
                    x[k] = b[p[k]];
                }
            }
        }

        /// <summary>
        /// Permutes a vector, x = P'b.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="b">Input vector.</param>
        /// <param name="x">Output vector, x = P'b.</param>
        /// <param name="n">Length of p, b, and x.</param>
        /// <remarks>
        /// p = null denotes identity.
        /// </remarks>
        public static void ApplyInverse(int[] p, double[] b, double[] x, int n)
        {
            if (p == null)
            {
                for (int k = 0; k < n; k++)
                {
                    x[k] = b[k];
                }
            }
            else
            {
                for (int k = 0; k < n; k++)
                {
                    x[p[k]] = b[k];
                }
            }
        }

        /// <summary>
        /// Returns a permutation vector of length n.
        /// </summary>
        /// <param name="n">Length of the permutation.</param>
        /// <param name="seed">0: identity, -1: reverse, seed > 0: random permutation</param>
        /// <returns>Permutation vector.</returns>
        public static int[] Create(int n, int seed = 0)
        {
            int i, j, tmp;
            int[] p = new int[n];

            if (seed == 0)
            {
                // Identity
                for (i = 0; i < n; i++)
                {
                    p[i] = i;
                }
            }
            else
            {
                // Inverse
                for (i = 0; i < n; i++)
                {
                    p[i] = n - i - 1;
                }
            }

            if (seed > 0)
            {
                // Randomize permutation.
                var rand = new Random(seed);

                for (i = 0; i < n; i++)
                {
                    // j = rand integer in range k to n-1
                    j = i + (rand.Next() % (n - i));

                    // swap p[k] and p[j]
                    tmp = p[j];
                    p[j] = p[i];
                    p[i] = tmp;
                }
            }

            return p;
        }

        /// <summary>
        /// Inverts a permutation vector.
        /// </summary>
        /// <param name="p">A permutation vector.</param>
        /// <returns>Returns pinv[i] = k if p[k] = i on input.</returns>
        public static int[] Invert(int[] p)
        {
            int k, n = p.Length;

            int[] pinv = new int[n];

            for (k = 0; k < n; k++)
            {
                // Invert the permutation.
                pinv[p[k]] = k;
            }
            return pinv;
        }

        /// <summary>
        /// Checks whether the <paramref name="p"/> array represents a proper permutation.
        /// </summary>
        /// <param name="p">An array which represents where each integer is permuted too: indices[i]
        /// represents that integer i is permuted to location indices[i].</param>
        /// <returns>True if <paramref name="p"/> represents a proper permutation, <c>false</c> otherwise.</returns>
        public static bool IsValid(int[] p)
        {
            int length = p.Length;

            var check = new bool[length];

            for (int i = 0; i < length; i++)
            {
                if (p[i] >= length || p[i] < 0)
                {
                    return false;
                }

                check[p[i]] = true;
            }

            for (int i = 0; i < length; i++)
            {
                if (check[i] == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
