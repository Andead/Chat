using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Andead.Chat.Common.Utilities;
using GalaSoft.MvvmLight.CommandWpf;

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
        private string _title;

        public ChatViewModel(IServiceClient client)
        {
            client.IsNotNull(nameof(client));

            _client = client;
            _client.OnlineCountUpdated += ClientOnOnlineCountUpdated;
            _client.MessageReceived += ClientOnMessageReceived;

            CreateCommands();
        }

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public int? OnlineCount
        {
            get { return _onlineCount; }
            set
            {
                if (Set(ref _onlineCount, value))
                {
                    UpdateTitle();
                    Task.Run(() => UpdateOnlineNames());
                }
            }
        }

        private void UpdateTitle()
        {
            Title = OnlineCount.HasValue
                ? $"{ServerName} ({OnlineCount} users)"
                : $"{ServerName}";
        }

        public string Title
        {
            get { return _title; }
            private set { Set(ref _title, value); }
        }

        public string ServerName
        {
            get { return _serverName; }
            private set
            {
                Set(ref _serverName, value);
                UpdateTitle();
            }
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

            _client.Disconnect();
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

        public async void UpdateOnlineNames()
        {
            ReadOnlyCollection<string> names;

            var asyncServiceClient = _client as IAsyncServiceClient;
            if (asyncServiceClient != null)
            {
                names = await asyncServiceClient.GetNamesOnlineAsync();
            }
            else
            {
                names = _client.GetNamesOnline();
            }

            OnlineNames = names;
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

                if (!result.Success)
                {
                    OnError(result.Message);
                    return;
                }

                Message = null;
            }
            catch (Exception e)
            {
                OnError(new ErrorEventArgs(e));
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