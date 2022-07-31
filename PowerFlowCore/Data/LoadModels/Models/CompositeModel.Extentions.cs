using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Extention methods to work with <see cref="CompositeLoadModel"/>
    /// </summary>
    public static partial class CompositeLoadModelExtentions
    {
        #region Public methods


        /// <summary>
        /// Add and validate new submodel to parent <see cref="CompositeLoadModel"/>
        /// </summary>
        /// <param name="parentModel">Parent <see cref="CompositeLoadModel"/></param>
        /// <param name="childModel">Model to be added to <see cref="CompositeLoadModel.SubModels"/> list of parent</param>
        /// <returns>Parent <see cref="CompositeLoadModel"/></returns>
        public static CompositeLoadModel AddModel(this CompositeLoadModel parentModel,
                                                   CompositeLoadModel childModel)
        {
            if (childModel == null)
                throw new ArgumentNullException(nameof(childModel));

            parentModel.SubModels.Add(childModel);
            VallidateAggregation(parentModel);
            return parentModel;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Validate <see cref="CompositeLoadModel"/> model on Umin/Umax ranges with submodels
        /// </summary>
        /// <param name="model">Parent model</param>
        private static void VallidateAggregation(this CompositeLoadModel model)
        {
            if (model.SubModels.Count == 0)
                return;

            List<ILoadModel> modelsP = new List<ILoadModel>();
            List<ILoadModel> modelsQ = new List<ILoadModel>();

            if (model.P != null && model.P.IsValid) modelsP.Add(model.P);
            if (model.Q != null && model.Q.IsValid) modelsQ.Add(model.Q);

            // Find all submodels
            if (model.SubModels.Count > 0)
            {
                (List<ILoadModel> subsP, List<ILoadModel> subsQ) = FindRecursive(model);
                modelsP.AddRange(subsP);
                modelsQ.AddRange(subsQ);
            }

            modelsP.ValidateList();
            modelsQ.ValidateList();            
        }
        
        /// <summary>
        /// Inner validation of models list
        /// </summary>
        /// <param name="models">Input <see cref="ILoadModel"/> list (P or Q)</param>
        private static void ValidateList(this List<ILoadModel> models)
        {
            if (models == null || models.Count == 0)
                return;

            //Check for whole range cover
            if (models.Any(m => !m.Umax.HasValue & !m.Umin.HasValue))
            {
                var mod = models.Where(m => !m.Umax.HasValue & !m.Umin.HasValue).First();
                Logger.LogWarning($"Model cover all voltage range including another existing models. Now works only with model \"{mod.Name}\"!");
                foreach (var item in models.Where(m => m.Id != mod.Id &
                                                       m.IsValid == true))
                {
                    item.IsValid = false;
                    Logger.LogWarning($"Model \"{item.Name}\" was disabled!");
                }
                return;
            }

            // Are models there?
            if (models.Where(m => m.IsValid).Count() < 1)
                return;

            // Analyze
            foreach (var outer in models)
            {
                foreach (var inner in models.Where(m => m.Id != outer.Id))
                {
                    if (!inner.IsValid)
                        continue;

                    // No Umax/Umin value both in inner and outer
                    if ((!outer.Umax.HasValue & !inner.Umax.HasValue) |
                       (!outer.Umin.HasValue & !inner.Umin.HasValue))
                    {
                        Logger.LogWarning($"Intersecting ranges. Models \"{outer.Name}\" and \"{inner.Name}\" are not valid!");
                        outer.IsValid = false;
                        inner.IsValid = false;
                        continue;
                    }

                    // With all values
                    if (inner.Umin.HasValue & inner.Umax.HasValue &
                        outer.Umin.HasValue & outer.Umax.HasValue)
                    {
                        // On equals values
                        if (inner.Umax!.Value == outer.Umax!.Value |
                            inner.Umax!.Value == outer.Umin!.Value |
                            inner.Umin!.Value == outer.Umax!.Value |
                            inner.Umin!.Value == outer.Umin!.Value)
                        {
                            Logger.LogWarning($"Equal bounds in Umin/Umax ranges. Models \"{outer.Name}\" and \"{inner.Name}\" are not valid!");
                            outer.IsValid = false;
                            inner.IsValid = false;
                            continue;
                        }
                        // On ranges intersection
                        if ((outer.Umax!.Value < inner.Umax!.Value & outer.Umax!.Value > inner.Umin!.Value) |
                            (inner.Umax!.Value < outer.Umax!.Value & inner.Umax!.Value > outer.Umin!.Value) |
                            (outer.Umin!.Value < inner.Umax!.Value & outer.Umin!.Value > inner.Umin!.Value) |
                            (inner.Umin!.Value < outer.Umax!.Value & inner.Umin!.Value > outer.Umin!.Value))
                        {
                            Logger.LogWarning($"Intersecting ranges. Models \"{outer.Name}\" and \"{inner.Name}\" are not valid!");
                            outer.IsValid = false;
                            inner.IsValid = false;
                            continue;
                        }
                    }

                    // Umax has no value
                    if (!outer.Umax.HasValue & inner.Umax.HasValue &
                         outer.Umin.HasValue & outer.Umax.HasValue)
                    {
                        if (inner.Umax!.Value > outer.Umin!.Value)
                        {
                            Logger.LogWarning($"Intersecting ranges (unbound Umax). Models \"{outer.Name}\" and \"{inner.Name}\" are not valid!");
                            outer.IsValid = false;
                            inner.IsValid = false;
                            continue;
                        }
                    }
                    else if (outer.Umax.HasValue & !inner.Umax.HasValue &
                            outer.Umin.HasValue & outer.Umax.HasValue)
                    {
                        if (outer.Umax!.Value > inner.Umin!.Value)
                        {
                            Logger.LogWarning($"Intersecting ranges (unbound Umax). Models \"{outer.Name}\" and \"{inner.Name}\" are not valid!");
                            outer.IsValid = false;
                            inner.IsValid = false;
                            continue;
                        }
                    }
                    // Umin has no value
                    else if (outer.Umax.HasValue & inner.Umax.HasValue &
                            !outer.Umin.HasValue & outer.Umax.HasValue)
                    {
                        if (inner.Umin!.Value < outer.Umax!.Value)
                        {
                            Logger.LogWarning($"Intersecting ranges (unbound Umin). Models \"{outer.Name}\" and \"{inner.Name}\" are not valid!");
                            outer.IsValid = false;
                            inner.IsValid = false;
                            continue;
                        }
                    }
                    else if (outer.Umax.HasValue & inner.Umax.HasValue &
                             outer.Umin.HasValue & !outer.Umax.HasValue)
                    {
                        if (outer.Umin!.Value < inner.Umax!.Value)
                        {
                            Logger.LogWarning($"Intersecting ranges (unbound Umin). Models \"{outer.Name}\" and \"{inner.Name}\" are not valid!");
                            outer.IsValid = false;
                            inner.IsValid = false;
                            continue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recursively collects all valid submodels in aggregation model
        /// </summary>
        /// <param name="model"><see cref="CompositeLoadModel"/> to find in</param>
        internal static (List<ILoadModel> P, List<ILoadModel> Q) FindRecursive(CompositeLoadModel model)
        {
            var listOutP = new List<ILoadModel>();
            var listOutQ = new List<ILoadModel>();

            foreach (var item in model.SubModels)
            {
                if (item.P != null && item.P.IsValid == true) listOutP.Add(item.P);
                if (item.Q != null && item.Q.IsValid == true) listOutQ.Add(item.Q);

                if (item.SubModels.Count > 0)
                {
                    (List<ILoadModel> subsP, List<ILoadModel> subsQ) = FindRecursive(item);
                    listOutP.AddRange(subsP);
                    listOutQ.AddRange(subsQ);
                }
            }

            return (listOutP, listOutQ);
        }

        #endregion

    }
}
