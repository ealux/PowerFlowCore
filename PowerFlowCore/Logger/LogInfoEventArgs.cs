using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore
{
    /// <summary>
    /// Arguments with <see cref="Logger.LogBroadcast" event/ Contains log message/>
    /// </summary>
    public class LogInfoEventArgs: EventArgs
    {
        /// <summary>
        /// Log message
        /// </summary>
        public string Message { get; set; }

        public LogInfoEventArgs(string message) => this.Message = message;
    }
}
