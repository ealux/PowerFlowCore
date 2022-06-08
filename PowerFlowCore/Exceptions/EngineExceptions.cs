using System;
using PowerFlowCore.Data;

namespace PowerFlowCore
{
    /// <summary>
    /// Raise when nominal voltage is less then calculated one
    /// </summary>
    [Serializable]
    internal class VoltageOverflowException:Exception
    {
        public VoltageOverflowException(INode node) : base("Voltage overflow! Node: " + $"{node.Num}") { }
    }


    /// <summary>
    /// Raise when calculated voltage is less then nominal one
    /// </summary>
    [Serializable]
    internal class VoltageLackException : Exception
    {
        public VoltageLackException(INode node) : base("Voltage lack! Node: " + $"{node.Num}") { }
    }
}
