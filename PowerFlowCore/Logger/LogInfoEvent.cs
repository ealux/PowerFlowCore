using System;

using PowerFlowCore.Data;

namespace PowerFlowCore
{
    /// <summary>
    /// Logger broadcast event type
    /// </summary>
    /// <param name="SenderId">Sender identificator. Can be <see cref="Grid.Id"/> OR <see cref="String.Empty"/></param>
    /// <param name="Message">Logger message</param>
    public delegate void LogInfoEvent (string SenderId, string Message);
}
