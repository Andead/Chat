using System;
using System.Windows.Input;
using Andead.Chat.Common.Utilities;
using GalaSoft.MvvmLight.Command;

namespace Andead.Chat.Client.Uwp
{
    public class LoginViewModel : ViewModel
    {
        private readonly IServiceClientFactory _serviceClientFactory;
        private bool _busy;
        private IServiceClient _client;
        private string _name = "Guest";
        private bool _enterEnabled = true;
        private string _serverName = "localhost";

        public LoginViewModel(IServiceClientFactory serviceClientFactory)
        {
            serviceClientFactory.IsNotNull(nameof(serviceClientFactory));
            _serviceClientFactory = serviceClientFactory;

            CreateCommands();
        }

        public bool EnterEnabled
        {
            get { return _enterEnabled; }
            private set { Set(ref _enterEnabled, value); }
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

        public ICommand EnterCommand { get; private set; }

        public string ServerName
        {
            get { return _serverName; }
            set { Set(ref _serverName, value); }
        }

        private void UpdateSignInEnabled()
        {
            EnterEnabled = !Busy && !string.IsNullOrWhiteSpace(Name);
        }

        private void CreateCommands()
        {
            EnterCommand = new RelayCommand(Enter);
        }

        private async void Enter()
        {
            Busy = true;

            _client = _serviceClientFactory.GetServiceClient();

            var configuration = new ConnectionConfiguration {ServerName = ServerName};
            _client.Connect(configuration);

            SignInResult result = await _client.SignInAsync(Name);

            Busy = false;

            ChatViewModel chatViewModel = result.Success
                ? new ChatViewModel(_client)
                : null;

            OnEntered(new SignInEventArgs(result, chatViewModel));
        }

        public event EventHandler<SignInEventArgs> Entered;

        protected virtual void OnEntered(SignInEventArgs e)
        {
            Entered?.Invoke(this, e);
        }
    }
}