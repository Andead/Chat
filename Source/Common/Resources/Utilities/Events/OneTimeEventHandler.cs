using System;

namespace Andead.Chat.Common.Utilities
{
    public class OneTimeEventHandler
    {
        private EventHandler _eventHandler;

        public OneTimeEventHandler(EventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public OneTimeEventHandler(Action eventHandler)
        {
            _eventHandler = (sender, args) => eventHandler();
        }

        public OneTimeEventHandler(Action<EventArgs> eventHandler)
        {
            _eventHandler = (sender, args) => eventHandler(args);
        }

        private void Invoke(object sender, EventArgs eventArgs)
        {
            if (_eventHandler != null)
            {
                _eventHandler(sender, eventArgs);
                _eventHandler = null;
            }
        }

        public static implicit operator EventHandler(OneTimeEventHandler oneTimeEventHandler)
        {
            return oneTimeEventHandler.Invoke;
        }
    }

    public class OnceEventHandler<TEventArgs>
    {
        private EventHandler<TEventArgs> _eventHandler;

        public OnceEventHandler(EventHandler<TEventArgs> eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public OnceEventHandler(Action eventHandler)
        {
            _eventHandler = (sender, args) => eventHandler();
        }

        public OnceEventHandler(Action<TEventArgs> eventHandler)
        {
            _eventHandler = (sender, args) => eventHandler(args);
        }

        private void Invoke(object sender, TEventArgs eventArgs)
        {
            if (_eventHandler != null)
            {
                _eventHandler(sender, eventArgs);
                _eventHandler = null;
            }
        }

        public static implicit operator EventHandler<TEventArgs>(OnceEventHandler<TEventArgs> onceEventHandler)
        {
            return onceEventHandler.Invoke;
        }
    }
}