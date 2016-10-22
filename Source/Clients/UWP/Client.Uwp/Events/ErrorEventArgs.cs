using System;

namespace Andead.Chat.Client.Uwp
{
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception _exception;

        public ErrorEventArgs(Exception exception)
        {
            _exception = exception;
        }

        public Exception GetException()
        {
            return _exception;
        }
    }
}