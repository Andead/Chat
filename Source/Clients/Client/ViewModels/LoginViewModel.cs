using System;
using System.IO;
using System.Windows.Input;
using Andead.Chat.Common.Utilities;
using GalaSoft.MvvmLight.CommandWpf;

namespace Andead.Chat.Client
{
    public class LoginViewModel : ViewModel
    {
        private readonly ConnectionConfiguration _connectionConfiguration;
        private readonly IServiceClientFactory _serviceClientFactory;
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
                SignInEnabled = !string.IsNullOrWhiteSpace(value);
            }
        }

        public ICommand SignInCommand { get; private set; }

        public override void Load()
        {
            ReloadClient();

            base.Load();
        }

        private void ReloadClient()
        {
            IAsyncServiceClient asyncServiceClient = _serviceClientFactory.GetAsyncServiceClient();
            _client = asyncServiceClient ?? _serviceClientFactory.GetServiceClient();

            _client.Connect(_connectionConfiguration);
        }

        private void CreateCommands()
        {
            SignInCommand = new RelayCommand(ExecuteSignIn, () => SignInEnabled);
        }

        private async void ExecuteSignIn()
        {
            SignInResult result;

            try
            {
                SignInEnabled = false;

                var asyncServiceClient = _client as IAsyncServiceClient;
                if (asyncServiceClient != null)
                {
                    result = await asyncServiceClient.SignInAsync(Name);
                }
                else
                {
                    result = _client.SignIn(Name);
                }
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(e));
                ReloadClient();
                return;
            }
            finally
            {
                SignInEnabled = true;
            }

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