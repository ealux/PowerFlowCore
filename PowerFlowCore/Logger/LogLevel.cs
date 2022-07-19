using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore
{
    /// <summary>
    /// Log level based on warnings
    /// </summary>
    public enum LogLevel: byte
    {
        Info     = 0,
        Success  = 1,
        Warning  = 2,
        Critical = 3,
    }
}
