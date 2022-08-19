using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Incapsulate data of Linear load model.
    /// <para>
    /// <c>Linear[Load(U)]:  Load = Load_spec*[<see cref="Linear.a"/> + <see cref="Linear.b"/>*(U/U0)]</c>
    /// </para>
    /// </summary>
    public class Linear: ILoadModel
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
        /// Free term of Linear model
        /// </summary>
        public double a { get; set; }
        /// <summary>
        /// Voltage dependent term of Linear model
        /// </summary>
        public double b { get; set; }

        #endregion

        // Hide empty ctor
        private Linear() { }

        // Private ctor to using by Initialize method
        private Linear(string name, double a, double b)
        {
            Name = name;
            Id = Guid.NewGuid();
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// Create a new instance of <see cref="Linear"/> model
        /// </summary>
        /// <param name="name">Model Name</param>
        /// <param name="a">Free term of Linear model</param>
        /// <param name="b">Voltage dependent term of Linear model</param>
        /// <returns>Inctance of <see cref="Linear"/> model</returns>
        public static Linear Initialize(string name,
                                        double a = 0.0,
                                        double b = 0.0)
        {

            var model = new Linear(name, a, b);
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

            return sourceValue * (this.a +
                                  this.b * (sourceU / sourceUnom));
        }

        /// <summary>
        /// Validate <see cref="Linear"/> model on coefficients have not 0.0 value
        /// and Umin/Umax bounds value
        /// </summary>
        public void Validate()
        {
            if (Math.Round(this.a + this.b, 5) == 1.0)
            {
                Logger.LogWarning($"Coefficients of Linear model is equal to 0! Model \"{this.Name}\" is invalid!");
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


        /// <<inheritdoc/>
        public ILoadModel DeepCopy()
        {
            return Initialize(this.Name, this.a, this.b);
        }
    }
}


