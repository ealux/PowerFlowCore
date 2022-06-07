using System;

using PowerFlowCore.Data;

namespace PowerFlowCore
{
    /// <summary>
    /// Logger broadcast event type
    /// </summary>
    /// <param name="SenderId"><see cref="String.Empty"/> OR source <see cref="Grid.Id"/></param>
    /// <param name="Message">Logger message</param>
    public delegate void LogInfoEvent (string SenderId, string Message);
}
