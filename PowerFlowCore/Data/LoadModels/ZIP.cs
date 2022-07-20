using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Incapsulate data of ZIP load model.
    /// <para>
    /// <c>ZIP[P(U)]:  P = P0*[p0 + p1*(U/U0) + p2*(U/U0)^2]</c>
    /// </para>
    /// <para>
    /// <c>ZIP[Q(U)]: Q = Q0*[q0 + q1*(U/U0) + q2*(U/U0)^2]</c>
    /// </para>
    /// </summary>
    public class ZIP : IStaticLoadModel
    {
        /// <summary>
        /// Model Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Model Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Lower voltage bound for model usage
        /// </summary>
        public double? Umin { get; set; }
        /// <summary>
        /// Upper voltage bound for model usage
        /// </summary>
        public double? Umax { get; set; }

        /// <summary>
        /// Indicates for model usage ability
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// Submodels
        /// </summary>
        internal List<ZIP> AdditionalModels { get; set; } = new List<ZIP>();


        #region Active power cofficients

        /// <summary>
        /// P component coefficient of ZIP model (active power model)
        /// </summary>
        public double p0 { get; set; }
        /// <summary>
        /// I component coefficient of ZIP model (active power model)
        /// </summary>
        public double p1 { get; set; }
        /// <summary>
        /// Z component coefficient of ZIP model (active power model)
        /// </summary>
        public double p2 { get; set; }

        #endregion

        #region Reactive power cofficients

        /// <summary>
        /// P component coefficient of ZIP model (reactive power model)
        /// </summary>
        public double q0 { get; set; }
        /// <summary>
        /// I component coefficient of ZIP model (reactive power model)
        /// </summary>
        public double q1 { get; set; }
        /// <summary>
        /// Z component coefficient of ZIP model (reactive power model)
        /// </summary>
        public double q2 { get; set; }

        #endregion


        // Hide empty ctor
        private ZIP() { }

        // Private ctor to using by Initialize method
        private ZIP(string name,double p0, double p1, double p2, double q0, double q1, double q2, double? umin, double? umax)
        {
            Name = name;
            Id = Guid.NewGuid();
            Umin = umin;
            Umax = umax;
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.q0 = q0;
            this.q1 = q1;
            this.q2 = q2;
        }

        /// <summary>
        /// Create a new instance of <see cref="ZIP"/> model
        /// </summary>
        /// <param name="name">Model Name</param>
        /// <param name="p0">P component coefficient of ZIP model (active power model)</param>
        /// <param name="p1">I component coefficient of ZIP model (active power model)</param>
        /// <param name="p2">Z component coefficient of ZIP model (active power model)</param>
        /// <param name="q0">P component coefficient of ZIP model (reactive power model)</param>
        /// <param name="q1">I component coefficient of ZIP model (reactive power model)</param>
        /// <param name="q2">Z component coefficient of ZIP model (reactive power model)</param>
        /// <param name="umin">Lower bound of activation voltage level. Can be null</param>
        /// <param name="umax">Upper bound of activation voltage level. Can be null</param>
        /// <returns>Inctance of <see cref="ZIP"/> model</returns>
        public static ZIP Initialize(string name, 
                                     double p0 = 0.0, double p1 = 0.0, double p2 = 0.0, 
                                     double q0 = 0.0, double q1 = 0.0, double q2 = 0.0,
                                     double? umin = null, double? umax = null)
        {

            var model =  new ZIP(name,
                                 p0, p1, p2,
                                 q0, q1, q2,
                                 umin, umax);

            model.Validate();
            return model;
        }

        /// <summary>
        /// Calculate <see cref="INode.S_calc"/> within ZIP model
        ///  based on <see cref="INode.S_load"/> property,
        ///  actual and nominal voltages.
        ///  Choose appropriate model from base and <see cref="ZIP.AdditionalModels"/>.
        /// </summary>
        /// <param name="node"><see cref="INode"/> to be calculated</param>
        public void ApplyModel(INode node)
        {
            // Check node
            if (node.S_load == System.Numerics.Complex.Zero)
                return;
            if (node.U == System.Numerics.Complex.Zero)
                return;

            List<ZIP> models;
            double P = 0.0;
            double Q = 0.0;

            if (AdditionalModels.Count > 0)
            {
                models = AdditionalModels.ToList();
                models.Add(this);
            }
            else
                models = new List<ZIP>() { this };


            foreach (var item in models)
            {
                if (IsValid == false)
                    continue;                              

                if ((item.Umin.HasValue && item.Umin.Value >= (node.U.Magnitude / node.Unom.Magnitude)) |
                    (item.Umax.HasValue && item.Umax.Value <= (node.U.Magnitude / node.Unom.Magnitude)))
                    continue;

                P = node.S_load.Real * (item.p0 +
                                        item.p1 * (node.U.Magnitude / node.Unom.Magnitude) +
                                        item.p2 * Math.Pow((node.U.Magnitude / node.Unom.Magnitude), 2));

                Q = node.S_load.Imaginary * (item.q0 +
                                             item.q1 * (node.U.Magnitude / node.Unom.Magnitude) +
                                             item.q2 * Math.Pow((node.U.Magnitude / node.Unom.Magnitude), 2));
            }

            if (P == 0.0 & Q == 0.0)
                return;
            else
                node.S_calc = new System.Numerics.Complex(P, Q);
        }


        #region [Sample ZIP Models]

        /// <summary>
        /// Standard, practically received 
        /// industrial load model characteristics for 110 kV node
        /// </summary>
        public static ZIP IndustrialLoad_110kV()
        {
            var model = ZIP.Initialize("IndustrialLoad_110kV (from 0.815 to 1.2)",
                                       p0: 0.83, p1: -0.3, p2: 0.47,
                                       q0: 3.7, q1: -7.0, q2: 4.3,
                                       umin: 0.815, umax: 1.2);
            return model;
        }

        /// <summary>
        /// Standard, practically received 
        /// industrial load model characteristics for 35 kV node
        /// </summary>
        public static ZIP IndustrialLoad_35kV()
        {
            var model = ZIP.Initialize("IndustrialLoad_35kV (from 0.815 to 1.2)",
                                       p0: 0.83,  p1: -0.3,  p2: 0.47,
                                       q0: 4.90,  q1: -10.1, q2: 6.20,
                                       umin: 0.815, umax: 1.2);
            return model;
        }

        #endregion
    }

}
