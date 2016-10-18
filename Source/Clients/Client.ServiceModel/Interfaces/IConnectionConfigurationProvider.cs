using Andead.Chat.Client.ServiceModel.Entities;

namespace Andead.Chat.Client.ServiceModel.Interfaces
{
    public interface IConnectionConfigurationProvider
    {
        ConnectionConfiguration GetConfiguration();
    }
}