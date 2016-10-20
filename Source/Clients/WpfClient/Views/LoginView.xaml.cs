using System;
using System.Windows;
using Andead.Chat.Client;
using Andead.Chat.Clients.Wpf.Interfaces;
using Andead.Chat.Clients.Wpf.Properties;

namespace Andead.Chat.Clients.Wpf
{
    /// <summary>
    ///     Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView
    {
        private readonly IViewFactory _viewFactory;

        public LoginView()
        {
            InitializeComponent();
        }

        public LoginView(LoginViewModel loginViewModel, IViewFactory viewFactory)
            : base(loginViewModel)
        {
            if (loginViewModel == null)
            {
                throw new ArgumentNullException(nameof(loginViewModel));
            }
            if (viewFactory == null)
            {
                throw new ArgumentNullException(nameof(viewFactory));
            }

            _viewFactory = viewFactory;

            InitializeComponent();
        }

        protected override void Load(ViewModel viewModel)
        {
            base.Load(viewModel);

            var loginViewModel = viewModel as LoginViewModel;
            if (loginViewModel == null)
            {
                return;
            }

            loginViewModel.Name = Settings.Default.Username;
            loginViewModel.SignIn += LoginViewModelOnSignIn;
        }

        protected override void Unload(ViewModel viewModel)
        {
            var loginViewModel = viewModel as LoginViewModel;
            if (loginViewModel != null)
            {
                loginViewModel.SignIn -= LoginViewModelOnSignIn;
            }

            base.Unload(viewModel);
        }

        private void LoginViewModelOnSignIn(object sender, SignInEventArgs args)
        {
            if (args.ChatViewModel != null)
            {
                ClientWindow chatWindow = ClientWindow.Create("Chat", args.ChatViewModel, 600, 400);
                chatWindow.ShowMaxRestoreButton = true;
                chatWindow.ContentControl.Content = _viewFactory.GetView(args.ChatViewModel);
                chatWindow.Title = args.ChatViewModel.Title;
                chatWindow.Show();

                Application.Current.MainWindow.Hide();
                chatWindow.Closed += (s, e) => Application.Current.MainWindow.Show();
            }
        }
    }
}