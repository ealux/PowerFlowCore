using System;
using System.Linq;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Extention methods to work with <see cref="ZIP"/> models
    /// </summary>
    public static class ZIPExtentions
    {
        /// <summary>
        /// Add and validate new submodel to parent <see cref="ZIP"/> model
        /// </summary>
        /// <param name="parentModel">Parent <see cref="ZIP"/> model</param>
        /// <param name="childModel">Model to be added to <see cref="ZIP.AdditionalModels"/> list of parent</param>
        /// <returns>Parent <see cref="ZIP"/> model</returns>
        public static ZIP AddModel(this ZIP parentModel, ZIP childModel)
        {
            if (childModel == null)
                throw new ArgumentNullException(nameof(childModel));

            parentModel.AdditionalModels.Add(childModel);
            ValidateWithAdditional(parentModel);
            return parentModel;
        }

        /// <summary>
        /// Validate <see cref="ZIP"/> model on Umin/Umax ranges with submodels
        /// </summary>
        /// <param name="model">Parent model</param>
        private static void ValidateWithAdditional(this ZIP model)
        {
            if (model.AdditionalModels.Count == 0)
                return;

            var models = model.AdditionalModels.ToList();
            models.Add(model);

            //Check for whole range cover
            if (models.Any(m => !m.Umax.HasValue & !m.Umin.HasValue))
            {
                var mod = models.Where(m => !m.Umax.HasValue & !m.Umin.HasValue).First();
                Logger.LogWarning($"Model cover all voltrage range with another model existing. Now works only with model \"{mod.Name}\"!");
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
        /// Validate <see cref="ZIP"/> model and inner submodels
        ///  by coeffients and Umin/Umax ranges
        /// </summary>
        /// <param name="model"><see cref="ZIP"/> model to be validated</param>
        /// <returns><see cref="ZIP"/> model after validation</returns>
        public static ZIP Validate(this ZIP model)
        {
            string alarm = "";

            if (Math.Round(model.p0 + model.p1 + model.p2, 5) != 1.0)
                    alarm += "P ";
            if (Math.Round(model.q0 + model.q1 + model.q2, 5) != 1.0)
                alarm += "Q";
            if (!string.IsNullOrEmpty(alarm.Trim()))
            {
                Logger.LogWarning($"Coefficient ({alarm}) sum of ZIP model is not equal to 1.0! Model \"{model.Name}\" is invalid!");
                model.IsValid = false;
            }

            if (model.Umin.HasValue & model.Umax.HasValue)
                if (model.Umin!.Value >= model.Umax!.Value)
                {
                    Logger.LogWarning($"Umax is less or equal Umin. Model \"{model.Name}\" is invalid!");
                    model.IsValid = false;
                }

            if(model.AdditionalModels.Count > 0)
                ValidateWithAdditional(model);

            return model;
        }
    }
}
