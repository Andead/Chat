namespace Andead.Chat.Client
{
    public class SignInResult
    {
        public SignInResult(bool success, string message, int onlineCount)
        {
            Success = success;
            Message = message;
            OnlineCount = onlineCount;
        }

        public bool Success { get; }

        public string Message { get; }

        public int OnlineCount { get; }
    }
}