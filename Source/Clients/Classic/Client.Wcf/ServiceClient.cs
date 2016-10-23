using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Andead.Chat.Client.Wcf.ChatService;
using Andead.Chat.Common.Policy;
using Andead.Chat.Common.Utilities;

namespace Andead.Chat.Client.Wcf
{
    public class ServiceClient : IAsyncServiceClient, IChatServiceCallback, IDisposable
    {
        private const string PerformingSignInDescription = "performing sign in";
        private const string PerformingSignOutDescription = "performing sign-out";
        private const string SendingAMessageDescription = "sending a message";
        private const string GettingNamesOnlineDescription = "getting names online";

        private ConnectionConfiguration _lastConfiguration;
        private string _lastName;

        private TimeSpan _timeout;

        /// <summary>
        ///     This property is marked virtual and public for testing purposes only.
        /// </summary>
        public virtual IChatService Service { get; private set; }

        public bool SignedIn { get; private set; }

        public virtual event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<OnlineCountUpdatedEventArgs> OnlineCountUpdated;

        public string ServerName { get; private set; }

        public bool UsesSsl { get; private set; }

        public void Connect(ConnectionConfiguration configuration)
        {
            configuration.IsNotNull(nameof(configuration));
            configuration.Protocol.IsIn("net.tcp", "http");

            // Remember last config
            _lastConfiguration = configuration;

            // Search for the end of host name
            string hostname = configuration.ServerName.Split('/').First();
            string path = configuration.ServerName.Substring(hostname.Length);

            var address =
                new EndpointAddress($"{configuration.Protocol}://{hostname}:{configuration.Port}{path}/Service.svc");

            Binding binding;
            switch (configuration.Protocol)
            {
                case "net.tcp":
                    binding = configuration.UseSsl
                        ? new NetTcpBinding(SecurityMode.Transport)
                        {
                            Security =
                            {
                                Mode = SecurityMode.Transport,
                                Transport =
                                {
                                    ClientCredentialType = TcpClientCredentialType.None
                                }
                            }
                        }
                        : new NetTcpBinding(SecurityMode.None);
                    break;
                case "http":
                    binding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
                    break;
                default:
                    throw new NotSupportedException("Supported protocols are only net.tcp and http.");
            }

            Service = DuplexChannelFactory<IChatService>.CreateChannel(new InstanceContext(this), binding, address);

            ServerName = configuration.ServerName;
            UsesSsl = configuration.UseSsl;

            _timeout = TimeSpan.FromMilliseconds(configuration.TimeOut);
        }

        public void Disconnect()
        {
            Handle.Errors(() => ((ICommunicationObject) Service).Close(_timeout), "disconnecting");
        }

        public SignInResult SignIn(string name)
        {
            SignInRequest request = CreateSignInRequest(name);

            SignInResponse response = Handle.Errors(() => Service.SignIn(request),
                Limits.ReconnectTimes, PerformingSignInDescription, Connect);

            return ProcessSignInResponse(response);
        }

        public async Task<SignInResult> SignInAsync(string name)
        {
            SignInRequest request = CreateSignInRequest(name);

            SignInResponse response = await Handle.ErrorsAsync(async () => await Service.SignInAsync(request),
                Limits.ReconnectTimes, PerformingSignInDescription, e => Connect(e));

            return ProcessSignInResponse(response);
        }

        public void SignOut()
        {
            Handle.Errors(() => Service.SignOut(), PerformingSignOutDescription);

            SignedIn = false;
        }

        public async Task SignOutAsync()
        {
            await Handle.ErrorsAsync(async () => await Service.SignOutAsync(), PerformingSignOutDescription);

            SignedIn = false;
        }

        public SendMessageResult Send(string message)
        {
            SendMessageRequest request = CreateSendMessageRequest(message);

            SendMessageResponse response = Handle.Errors(() => Service.SendMessage(request),
                SendingAMessageDescription, ConnectAndSignIn);

            return ProcessSendMessageResult(response);
        }

        public async Task<SendMessageResult> SendAsync(string message)
        {
            SendMessageRequest request = CreateSendMessageRequest(message);

            SendMessageResponse response = await Handle.ErrorsAsync(async () => await Service.SendMessageAsync(request),
                SendingAMessageDescription, ConnectAndSignInAsync);

            return ProcessSendMessageResult(response);
        }

        public ReadOnlyCollection<string> GetNamesOnline()
        {
            return Handle.Errors(() => Service.GetNamesOnline(), GettingNamesOnlineDescription);
        }

        public async Task<ReadOnlyCollection<string>> GetNamesOnlineAsync()
        {
            return await Handle.ErrorsAsync(() => Service.GetNamesOnlineAsync(), GettingNamesOnlineDescription);
        }

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

        private SignInRequest CreateSignInRequest(string name)
        {
            var request = new SignInRequest {Name = name};
            _lastName = name;
            return request;
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

        private static SendMessageRequest CreateSendMessageRequest(string message)
        {
            var request = new SendMessageRequest {Message = message};
            return request;
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

        private void Connect(Exception exception)
        {
            if (exception is CommunicationException)
            {
                Connect(_lastConfiguration);
            }
        }

        private void ConnectAndSignIn(Exception exception)
        {
            if (exception is CommunicationException)
            {
                // Open a new channel
                Connect(_lastConfiguration);
                SignIn(_lastName);
            }
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