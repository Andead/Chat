using System;
using System.Threading.Tasks;
using Andead.Chat.Client.Entities;
using Andead.Chat.Client.Interfaces;
using Andead.Chat.Client.ServiceModel.ChatService;
using Andead.Chat.Client.ServiceModel.Interfaces;

namespace Andead.Chat.Client.ServiceModel.Services
{
    public class ServiceClient : IServiceClient, IChatServiceCallback
    {
        private readonly IChatServiceFactory _chatServiceFactory;
        private readonly IConnectionConfigurationProvider _connectionConfigurationProvider;
        private IChatService _service;

        public ServiceClient(IConnectionConfigurationProvider connectionConfigurationProvider,
            IChatServiceFactory chatServiceFactory)
        {
            if (connectionConfigurationProvider == null)
            {
                throw new ArgumentNullException(nameof(connectionConfigurationProvider));
            }
            if (chatServiceFactory == null)
            {
                throw new ArgumentNullException(nameof(chatServiceFactory));
            }

            _connectionConfigurationProvider = connectionConfigurationProvider;
            _chatServiceFactory = chatServiceFactory;
        }

        private IChatService Service => _service ?? (_service = CreateService());

        void IChatServiceCallback.ReceiveMessage(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        public bool SignedIn { get; private set; }

        public string ServerName => _connectionConfigurationProvider.GetConfiguration().ServerName;

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

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public async Task SignOutAsync()
        {
            await Service.SignOutAsync();

            SignedIn = false;
        }

        public async Task<SendMessageResult> SendAsync(String message)
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

        public void Dispose()
        {
            _chatServiceFactory.Dispose(_service, _connectionConfigurationProvider.GetConfiguration());
        }

        private IChatService CreateService()
        {
            return _chatServiceFactory.Create(_connectionConfigurationProvider.GetConfiguration(), this);
        }
    }
}