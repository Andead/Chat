using System;

namespace Andead.Chat.Client.Uwp
{
    public class OperationFailedException : Exception
    {
        public OperationFailedException(string message) : base(message)
        {
        }
    }
}