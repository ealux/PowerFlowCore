
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
        public double Accuracy { get; set; } = 1e-6;


        /// <summary>
        /// Maximum number of iterations
        /// </summary>
        public int IterationsCount { get; set; } = 150;


        /// <summary>
        /// Voltage tolerance evaluation towards the nominal one
        /// </summary>
        public double VotageRatio { get; set; } = 0.25;


        /// <summary>
        /// Voltage convergence criteria for Newton-Raphson method
        /// </summary>
        public double VoltageConvergence { get; set; } = 1e-6;

        /// <summary>
        /// Choosen calculation method (def.: Newton-Raphson)
        /// </summary>
        public CalculationMethod CalculationMethod { get; set; } = CalculationMethod.Newton;
    }


    /// <summary>
    /// Method to calculate power flow problem
    /// </summary>
    public enum CalculationMethod
    {
        Newton, 
        Gauss
    }
}
