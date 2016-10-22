using System;
using Andead.Chat.Client;
using Andead.Chat.Common.Utilities;

namespace Andead.Chat.Clients.Wpf
{
    public class ServiceClientFactory : IServiceClientFactory
    {
        private readonly Func<IAsyncServiceClient> _getAsyncServiceClient;
        private readonly Func<IServiceClient> _getServiceClient;

        public ServiceClientFactory(Func<IServiceClient> getServiceClient)
        {
            getServiceClient.IsNotNull(nameof(getServiceClient));
            _getServiceClient = getServiceClient;
        }

        public ServiceClientFactory(Func<IAsyncServiceClient> getAsyncServiceClient)
            : this((Func<IServiceClient>) getAsyncServiceClient)
        {
            getAsyncServiceClient.IsNotNull(nameof(getAsyncServiceClient));
            _getAsyncServiceClient = getAsyncServiceClient;
        }

        public ServiceClientFactory(Func<IAsyncServiceClient> getAsyncServiceClient,
            Func<IServiceClient> getServiceClient)
        {
            getServiceClient.IsNotNull(nameof(getServiceClient));
            getAsyncServiceClient.IsNotNull(nameof(getAsyncServiceClient));

            _getAsyncServiceClient = getAsyncServiceClient;
            _getServiceClient = getServiceClient;
        }

        public IAsyncServiceClient GetAsyncServiceClient()
        {
            IAsyncServiceClient client = _getAsyncServiceClient();

            return client;
        }

        public IServiceClient GetServiceClient()
        {
            IServiceClient client = _getServiceClient();

            return client;
        }
    }
}