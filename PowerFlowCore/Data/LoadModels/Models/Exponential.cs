using System;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Incapsulate data of Exponential load model.
    /// <para>
    /// <c>Exponential[Load(U)]:  Load = Load_spec*[(U/U0)^<see cref="Exponential.p"/>]</c>
    /// </para>
    /// </summary>
    public class Exponential : ILoadModel
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <inheritdoc/>
        public double? Umin { get; set; }

        /// <inheritdoc/>
        public double? Umax { get; set; }

        /// <inheritdoc/>
        public bool IsValid { get; set; } = true;


        #region Cofficients

        /// <summary>
        /// Voltage dependent power term of Exponential model
        /// </summary>
        public double p { get; set; }

        #endregion

        // Hide empty ctor
        private Exponential() { }

        // Private ctor to using by Initialize method
        private Exponential(string name, double p)
        {
            Name = name;
            Id = Guid.NewGuid();
            this.p = p;
        }

        /// <summary>
        /// Create a new instance of <see cref="Exponential"/> model
        /// </summary>
        /// <param name="name">Model Name</param>
        /// <param name="p">Voltage dependent power term of Exponential model</param>
        /// <returns>Inctance of <see cref="Exponential"/> model</returns>
        public static Exponential Initialize(string name, double p = 0.0)
        {

            var model = new Exponential(name, p);
            model.Validate();
            return model;
        }

        /// <inheritdoc/>
        public double? ApplyModel(double sourceValue,
                                  double sourceU,
                                  double sourceUnom)
        {
            // Check model
            if (IsValid == false)
                return null;

            return sourceValue * Math.Pow(sourceU / sourceUnom, p);
        }

        /// <summary>
        /// Validate <see cref="Exponential"/> model Umin/Umax bound values
        /// </summary>
        public void Validate()
        {
            if (this.Umin.HasValue & this.Umax.HasValue)
            {
                if (this.Umin!.Value >= this.Umax!.Value)
                {
                    Logger.LogWarning($"Umax is less or equal Umin. Model \"{this.Name}\" is invalid!");
                    IsValid = false;
                }
            }
            else
                IsValid = true;
        }

        /// <<inheritdoc/>
        public ILoadModel DeepCopy()
        {
            return Initialize(this.Name, this.p);
        }
    }
}


