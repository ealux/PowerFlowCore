using PowerFlowCore.Data;

namespace PowerFlowCore
{
    /// <summary>
    /// Custom logger listener to receive messages
    /// </summary>
    public interface ILoggerListener
    {
        /// <summary>
        /// Implements logic to receive messages from <see cref="Logger"/>
        /// </summary>
        /// <param name="senderId">Source <see cref="Grid"/> Id</param>
        /// <param name="message"><see cref="LoggerMessage"/> message</param>
        void ReceiveMessage(string senderId, LoggerMessage message);
    }
}
