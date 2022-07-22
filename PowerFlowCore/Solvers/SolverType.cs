namespace PowerFlowCore.Solvers
{
    /// <summary>
    /// Presents solver type to applied in grid calculation
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        /// Apply Newton-Rapshon solver
        /// </summary>
        NewtonRaphson = 0,
        /// <summary>
        /// Apply Gauss-Seidel solver
        /// </summary>
        GaussSeidel = 1
    }
}
