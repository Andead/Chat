using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Andead.Chat.Client.Uwp
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public virtual event ErrorEventHandler Error;

        protected virtual void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        protected virtual void OnError(string message)
        {
            Error?.Invoke(this, new ErrorEventArgs(new OperationFailedException(message)));
        }

        public virtual void Load()
        {
        }

        public virtual void Unload()
        {
        }

        public void Reload()
        {
            Unload();
            Load();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T field, T value = default(T), [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}