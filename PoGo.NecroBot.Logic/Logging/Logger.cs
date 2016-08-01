#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using PoGo.NecroBot.Logic.State;

#endregion

namespace PoGo.NecroBot.Logic.Logging
{
    public static class Logger
    {
        private static ILogger _logger;
        private static string _path;
        private static DateTime _lastLogTime;
        private static readonly IList<string> LogbufferList = new List<string>();

        private static void Log(string message, bool force = false)
        {
            lock (LogbufferList)
            {
                if (_lastLogTime.AddSeconds(60) < DateTime.Now && !force)
                {
                    LogbufferList.Add(message);
                    return;
                }
                using (
                    var log =
                        File.AppendText(Path.Combine(_path,
                            $"NecroBot-{DateTime.Today.ToString("yyyy-MM-dd")}-{DateTime.Now.ToString("HH")}.txt"))
                    )
                {
                    foreach (var line in LogbufferList)
                    {
                        log.WriteLine(line);
                    }
                    _lastLogTime = DateTime.Now;
                    log.Flush();
                }
            }
        }

        /// <summary>
        ///     Set the logger. All future requests to <see cref="Write(string,LogLevel,ConsoleColor)" /> will use that logger, any
        ///     old will be
        ///     unset.
        /// </summary>
        /// <param name="logger"></param>
        public static void SetLogger(ILogger logger, string subPath = "")
        {
            _logger = logger;
            _path = Path.Combine(Directory.GetCurrentDirectory(), subPath, "Logs");
            Directory.CreateDirectory(_path);
            Log($"Initializing Rocket logger at time {DateTime.Now}...");
        }

        /// <summary>
        ///     Sets Context for the logger
        /// </summary>
        /// <param name="session">Context</param>
        public static void SetLoggerContext(ISession session)
        {
            _logger?.SetSession(session);
        }

        /// <summary>
        ///     Log a specific message to the logger setup by <see cref="SetLogger(ILogger)" /> .
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">Optional level to log. Default <see cref="LogLevel.Info" />.</param>
        /// <param name="color">Optional. Default is automatic color.</param>
        public static void Write(string message, LogLevel level = LogLevel.Info, ConsoleColor color = ConsoleColor.Black, bool force = false)
        {
            if (_logger == null)
                return;
            _logger.Write(message, level, color);
            Log(string.Concat($"[{DateTime.Now.ToString("HH:mm:ss")}] ", message, force));
        }
    }

    public enum LogLevel
    {
        Self = -1,
        None = 0,
        Error = 1,
        Warning = 2,
        Pokestop = 3,
        Farming = 4,
        Sniper = 5,
        Recycling = 6,
        Berry = 7,
        Caught = 8,
        Transfer = 9,
        Evolve = 10,
        Egg = 11,
        Update = 12,
        Info = 13,
        Debug = 14
    }
}