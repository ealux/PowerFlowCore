using System;
using System.Collections.Generic;
using System.Text;

using Complex = System.Numerics.Complex;


namespace PowerFlowCore.Data
{
    /// <summary>
    /// Incapsulate data of ZIP load model.
    /// <para>
    /// <c>ZIP[Load(U)]:  Load = Load_spec*[a0 + a1*(U/U0) + a2*(U/U0)^2]</c>
    /// </para>
    /// </summary>
    public class ZIP : ILoadModel
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
        /// P component coefficient of ZIP model
        /// </summary>
        public double a0 { get; set; }
        /// <summary>
        /// I component coefficient of ZIP model
        /// </summary>
        public double a1 { get; set; }
        /// <summary>
        /// Z component coefficient of ZIP model
        /// </summary>
        public double a2 { get; set; }

        #endregion


        // Hide empty ctor
        private ZIP() { }

        // Private ctor to using by Initialize method
        private ZIP(string name, double a0, double a1, double a2)
        {
            Name = name;
            Id = Guid.NewGuid();
            this.a0 = a0;
            this.a1 = a1;
            this.a2 = a2;
        }

        /// <summary>
        /// Create a new instance of <see cref="ZIP"/> model
        /// </summary>
        /// <param name="name">Model Name</param>
        /// <param name="a0">P component coefficient of ZIP model</param>
        /// <param name="a1">I component coefficient of ZIP model</param>
        /// <param name="a2">Z component coefficient of ZIP model</param>
        /// <returns>Inctance of <see cref="ZIP"/> model</returns>
        public static ZIP Initialize(string name,
                                     double a0 = 0.0, 
                                     double a1 = 0.0, 
                                     double a2 = 0.0)
        {

            var model = new ZIP(name, a0, a1, a2);
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

            return sourceValue * (this.a0 +
                                  this.a1 * (sourceU / sourceUnom) +
                                  this.a2 * Math.Pow(sourceU / sourceUnom, 2));             
        }

        /// <summary>
        /// Validate <see cref="ZIP"/> model on coefficients sum is equal to 1.0
        /// and Umin/Umax bounds value
        /// </summary>
        public void Validate()
        {
            if (Math.Round(this.a0 + this.a1 + this.a2, 5) != 1.0)
            {
                Logger.LogWarning($"Coefficient sum of ZIP model is not equal to 1.0! Model \"{this.Name}\" is invalid!");
                IsValid = false;
            }
            else if (this.Umin.HasValue & this.Umax.HasValue)
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
    }
}
