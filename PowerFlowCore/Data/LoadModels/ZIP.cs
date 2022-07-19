using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Incapsulate data of ZIP Load Model.
    /// <para>
    /// ZIP(U): P = P0*[p0 + p1*(U/U0) + p2*(U/U0)^2]
    /// </para>
    /// </summary>
    public struct ZIP : IStaticLoadModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public double? Umin { get; set; }
        public double? Umax { get; set; }

        public bool IsValid { get; set; }

        public double p0;
        public double p1;
        public double p2;
                        
        public double q0;
        public double q1;
        public double q2;

        public ZIP(string name, 
                   double p0 = 0.0, double p1 = 0.0, double p2 = 0.0, 
                   double q0 = 0.0, double q1 = 0.0, double q2 = 0.0,
                   double? umin = null, double? umax = null)
        {
            Name = name;
            Id = Guid.NewGuid();

            this.p0 = p0;
            this.q0 = q0;
            this.p1 = p1;
            this.q1 = q1;
            this.p2 = p2;
            this.q2 = q2;

            this.Umin = umin;
            this.Umax = umax;

            this.IsValid = true;

            if (p0 + p1 + p2 != 1.0 || q0 + q1 + q2 != 1.0)
            {
                Logger.LogCritical($"Coefficient sum of ZIP model \"{Name}\" is not equal to 1.0!");
                IsValid = false;
            }
        }

        /// <summary>
        /// Calculate <see cref="INode.S_calc"/> within ZIP model
        ///  based on <see cref="INode.S_load"/> property,
        ///  actual and nominal voltages.
        /// </summary>
        /// <param name="node"><see cref="INode"/> to be calculated</param>
        public void ApplyModel(INode node)
        {
            if (node.S_load == System.Numerics.Complex.Zero)
                return;
            if(node.U == System.Numerics.Complex.Zero)
                return;
            if (this.p0 == 0.0 & this.p1 == 0.0 & this.p2 == 0.0
              & this.q0 == 0.0 & this.q1 == 0.0 & this.q2 == 0.0)
                return;
            if (IsValid == false)
                return;

            double P, Q;
            bool calc = true;

            if((Umin.HasValue && Umin.Value >= node.U.Magnitude) |
               (Umax.HasValue && Umax.Value <= node.U.Magnitude))
                calc = false;

            if (calc)
            {
                P = node.S_load.Real * (p0 +
                                        p1 * (node.U.Magnitude / node.Unom.Magnitude) +
                                        p2 * Math.Pow((node.U.Magnitude / node.Unom.Magnitude), 2));

                Q = node.S_load.Imaginary * (q0 +
                                             q1 * (node.U.Magnitude / node.Unom.Magnitude) +
                                             q2 * Math.Pow((node.U.Magnitude / node.Unom.Magnitude), 2));
            }
            else
                return;            

            node.S_calc = new System.Numerics.Complex(P, Q);
        }
    }
}
