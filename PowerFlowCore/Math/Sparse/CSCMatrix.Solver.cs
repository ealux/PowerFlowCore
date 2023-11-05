namespace PowerFlowCore.Algebra
{
    /// <summary>
    /// Contatins LU sparse colver routine
    /// </summary>
    public static class CSCMatrixSolver
    {   
        /// <summary>
        /// Sovle directly sparse system Ax=b by LU decomposition
        /// </summary>
        /// <param name="A">Sparce matrix</param>
        /// <param name="b">Right side dense vector</param>
        /// <returns>Result of sparse system solving</returns>
        public static double[] Solve(CSCMatrix A, double[] b)
        {
            var res = new double[b.Length];     // allocate result
            var spLU = LU.Create(A, 1, 1.0);    // LU decomposition

            spLU.Solve(b, res);                 // Solve system

            return res;
        }
    }
}