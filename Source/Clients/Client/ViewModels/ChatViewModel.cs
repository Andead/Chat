using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Andead.Chat.Client
{
    public class ChatViewModel : ViewModel
    {
        private readonly IServiceClient _client;
        private string _message;
        private int? _onlineCount;
        private IReadOnlyCollection<string> _onlineNames;
        private bool _sendEnabled;
        private string _serverName;

        public ChatViewModel(IServiceClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;
            _client.OnlineCountUpdated += ClientOnOnlineCountUpdated;
            _client.MessageReceived += ClientOnMessageReceived;

            CreateCommands();
        }

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public int? OnlineCount
        {
            get { return _onlineCount; }
            set { Set(ref _onlineCount, value); }
        }

        public string ServerName
        {
            get { return _serverName; }
            private set { Set(ref _serverName, value); }
        }

        public IReadOnlyCollection<string> OnlineNames
        {
            get { return _onlineNames; }
            private set { Set(ref _onlineNames, value); }
        }

        public ICommand SendMessageCommand { get; private set; }

        public bool SendEnabled
        {
            get { return _sendEnabled; }
            private set { Set(ref _sendEnabled, value); }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                Set(ref _message, value);

                SendEnabled = !string.IsNullOrWhiteSpace(value);
            }
        }

        public SignInResult SignInResult { get; set; }

        public override async void Unload()
        {
            _client.OnlineCountUpdated -= ClientOnOnlineCountUpdated;
            _client.MessageReceived -= ClientOnMessageReceived;

            var asyncServiceClient = _client as IAsyncServiceClient;
            if (asyncServiceClient != null)
            {
                await asyncServiceClient.SignOutAsync();
            }
            else
            {
                _client.SignOut();
            }
        }

        private void CreateCommands()
        {
            SendMessageCommand = new RelayCommand(ExecuteSendMessage, () => SendEnabled);
        }

        private void ClientOnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Messages.Add(args.Message);
        }

        private void ClientOnOnlineCountUpdated(object sender, OnlineCountUpdatedEventArgs args)
        {
            OnlineCount = args.Value;
        }

        public override void Load()
        {
            base.Load();

            ServerName = _client.ServerName;

            if (SignInResult.Success)
            {
                OnlineCount = SignInResult.OnlineCount;
                Messages.Add(SignInResult.Message);
            }
        }

        public async void UpdateOnlineNames()
        {
            string[] names;

            var asyncServiceClient = _client as IAsyncServiceClient;
            if (asyncServiceClient != null)
            {
                names = await asyncServiceClient.GetNamesOnlineAsync();
            }
            else
            {
                names = _client.GetNamesOnline();
            }

            OnlineNames = new ObservableCollection<string>(names);
        }

        private async void ExecuteSendMessage()
        {
            try
            {
                SendMessageResult result;

                SendEnabled = false;

                var asyncServiceClient = _client as IAsyncServiceClient;
                if (asyncServiceClient != null)
                {
                    result = await asyncServiceClient.SendAsync(Message);
                }
                else
                {
                    result = _client.Send(Message);
                }

                OnSendMessage(new SendMessageEventArgs(result));

                if (result.Success)
                {
                    Message = null;
                }
            }
            finally
            {
                SendEnabled = true;
            }
        }

        public event EventHandler<SendMessageEventArgs> SendMessage;

        protected virtual void OnSendMessage(SendMessageEventArgs e)
        {
            SendMessage?.Invoke(this, e);
        }
    }
}