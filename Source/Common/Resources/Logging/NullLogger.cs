namespace Andead.Chat.Common.Logging
{
    public class NullLogger : ILogger
    {
        public void Info(string message, InfoCategory category)
        {
        }

        public void Warn(string message, WarnCategory category)
        {
        }

        public void Trace(string message)
        {
        }

        public void Fatal(string message)
        {
        }
    }
}