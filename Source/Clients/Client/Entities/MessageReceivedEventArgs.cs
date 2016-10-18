using System;

namespace Andead.Chat.Client.Entities
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}