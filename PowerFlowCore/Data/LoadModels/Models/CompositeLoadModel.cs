using System;
using System.Collections.Generic;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Basic load model. Include:
    /// <para><see cref="P"/> - Active power load model</para>
    /// <para><see cref="Q"/> - Reactive power load model</para>
    /// <para><see cref="Umax"/> and <see cref="Umin"/> - Voltage limits to apply model</para>
    /// <para>Also includes list of submodels of <see cref="CompositeLoadModel"/> with own voltage limits. To add submodel:</para>
    /// <para><code>ParentModel.AddModel(ChildModel)</code></para>
    /// </summary>
    [Serializable]
    public sealed class CompositeLoadModel
    {
        #region ILoadModel properties

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

        #endregion


        /// <summary>
        /// <see cref="ILoadModel"/> to apply to load Active power
        /// </summary>
        public ILoadModel P { get; set; }

        /// <summary>
        /// <see cref="ILoadModel"/> to apply to load Reactive power
        /// </summary>
        public ILoadModel Q { get; set; }

        /// <summary>
        /// Collection of <see cref="CompositeLoadModel"/> to be added to parent model
        /// </summary>
        internal List<CompositeLoadModel> SubModels { get; set; } = new List<CompositeLoadModel>();


        // Private ctor
        private CompositeLoadModel() => this.Id = new Guid();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="P"></param>
        /// <param name="Q"></param>
        /// <param name="umin"></param>
        /// <param name="umax"></param>
        /// <returns></returns>
        public static CompositeLoadModel Initialize(ILoadModel P = null,
                                                    ILoadModel Q = null,
                                                    double? umin = null, double? umax = null)
        {
            var model = new CompositeLoadModel();

            if (P != null)
            {
                model.P = P;
                model.P.Umin = umin;
                model.P.Umax = umax;
            }
            if (Q != null)
            {
                model.Q = Q;
                model.Q.Umin = umin;
                model.Q.Umax = umax;
            }

            model.Umin = umin;
            model.Umax = umax;

            model.Validate();
            return model;
        }

        /// <summary>
        /// Calculate <see cref="INode.S_calc"/> with <see cref="CompositeLoadModel"/>
        ///  based on <see cref="INode.S_load"/> property,
        ///  actual and nominal voltages.
        ///  Choose appropriate model from submodels list if exists.
        /// </summary>
        /// <param name="node"><see cref="INode"/> to be calculated</param>
        public void ApplyModel(INode node)
        {
            // Check node
            if (node.S_load == System.Numerics.Complex.Zero)
                return;
            if (node.U == System.Numerics.Complex.Zero)
                return;

            List<ILoadModel> modelsP = new List<ILoadModel>();
            List<ILoadModel> modelsQ = new List<ILoadModel>();
            double p = 0.0;
            double q = 0.0;

            if (P != null && P.IsValid) modelsP.Add(P);
            if (Q != null && Q.IsValid) modelsQ.Add(Q);

            // Find all submodels
            if (SubModels.Count > 0)
            {
                (List<ILoadModel> subsP, List<ILoadModel> subsQ) = CompositeLoadModelExtentions.FindRecursive(this);
                modelsP.AddRange(subsP);
                modelsQ.AddRange(subsQ);
            }

            // Apply model
            foreach (var item in modelsP)
            {
                if ((item.Umin.HasValue && item.Umin.Value >= (node.U.Magnitude / node.Unom.Magnitude)) |
                    (item.Umax.HasValue && item.Umax.Value <= (node.U.Magnitude / node.Unom.Magnitude)))
                    continue;

                p = item.ApplyModel(node.S_load.Real, node.U.Magnitude, node.Unom.Magnitude) ?? 0.0;
                break;
            }
            foreach (var item in modelsQ)
            {
                if ((item.Umin.HasValue && item.Umin.Value >= (node.U.Magnitude / node.Unom.Magnitude)) |
                    (item.Umax.HasValue && item.Umax.Value <= (node.U.Magnitude / node.Unom.Magnitude)))
                    continue;

                q = item.ApplyModel(node.S_load.Imaginary, node.U.Magnitude, node.Unom.Magnitude) ?? 0.0;
                break;
            }

            if (p == 0.0 & q == 0.0)
                return;
            else if (p == 0.0 & q != 0.0)
                node.S_calc = new System.Numerics.Complex(node.S_load.Real, q);
            else if (p != 0.0 & q == 0.0)
                node.S_calc = new System.Numerics.Complex(p, node.S_load.Imaginary);
            else
                node.S_calc = new System.Numerics.Complex(p, q);
        }

        /// <summary>
        /// Validate <see cref="CompositeLoadModel"/> by 
        /// <see cref="CompositeLoadModel.P"/> and <see cref="CompositeLoadModel.Q"/> submodels existance
        /// </summary>
        public void Validate()
        {
            if (P == null & Q == null)
                IsValid = false;
            else if (P != null | Q != null)
            {
                if ((Q == null & P != null) && P!.IsValid == false)
                    IsValid = false;
                else if ((P == null & Q != null) && Q!.IsValid == false)
                    IsValid = false;
                else if (P != null & Q != null)
                    if (P!.IsValid == false & Q!.IsValid == false)
                        IsValid = false;
            }
            else
                IsValid = true;
        }

        /// <summary>
        /// Make full copy instance
        /// </summary>
        /// <returns>New instance of <see cref="CompositeLoadModel"/></returns>
        public CompositeLoadModel DeepCopy()
        {
            var model = Initialize(P?.DeepCopy(), Q?.DeepCopy(), Umin, Umax);

            foreach (var mod in SubModels)
                model.SubModels.Add(Initialize(mod.P?.DeepCopy(), mod.Q?.DeepCopy(), mod.Umin, mod.Umax));

            return model;
        }



        #region Template Models

        /// <summary>
        /// Presents template <see cref="CompositeLoadModel"/> for 110 kV 
        /// complex (industrial, residental etc.) load node.
        /// </summary>
        public static CompositeLoadModel ComplexLoadNode_110kV()
        {
            var model = CompositeLoadModel.Initialize(P: ZIP.Initialize("IndustrialLoad_110kV P (less then 0.815 Unom)", a0: 0.83, a1: -0.3, a2: 0.47),
                                                      Q: Linear.Initialize("IndustrialLoad_110kV Q (less then 0.815 Unom)", a: 0.721, b: 0.158),
                                                      umin: null, umax: 0.81499)
                                      .AddModel(CompositeLoadModel.Initialize
                                                 (P: ZIP.Initialize("IndustrialLoad_110kV P (from 0.815 to 1.2 Unom)", a0: 0.83, a1: -0.3, a2: 0.47),
                                                  Q: ZIP.Initialize("IndustrialLoad_110kV Q (from 0.815 to 1.2 Unom)", a0: 3.7, a1: -7.0, a2: 4.3),
                                                  umin: 0.815, umax: 1.19999))
                                      .AddModel(CompositeLoadModel.Initialize
                                                 (P: ZIP.Initialize("IndustrialLoad_110kV P (over 1.2 Unom)", a0: 0.83, a1: -0.3, a2: 0.47),
                                                  Q: Linear.Initialize("IndustrialLoad_110kV Q (over 1.2 Unom)", a: 1.49, b: 0),
                                                  umin: 1.2, umax: null));

            return model;
        }

        /// <summary>
        /// Presents template <see cref="CompositeLoadModel"/> for 35 kV 
        /// complex (industrial, residental etc.) load node.
        /// </summary>
        public static CompositeLoadModel ComplexLoadNode_35kV()
        {
            var model = CompositeLoadModel.Initialize(P: ZIP.Initialize("IndustrialLoad_110kV P (less then 0.815 Unom)", a0: 0.83, a1: -0.3, a2: 0.47),
                                                  Q: Linear.Initialize("IndustrialLoad_110kV Q (less then 0.815 Unom)", a: 0.657, b: 0.158),
                                                  umin: null, umax: 0.81499)
                                      .AddModel(CompositeLoadModel.Initialize
                                                 (P: ZIP.Initialize("IndustrialLoad_110kV P (from 0.815 to 1.2 Unom)", a0: 0.83, a1: -0.3, a2: 0.47),
                                                  Q: ZIP.Initialize("IndustrialLoad_110kV Q (from 0.815 to 1.2 Unom)", a0: 4.9, a1: -10.1, a2: 6.2),
                                                  umin: 0.815, umax: 1.19999))
                                      .AddModel(CompositeLoadModel.Initialize
                                                 (P: ZIP.Initialize("IndustrialLoad_110kV P (over 1.2 Unom)", a0: 0.83, a1: -0.3, a2: 0.47),
                                                  Q: Linear.Initialize("IndustrialLoad_110kV Q (over 1.2 Unom)", a: 1.708, b: 0),
                                                  umin: 1.2, umax: null));

            return model;
        }

        #endregion

    }
}
