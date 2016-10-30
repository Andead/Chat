using System;

namespace Andead.Chat.Client
{
    public class SendMessageEventArgs : EventArgs
    {
        public SendMessageEventArgs(SendMessageResult result)
        {
            Result = result;
        }

        public SendMessageResult Result { get; }
    }
}