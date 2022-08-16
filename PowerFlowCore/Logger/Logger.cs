using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Diagnostics;

using PowerFlowCore.Data;
using System.Threading.Tasks;

namespace PowerFlowCore
{
    /// <summary>
    /// Logger class. Contains methods and event for logging.
    /// </summary>
    public static class Logger
    {
        #region [Private/Internal interface]

        internal static readonly object _lock = new object();

        /// <summary>
        /// Logger status (1 - in work, 0 - out of work)
        /// </summary>
        private static int inService = 1;

        /// <summary>
        /// Modes collection
        /// </summary>
        private static HashSet<LogMode> Modes = new HashSet<LogMode>();

        /// <summary>
        /// Collection of custom logger listeners
        /// </summary>
        private static List<ILoggerListener> listeners = new List<ILoggerListener>();

        #endregion

        #region [Public interface]

        /// <summary>
        /// Broadcast <see cref="Logger"/> messages to receivers.
        /// <para>Sender Id (<see cref="String"/>) -> <see cref="String.Empty"/> OR source <see cref="Grid.Id"/></para>
        /// <para>Message content (<see cref="LoggerMessage"/>)</para>
        /// </summary>
        public static event LogInfoEvent? LogBroadcast;

        #endregion

        #region [Enable/Disable Log]

        /// <summary>
        /// Set <see cref="Logger"/> to work status 
        /// </summary>
        public static void EnableLog() => Interlocked.Exchange(ref inService, 1);

        /// <summary>
        /// Disable <see cref="Logger"/>. Broadcasting continues with event.
        /// </summary>
        public static void DisableLog() => Interlocked.Exchange(ref inService, 0);

        /// <summary>
        /// Check <see cref="Logger"/> work status
        /// </summary>
        public static bool IsEnabled() => inService == 1;

        #endregion

        #region [Logger Modes]

        /// <summary>
        /// Add presented modes to <see cref="Logger"/>
        /// </summary>
        /// <param name="modes">Modes list</param>
        public static void AddModes(params LogMode[] modes)
        {
            if (modes == null || modes.Length == 0)
                return;

            foreach (var item in modes)
                Modes.Add(item);
        }

        /// <summary>
        /// Clear <see cref="Logger"/> modes list
        /// </summary>
        public static void ClearModes()
        {
            Modes.Clear();
            if (listeners.Count > 0)
                listeners.Clear();
        }

        /// <summary>
        /// Add Console to <see cref="Logger"/> output
        /// </summary>
        public static void AddConsoleMode() => Modes.Add(LogMode.Console);

        /// <summary>
        /// Add Debug to <see cref="Logger"/> output
        /// </summary>
        public static void AddDebugMode()   => Modes.Add(LogMode.Debug);

        /// <summary>
        /// Add custom listener to <see cref="Logger"/> output
        /// </summary>
        public static void AddCustomMode(ILoggerListener customListener)
        {
            if(customListener != null)
            {
                Modes.Add(LogMode.Custom);
                listeners.Add(customListener);
            }
        }

        #endregion

        #region [Logger speakers]

        /// <summary>
        /// Internal log method. Prepare and show message, invoke <see cref="LogBroadcast"/> event.
        /// </summary>
        /// <param name="level">Message status</param>
        /// <param name="message">Message to log</param>
        private static void Log(LogLevel level, string message, string sourceGridId = "")
        {
            var delim = "||";   // Set delimiter
            var time = $"{DateTime.UtcNow.ToLocalTime():dd.MM.yy HH:mm:ss.fff}";    // Add timestamp

            var mesBuilder = new StringBuilder();

            mesBuilder.Append(time);
            mesBuilder.Append(delim);
            mesBuilder.Append(level.ToString());
            mesBuilder.Append(delim);
            mesBuilder.Append(message);

            // Output message
            string outputStr = mesBuilder.ToString();

            // Output LoggerMessage
            LoggerMessage output = new LoggerMessage(dateTimeStamp: time, level: level, message: message, source: sourceGridId);

            // Invoke logger event
            LogBroadcast?.Invoke(sourceGridId, output);

            // Logger is enabled
            if (inService == 1)
            {
                if (Modes.Count > 0)
                {
                    lock (_lock)
                    {
                        #region [Console output]                    
                        if (Modes.Contains(LogMode.Console))
                        {
                            lock (Console.Out)
                            {
                                var color = Console.ForegroundColor;
                                switch (level)
                                {
                                    case LogLevel.Success:
                                        color = ConsoleColor.Green;
                                        break;
                                    case LogLevel.Warning:
                                        color = ConsoleColor.Yellow;
                                        break;
                                    case LogLevel.Critical:
                                        color = ConsoleColor.Red;
                                        break;
                                    default:
                                        break;
                                }
                                Console.ForegroundColor = color;
                                Console.WriteLine(outputStr);
                                Console.ResetColor();
                            }
                        }
                        #endregion

                        #region [Debug output]
                        if (Modes.Contains(LogMode.Debug))
                            Debug.WriteLine(outputStr);
                        #endregion

                        #region [Custom output]
                        if (Modes.Contains(LogMode.Custom) && listeners.Count > 0)
                            Task.Run(() =>
                            {
                                try
                                {
                                    listeners.ForEach(l => l.ReceiveMessage(sourceGridId, output));
                                }
                                catch (Exception) { }
                            });
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// Log message with Information status. Invoke <see cref="LogBroadcast"/> event.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogInfo(string message, string sourceGridGuid = "") => Log(LogLevel.Info, message, sourceGridGuid);

        /// <summary>
        /// Log message with Success status. Invoke <see cref="LogBroadcast"/> event.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogSuccess(string message, string sourceGridGuid = "") => Log(LogLevel.Success, message, sourceGridGuid);

        /// <summary>
        /// Log message with Warning status. Invoke <see cref="LogBroadcast"/> event.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogWarning(string message, string sourceGridGuid = "") => Log(LogLevel.Warning, message, sourceGridGuid);

        /// <summary>
        /// Log message with Critical status. Invoke <see cref="LogBroadcast"/> event.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogCritical(string message, string sourceGridGuid = "") => Log(LogLevel.Critical, message, sourceGridGuid);

        #endregion
    }
}
