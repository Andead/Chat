using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Andead.Chat.Client.Entities;
using Andead.Chat.Client.Interfaces;
using Andead.Chat.Client.Wcf.ChatService;

namespace Andead.Chat.Client.Wcf
{
    public class ServiceClient : IServiceClient, IChatServiceCallback
    {
        private TimeSpan _timeout;

        /// <summary>
        ///     This property is marked virtual and public for testing purposes only.
        /// </summary>
        public virtual IChatService Service { get; private set; }

        void IChatServiceCallback.ReceiveMessage(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        public bool SignedIn { get; private set; }

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

        public async Task<int?> GetOnlineCountAsync()
        {
            return await Service.GetOnlineCountAsync();
        }

        public async Task<SignInResult> SignInAsync(string name)
        {
            var request = new SignInRequest {Name = name};

            SignInResponse response = await Service.SignInAsync(request);

            SignedIn = response.Success;

            return new SignInResult(response.Success, response.Message);
        }

        public virtual event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public async Task SignOutAsync()
        {
            await Service.SignOutAsync();

            SignedIn = false;
        }

        public async Task<SendMessageResult> SendAsync(string message)
        {
            var request = new SendMessageRequest {Message = message};

            SendMessageResponse response = await Service.SendMessageAsync(request);

            var result = new SendMessageResult {Message = response.Message, Success = response.Success};

            return result;
        }

        public async Task<string[]> GetNamesOnlineAsync()
        {
            return await Service.GetNamesOnlineAsync();
        }
    }
}