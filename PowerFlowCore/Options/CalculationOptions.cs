﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Calculatiator parameters
    /// </summary>
    public sealed class CalculationOptions
    {
        /// <summary>
        /// Method accuracy:
        ///   Gauss-Seidel:   Voltage step defference;
        ///   Newton-Raphson: Power residual tolerance.
        /// </summary>
        public double Accuracy { get; set; } = 1e-8;


        /// <summary>
        /// Maximum number of iterations
        /// </summary>
        public int IterationsCount { get; set; } = 300;


        /// <summary>
        /// Voltage tolerance evaluation towards the nominal one
        /// </summary>
        public double VotageRate { get; set; } = 0.5;


        /// <summary>
        /// Voltage convergence criteria (Newton-Raphson method only)
        /// </summary>
        public double VoltageConvergence { get; set; } = 1e-8;


        /// <summary>
        /// Gauss-Seidel solver acceleration rate
        /// </summary>
        public double AccelerationRateGS { get; set; } = 1.0;
    }
}
