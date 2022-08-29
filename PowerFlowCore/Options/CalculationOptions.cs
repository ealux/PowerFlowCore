﻿namespace PowerFlowCore
{
    /// <summary>
    /// Calculation settings
    /// </summary>
    public sealed class CalculationOptions
    {

        #region Iterations

        /// <summary>
        /// Solver residual accuracy
        /// </summary>
        public double Accuracy { get; set; } = 1e-5;


        /// <summary>
        /// Maximum number of iterations
        /// </summary>
        public int IterationsCount { get; set; } = 20;

        /// <summary>
        /// Gauss-Seidel solver acceleration rate
        /// </summary>
        public double AccelerationRateGS { get; set; } = 1.0;

        #endregion

        #region Constraints

        /// <summary>
        /// Break the iteration if voltage is less/more then setted constraint (in percentage: 30 -> 130%) toward nominal
        /// </summary>
        public double VoltageConstraintPercentage { get; set; } = 30;

        /// <summary>
        /// Check for <see cref="VoltageConstraint"/> on iteration
        /// </summary>
        public bool UseVoltageConstraint { get; set; } = true;

        #endregion

        #region Logging

        /// <summary>
        /// Logging internal solver's info (on iterations)
        /// </summary>
        public bool SolverInternalLogging { get; set; } = true;

        #endregion

        #region Scheme builder

        /// <summary>
        /// Replaces branches with low or zero impedance with specific breakers 
        /// </summary>
        public bool UseBreakerImpedance { get; set; } = true;

        #endregion
    }
}
