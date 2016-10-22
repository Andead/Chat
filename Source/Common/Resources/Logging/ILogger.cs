namespace Andead.Chat.Common.Logging
{
    public interface ILogger
    {
        void Info(string message, InfoCategory category);

        void Warn(string message, WarnCategory category);

        void Trace(string message);

        void Fatal(string message);
    }
}