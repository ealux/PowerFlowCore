using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore
{
    public class LogInfoEventArgs: EventArgs
    {
        public string Message { get; set; }

        public LogInfoEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
