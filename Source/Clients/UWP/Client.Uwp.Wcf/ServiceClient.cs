using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using Andead.Chat.Client.Uwp.Wcf.ChatService;
using Andead.Chat.Common.Policy;
using Andead.Chat.Common.Utilities;

namespace Andead.Chat.Client.Uwp.Wcf
{
    public class ServiceClient : IServiceClient, IChatServiceCallback, IDisposable
    {
        private const string PerformingSignInDescription = "performing sign in";
        private const string PerformingSignOutDescription = "performing sign-out";
        private const string SendingAMessageDescription = "sending a message";
        private const string GettingNamesOnlineDescription = "getting names online";
        private const string DisconnectingDescription = "disconnecting";

        private ConnectionConfiguration _lastConfiguration;
        private string _lastName;

        private TimeSpan _timeout;

        /// <summary>
        ///     This property is marked virtual and public for testing purposes only.
        /// </summary>
        public virtual IChatService Service { get; private set; }

        void IChatServiceCallback.ReceiveMessage(string message)
        {
            OnMessageReceived(message);
        }

        void IChatServiceCallback.UpdateOnlineCount(int value)
        {
            OnOnlineCountUpdated(value);
        }

        public void Dispose()
        {
            Disconnect();
        }

        public bool SignedIn { get; private set; }

        public virtual event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<OnlineCountUpdatedEventArgs> OnlineCountUpdated;

        public string ServerName { get; private set; }

        public void Connect(ConnectionConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Remember last config
            _lastConfiguration = configuration;

            DuplexChannelFactory<IChatService> factory;
            switch (configuration.Protocol)
            {
                case "net.tcp":
                    factory = new DuplexChannelFactory<IChatService>(new InstanceContext(this),
                        new NetTcpBinding(SecurityMode.None),
                        new EndpointAddress($"net.tcp://{configuration.ServerName}:{configuration.Port}/Service.svc"));
                    break;
                default:
                    throw new NotSupportedException("Supported protocol is only net.tcp.");
            }

            Service = factory.CreateChannel();

            ServerName = configuration.ServerName;
            _timeout = TimeSpan.FromMilliseconds(configuration.TimeOut);
        }

        public void Disconnect()
        {
            Handle.Errors(() => ((ICommunicationObject) Service).Close(_timeout), DisconnectingDescription);
        }

        public SignInResult SignIn(string name)
        {
            Task<SignInResult> task = SignInAsync(name);
            task.RunSynchronously();
            return task.Result;
        }

        public async Task<SignInResult> SignInAsync(string name)
        {
            var request = new SignInRequest {Name = name};

            // Remember last name
            _lastName = name;

            SignInResponse response =
                await Handle.ErrorsAsync(async () => await Service.SignInAsync(request),
                    Limits.ReconnectTimes, PerformingSignInDescription, e => Connect(e));

            return ProcessSignInResponse(response);
        }

        public void SignOut()
        {
            Task task = SignOutAsync();
            task.RunSynchronously();
        }

        public async Task SignOutAsync()
        {
            await Handle.ErrorsAsync(async () => await Service.SignOutAsync(), PerformingSignOutDescription);

            SignedIn = false;
        }

        public SendMessageResult Send(string message)
        {
            Task<SendMessageResult> task = SendAsync(message);
            task.RunSynchronously();
            return task.Result;
        }

        public async Task<SendMessageResult> SendAsync(string message)
        {
            var request = new SendMessageRequest {Message = message};

            SendMessageResponse response = await Handle.ErrorsAsync(async () => await Service.SendMessageAsync(request),
                SendingAMessageDescription, ConnectAndSignInAsync);

            return ProcessSendMessageResult(response);
        }

        public ObservableCollection<string> GetNamesOnline()
        {
            Task<ObservableCollection<string>> task = GetNamesOnlineAsync();
            task.RunSynchronously();
            return task.Result;
        }

        public async Task<ObservableCollection<string>> GetNamesOnlineAsync()
        {
            return await Handle.ErrorsAsync(() => Service.GetNamesOnlineAsync(), GettingNamesOnlineDescription);
        }

        private SignInResult ProcessSignInResponse(SignInResponse response)
        {
            if (response == null)
            {
                SignedIn = false;
                return new SignInResult(false, "Failed to sign in. See log for details.");
            }

            SignedIn = response.Success;
            if (response.Success)
            {
                OnOnlineCountUpdated(response.OnlineCount);
                OnMessageReceived(response.Message);
            }

            return new SignInResult(response.Success, response.Message, response.OnlineCount);
        }

        private void Connect(Exception exception)
        {
            if (exception is CommunicationException)
            {
                Connect(_lastConfiguration);
            }
        }

        private static SendMessageResult ProcessSendMessageResult(SendMessageResponse response)
        {
            if (response == null)
            {
                return new SendMessageResult
                {
                    Message = "Failed to send message to the server. See log for details.",
                    Success = false
                };
            }

            return new SendMessageResult {Message = response.Message, Success = response.Success};
        }

        private async Task ConnectAndSignInAsync(Exception e)
        {
            if (e is CommunicationException)
            {
                // Open a new channel
                Connect(_lastConfiguration);
                await SignInAsync(_lastName);
            }
        }

        private void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        private void OnOnlineCountUpdated(int value)
        {
            OnlineCountUpdated?.Invoke(this, new OnlineCountUpdatedEventArgs(value));
        }
    }
}