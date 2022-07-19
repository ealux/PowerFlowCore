using System;
using System.Collections.Generic;

namespace PowerFlowCore.Data
{
    public interface IStaticLoadModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public double? Umin { get; set; }
        public double? Umax { get; set; }

        public bool IsValid { get; set; }

        public void ApplyModel(INode node);
    }
}