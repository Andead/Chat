using System;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace Andead.Chat.Client
{
    public class LoginViewModel : ViewModel
    {
        private readonly IServiceClient _client;
        private string _name;
        private bool _signInEnabled;

        public LoginViewModel(IAsyncServiceClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;

            CreateCommands();
        }

        public LoginViewModel(IServiceClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;
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

        private void CreateCommands()
        {
            SignInCommand = new RelayCommand(ExecuteSignIn, () => SignInEnabled);
        }

        private async void ExecuteSignIn()
        {
            try
            {
                SignInEnabled = false;
                SignInResult result;

                var asyncServiceClient = _client as IAsyncServiceClient;
                if (asyncServiceClient != null)
                {
                    result = await asyncServiceClient.SignInAsync(Name);
                }
                else
                {
                    result = _client.SignIn(Name);
                }

                ChatViewModel chatViewModel = result.Success
                    ? new ChatViewModel(_client)
                    : null;

                OnSignIn(new SignInEventArgs(result, chatViewModel));
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(e));
            }
            finally
            {
                SignInEnabled = true;
            }
        }

        public event EventHandler<SignInEventArgs> SignIn;

        protected virtual void OnSignIn(SignInEventArgs e)
        {
            SignIn?.Invoke(this, e);
        }
    }
}