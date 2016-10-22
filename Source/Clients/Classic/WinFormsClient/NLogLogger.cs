using System;
using Andead.Chat.Common.Logging;

namespace Andead.Chat.Client.WinForms
{
    public class NLogLogger : ILogger
    {
        private readonly NLog.ILogger _logger;

        public NLogLogger(NLog.ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        public void Info(string message, InfoCategory category)
        {
            _logger.Info($"{category}: {message}");
        }

        public void Warn(string message, WarnCategory category)
        {
            _logger.Warn($"{category}: {message}");
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }
    }
}