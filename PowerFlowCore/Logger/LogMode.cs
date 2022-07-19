using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore
{
    /// <summary>
    /// Logger modes for messages output 
    /// <list>
    /// <item>Console -> Send messages to standart console</item>
    /// <item>Debug   -> Send messages to standart debug tools</item>
    /// <item>Custom  -> Slack/Reference node</item>
    /// </list>
    /// </summary>
    public enum LogMode: byte
    {
        Console = 0,
        Debug   = 1,
        Custom  = 2
    }
}
