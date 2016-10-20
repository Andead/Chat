using System.ComponentModel;
using Andead.Chat.Client;

namespace Andead.Chat.Clients.Wpf
{
    /// <summary>
    ///     Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class ClientWindow
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        public static ClientWindow Create(string title, ViewModel viewModel, int minWidth = 300, int minHeight = 80)
        {
            var window = new ClientWindow
            {
                Title = title,
                DataContext = viewModel,
                MinWidth = minWidth,
                MinHeight = minHeight
            };

            window.Closing += WindowOnClosing;

            return window;
        }

        private static void WindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            (((ClientWindow)sender).DataContext as ViewModel)?.Unload();
        }
    }
}