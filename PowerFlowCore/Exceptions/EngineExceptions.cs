using System;

namespace PowerFlowCore
{
    /// <summary>
    /// Raise when nominal voltage is less then calculated one
    /// </summary>
    [Serializable]
    internal class VoltageOverflowException:Exception
    {
        public VoltageOverflowException(string Node_info) : base("Недопустимое превышение напряжения! Узел: " + $"{Node_info}") { }
    }


    /// <summary>
    /// Raise when calculated voltage is less then nominal one
    /// </summary>
    [Serializable]
    internal class VoltageLackException : Exception
    {
        public VoltageLackException(string Node_info) : base("Недопустимое cнижение напряжения! Узел: " + $"{Node_info}") { }
    }
}
