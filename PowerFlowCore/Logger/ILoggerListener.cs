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
        /// <param name="snderId">Source <see cref="Grid"/> Id</param>
        /// <param name="message"><see cref="Logger"/> message</param>
        void ReceiveLoggerMessage(string snderId, string message);
    }
}
