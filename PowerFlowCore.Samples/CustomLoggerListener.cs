using System;
using System.Diagnostics;

namespace PowerFlowCore.Samples
{
    partial class Program
    {
        /// <summary>
        /// Test class for ILoggerListener
        /// </summary>
        public class CustomLoggerListener : ILoggerListener
        {
            public void ReceiveMessage(string senderId, LoggerMessage message)
            {
                Debug.WriteLine($"FROM CUSTOM LISTENER: ({senderId}) " + message.Message);
            }
        }
    }
}
