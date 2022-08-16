namespace PowerFlowCore
{
    /// <summary>
    /// Calculation settings
    /// </summary>
    public sealed class CalculationOptions
    {
        /// <summary>
        /// Solver residual accuracy
        /// </summary>
        public double Accuracy { get; set; } = 1e-5;


        /// <summary>
        /// Maximum number of iterations
        /// </summary>
        public int IterationsCount { get; set; } = 25;


        /// <summary>
        /// Voltage tolerance evaluation towards the nominal one
        /// </summary>
        public double VotageRate { get; set; } = 0.5;


        /// <summary>
        /// Gauss-Seidel solver acceleration rate
        /// </summary>
        public double AccelerationRateGS { get; set; } = 1.0;


        /// <summary>
        /// Logging internal solver's info (on iterations)
        /// </summary>
        public bool SolverInternalLogging { get; set; } = true;

        /// <summary>
        /// Replaces branches with low or no impedance with specific breakers 
        /// </summary>
        public bool UseBreakerImpedance { get; set; } = true;
    }
}
