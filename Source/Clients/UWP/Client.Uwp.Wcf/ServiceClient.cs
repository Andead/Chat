using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using Andead.Chat.Client.Uwp.Resources;
using Andead.Chat.Client.Uwp.Wcf.ChatService;

namespace Andead.Chat.Client.Uwp.Wcf
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
            try
            {
                ((ICommunicationObject) Service).Close(_timeout);
            }
            catch (CommunicationObjectFaultedException)
            {
            }
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

            SignInResponse response;

            try
            {
                response = await Service.SignInAsync(request);
            }
            catch (CommunicationObjectFaultedException)
            {
                SignedIn = false;
                return new SignInResult(false);
            }

            SignedIn = response.Success;
            return new SignInResult(response.Success, response.Message, response.OnlineCount);
        }

        public void SignOut()
        {
            Task task = SignOutAsync();
            task.RunSynchronously();
        }

        public async Task SignOutAsync()
        {
            try
            {
                await Service.SignOutAsync();
            }
            catch (CommunicationObjectFaultedException)
            {
            }
            finally
            {
                SignedIn = false;
            }
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

            SendMessageResponse response;

            try
            {
                response = await Service.SendMessageAsync(request);
            }
            catch (CommunicationObjectFaultedException)
            {
                return new SendMessageResult {Success = false, Message = Errors.ConnectionHasBeenLostTryToRelogin};
            }

            return new SendMessageResult {Message = response.Message, Success = response.Success};
        }

        public ObservableCollection<string> GetNamesOnline()
        {
            Task<ObservableCollection<string>> task = GetNamesOnlineAsync();
            task.RunSynchronously();
            return task.Result;
        }

        public async Task<ObservableCollection<string>> GetNamesOnlineAsync()
        {
            try
            {
                return await Service.GetNamesOnlineAsync();
            }
            catch (CommunicationObjectFaultedException)
            {
                return null;
            }
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