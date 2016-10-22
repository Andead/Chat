using System.Windows.Input;

namespace Andead.Chat.Client.Uwp
{
    public static class CommandExtensions
    {
        public static void TryExecute(this ICommand command, object parameter = null)
        {
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}