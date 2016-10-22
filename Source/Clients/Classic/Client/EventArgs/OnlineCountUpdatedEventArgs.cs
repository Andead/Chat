using System;

namespace Andead.Chat.Client
{
    public class OnlineCountUpdatedEventArgs : EventArgs
    {
        public OnlineCountUpdatedEventArgs(int? value)
        {
            Value = value;
        }

        public int? Value { get; }
    }
}