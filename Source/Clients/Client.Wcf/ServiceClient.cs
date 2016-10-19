using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Andead.Chat.Client.Wcf.ChatService;

namespace Andead.Chat.Client.Wcf
{
    public class ServiceClient : IAsyncServiceClient, IChatServiceCallback, IDisposable
    {
        private TimeSpan _timeout;

        /// <summary>
        ///     This property is marked virtual and public for testing purposes only.
        /// </summary>
        public virtual IChatService Service { get; private set; }

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

            switch (configuration.Protocol)
            {
                case "net.tcp":
                    Service = DuplexChannelFactory<IChatService>.CreateChannel(
                        new InstanceContext(this),
                        new NetTcpBinding(SecurityMode.None),
                        new EndpointAddress($"net.tcp://{configuration.ServerName}:{configuration.Port}/Service.svc"));
                    break;
                case "http":
                    Service = DuplexChannelFactory<IChatService>.CreateChannel(
                        new InstanceContext(this),
                        new WSDualHttpBinding(WSDualHttpSecurityMode.None),
                        new EndpointAddress($"http://{configuration.ServerName}:{configuration.Port}/Service.svc"));
                    break;
                default:
                    throw new NotSupportedException("Supported protocols are only net.tcp and http.");
            }

            ServerName = configuration.ServerName;
            _timeout = TimeSpan.FromMilliseconds(configuration.TimeOut);
        }

        public void Disconnect()
        {
            ((ICommunicationObject) Service).Close(_timeout);
        }

        public SignInResult SignIn(string name)
        {
            var request = new SignInRequest {Name = name};

            SignInResponse response = Service.SignIn(request);

            SignedIn = response.Success;

            if (response.Success)
            {
                OnOnlineCountUpdated(response.OnlineCount);
                OnMessageReceived(response.Message);
            }

            return new SignInResult(response.Success, response.Message, response.OnlineCount);
        }

        public async Task<SignInResult> SignInAsync(string name)
        {
            var request = new SignInRequest {Name = name};

            SignInResponse response = await Service.SignInAsync(request);

            SignedIn = response.Success;

            return new SignInResult(response.Success, response.Message, response.OnlineCount);
        }

        public void SignOut()
        {
            Service.SignOut();

            SignedIn = false;
        }

        public async Task SignOutAsync()
        {
            await Service.SignOutAsync();

            SignedIn = false;
        }

        public SendMessageResult Send(string message)
        {
            var request = new SendMessageRequest {Message = message};

            SendMessageResponse response = Service.SendMessage(request);

            var result = new SendMessageResult {Message = response.Message, Success = response.Success};

            return result;
        }

        public async Task<SendMessageResult> SendAsync(string message)
        {
            var request = new SendMessageRequest {Message = message};

            SendMessageResponse response = await Service.SendMessageAsync(request);

            var result = new SendMessageResult {Message = response.Message, Success = response.Success};

            return result;
        }

        public string[] GetNamesOnline()
        {
            return Service.GetNamesOnline();
        }

        public async Task<string[]> GetNamesOnlineAsync()
        {
            return await Service.GetNamesOnlineAsync();
        }

        void IChatServiceCallback.ReceiveMessage(string message)
        {
            OnMessageReceived(message);
        }

        private void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        void IChatServiceCallback.UpdateOnlineCount(int value)
        {
            OnOnlineCountUpdated(value);
        }

        private void OnOnlineCountUpdated(int value)
        {
            OnlineCountUpdated?.Invoke(this, new OnlineCountUpdatedEventArgs(value));
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}