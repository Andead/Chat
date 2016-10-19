namespace Andead.Chat.Client
{
    public sealed class MessageReceivedEventArgs : System.EventArgs
    {
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}