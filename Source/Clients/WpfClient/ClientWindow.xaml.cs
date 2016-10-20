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

            Closing += OnClosing;
        }

        private static void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            (((ClientWindow) sender).DataContext as ViewModel)?.Unload();
        }
    }
}