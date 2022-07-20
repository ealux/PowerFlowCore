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
        public string Source { get; set; }

        /// <summary>
        /// Logger message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Message date and time
        /// </summary>
        public string DateTimeStamp { get; set; }

        /// <summary>
        /// Logger message level (Info, Success, Warning or Critical)
        /// </summary>
        public LogLevel Level { get; set; }
    }
}
