using System;

namespace Andead.Chat.Client.Uwp
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