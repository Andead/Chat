using Andead.Chat.Client.ServiceModel.Entities;
using Andead.Chat.Client.ServiceModel.Interfaces;

namespace Andead.Chat.Client.ServiceModel.Services
{
    public class DefaultConnectionConfigurationProvider : IConnectionConfigurationProvider
    {
        public ConnectionConfiguration GetConfiguration()
        {
            return ConnectionConfiguration.Default;
        }
    }
}