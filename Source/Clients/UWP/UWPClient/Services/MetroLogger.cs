using Andead.Chat.Common.Logging;
using MetroLog;
using MetroLog.Layouts;
using MetroLog.Targets;
using ILogger = Andead.Chat.Common.Logging.ILogger;

namespace Andead.Chat.Client.Uwp.Services
{
    public class MetroLogger : ILogger
    {
        private readonly MetroLog.ILogger _logger;

        public MetroLogger()
        {
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new DebugTarget());
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Warn, LogLevel.Fatal,
                new StreamingFileTarget(new SingleLineLayout()));

            _logger = LogManagerFactory.DefaultLogManager.GetLogger("Default");
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