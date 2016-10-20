using System;
using Andead.Chat.Common.Utilities;
using Moq;
using NUnit.Framework;

namespace Common.Tests
{
    [TestFixture]
    public class OneTimeEventHandlerTests
    {
        public class TestTarget
        {
            internal event EventHandler Test;

            public void Raise()
            {
                Test?.Invoke(this, EventArgs.Empty);
            }
        }

        public class TestTarget<TEventArgs> where TEventArgs
            : EventArgs
        {
            private readonly TEventArgs _eventArgs;

            public TestTarget(TEventArgs eventArgs)
            {
                _eventArgs = eventArgs;
            }

            public event EventHandler<TEventArgs> Test;

            public void Raise()
            {
                Test?.Invoke(this, _eventArgs);
            }
        }

        public class TestSubscriber
        {
            public virtual void OnTargetEvent(object sender, EventArgs eventArgs)
            {
            }

            public virtual void OnTargetEvent<TEventArgs>(object sender, TEventArgs eventArgs)
            {
            }
        }

        [Test]
        public void OneTimeEventHandler_ExecutesOnce()
        {
            var target = new TestTarget();

            var subscriber = new Mock<TestSubscriber>();
            target.Test += new OneTimeEventHandler(subscriber.Object.OnTargetEvent);

            target.Raise();
            target.Raise();

            subscriber.Verify(s => s.OnTargetEvent(target, It.IsAny<EventArgs>()), Times.Once);
        }

        [Test]
        public void OneTimeEventHandlerGeneric_ExecutesOnce()
        {
            var eventArgs = new UnhandledExceptionEventArgs(null, false);
            var target = new TestTarget<UnhandledExceptionEventArgs>(eventArgs);

            var subscriber = new Mock<TestSubscriber>();
            target.Test += new OneTimeEventHandler<UnhandledExceptionEventArgs>(subscriber.Object.OnTargetEvent);

            target.Raise();
            target.Raise();

            subscriber.Verify(s => s.OnTargetEvent(target, eventArgs), Times.Once);
        }
    }
}