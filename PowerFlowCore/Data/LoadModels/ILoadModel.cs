using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore.Data
{
    public interface ILoadModel
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
        public bool IsValid { get; set; }

        /// <summary>
        /// Apply <see cref="ILoadModel"/> model to <see cref="INode.S_load"/> specified load
        /// </summary>
        /// <param name="sourceValue">Initial <see cref="INode.S_load"/> specified load</param>
        /// <param name="sourceU"><see cref="INode.U"/> calculated voltage</param>
        /// <param name="sourceUnom"><see cref="INode.Unom"/> nominal voltage</param>
        /// <returns>Result calculated value to be used as <see cref="INode.S_calc"/> 
        /// or null if model is invalid</returns>
        public double? ApplyModel(double sourceValue,
                                  double sourceU,
                                  double sourceUnom);

        /// <summary>
        /// Validate model
        /// </summary>
        public void Validate();

        /// <summary>
        /// Make full cope of load model
        /// </summary>
        /// <returns>New instance of <see cref="ILoadModel"/></returns>
        public ILoadModel DeepCopy();
    }
}
