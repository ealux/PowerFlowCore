using PowerFlowCore.Data;

namespace PowerFlowCore
{
    /// <summary>
    /// Message from <see cref="Logger"/> with attributes
    /// </summary>
    public struct LoggerMessage
    {        
        /// <summary>
        /// Message source (<see cref="Grid.Id"/> or another)
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Logger message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Message date and time
        /// </summary>
        public string DateTimeStamp { get; private set; }

        /// <summary>
        /// Logger message level (Info, Success, Warning or Critical)
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// Initialize <see cref="Logger"/> message
        /// </summary>
        /// <param name="source"><see cref="Grid"/> id or another string identifier</param>
        /// <param name="message">Message content</param>
        /// <param name="dateTimeStamp">Time stamp</param>
        /// <param name="level"><see cref="LogLevel"/> status</param>
        public LoggerMessage(string source, LogLevel level, string dateTimeStamp, string message)
        {
            Source = source;
            Level = level;
            DateTimeStamp = dateTimeStamp;
            Message = message;
        }

    }
}
