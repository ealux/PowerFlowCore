
namespace PowerFlowCore
{
    /// <summary>
    /// Engine's calculatiator parameters
    /// </summary>
    public class EngineOptions
    {
        /// <summary>
        /// Method accuracy:
        /// Gauss-Seidel:   Voltage step defference;
        /// Newton-Raphson: Power residual tolerance.
        /// </summary>
        public double accuracy { get; set; } = 1e-6;


        /// <summary>
        /// Maximum number of iterations
        /// </summary>
        public int iterations { get; set; } = 150;


        /// <summary>
        /// Voltage tolerance evaluation towards the nominal one
        /// </summary>
        public double votageRatio { get; set; } = 0.25;


        /// <summary>
        /// Voltage convergence criteria for Newton-Raphson method
        /// </summary>
        public double voltageConvergence { get; set; } = 1e-6;
    }
}
