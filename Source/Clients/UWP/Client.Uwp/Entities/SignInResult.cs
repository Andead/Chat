namespace Andead.Chat.Client.Uwp
{
    public class SignInResult
    {
        public SignInResult(bool success)
        {
            Success = success;
        }

        public SignInResult(bool success, string message, int onlineCount)
            : this(success)
        {
            Message = message;
            OnlineCount = onlineCount;
        }

        public bool Success { get; }

        public string Message { get; }

        public int OnlineCount { get; }
    }
}