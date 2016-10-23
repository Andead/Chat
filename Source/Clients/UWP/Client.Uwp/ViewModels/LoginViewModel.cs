using System;
using System.Windows.Input;
using Andead.Chat.Common.Utilities;
using GalaSoft.MvvmLight.Command;

namespace Andead.Chat.Client.Uwp
{
    public class LoginViewModel : ViewModel
    {
        private readonly ConnectionConfiguration _connectionConfiguration;
        private readonly IServiceClientFactory _serviceClientFactory;
        private bool _busy;
        private IServiceClient _client;
        private string _name;
        private bool _signInEnabled;

        public LoginViewModel(IServiceClientFactory serviceClientFactory,
            ConnectionConfiguration connectionConfiguration)
        {
            serviceClientFactory.IsNotNull(nameof(serviceClientFactory));
            connectionConfiguration.IsNotNull(nameof(connectionConfiguration));
            _serviceClientFactory = serviceClientFactory;
            _connectionConfiguration = connectionConfiguration;

            CreateCommands();
        }

        public bool SignInEnabled
        {
            get { return _signInEnabled; }
            private set { Set(ref _signInEnabled, value); }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                Set(ref _name, value);

                UpdateSignInEnabled();
            }
        }

        public bool Busy
        {
            get { return _busy; }
            private set
            {
                Set(ref _busy, value);

                UpdateSignInEnabled();
            }
        }

        public ICommand SignInCommand { get; private set; }

        private void UpdateSignInEnabled()
        {
            SignInEnabled = !Busy && !string.IsNullOrWhiteSpace(Name);
        }

        public override void Load()
        {
            IServiceClient asyncServiceClient = _serviceClientFactory.GetServiceClient();
            _client = asyncServiceClient ?? _serviceClientFactory.GetServiceClient();

            _client.Connect(_connectionConfiguration);

            base.Load();
        }

        private void CreateCommands()
        {
            SignInCommand = new RelayCommand(ExecuteSignIn, () => SignInEnabled);
        }

        private async void ExecuteSignIn()
        {
            Busy = true;
            SignInResult result = await _client.SignInAsync(Name);

            Busy = false;

            ChatViewModel chatViewModel = result.Success
                ? new ChatViewModel(_client)
                : null;

            OnSignIn(new SignInEventArgs(result, chatViewModel));
        }

        public event EventHandler<SignInEventArgs> SignIn;

        protected virtual void OnSignIn(SignInEventArgs e)
        {
            SignIn?.Invoke(this, e);
        }
    }
}