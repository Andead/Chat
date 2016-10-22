using System.Windows;
using Andead.Chat.Client;
using Andead.Chat.Clients.Wpf.Interfaces;
using Andead.Chat.Clients.Wpf.Properties;
using Andead.Chat.Common.Utilities;

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
            viewFactory.IsNotNull(nameof(viewFactory));

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
            if (!args.Result.Success)
            {
                ShowError(args.Result.Message);
                return;
            }

            if (args.ChatViewModel != null)
            {
                var chatWindow = new ClientWindow
                {
                    DataContext = args.ChatViewModel,
                    ContentControl = { Content = _viewFactory.GetView(args.ChatViewModel) },
                    MinWidth = 300,
                    MinHeight = 150,
                    Width = 600,
                    Height = 400,
                    ResizeMode = ResizeMode.CanResize,
                    ShowMaxRestoreButton = true,
                    Title = args.ChatViewModel.Title
                };

                chatWindow.Closed += new OneTimeEventHandler(() =>
                {
                    Application.Current.MainWindow.Show();
                    ((LoginViewModel)DataContext).Reload();
                });

                Application.Current.MainWindow.Hide();
                chatWindow.Show();
            }
        }
    }
}