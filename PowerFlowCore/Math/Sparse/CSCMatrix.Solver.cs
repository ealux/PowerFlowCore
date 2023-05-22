using System;

namespace PowerFlowCore.Algebra
{
    public static class CSCMatrixSolver
    {   
        public static double[] Solve(CSCMatrix A, double[] b)
        {
            var res = new double[b.Length];
            var spLU = SparseLU.Create(A, 1, 1.0);

            spLU.Solve(b, res);
            
            return res;
        }
    }
}