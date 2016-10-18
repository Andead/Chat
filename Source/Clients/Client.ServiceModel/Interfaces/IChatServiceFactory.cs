using Andead.Chat.Client.ServiceModel.ChatService;
using Andead.Chat.Client.ServiceModel.Entities;

namespace Andead.Chat.Client.ServiceModel.Interfaces
{
    public interface IChatServiceFactory
    {
        IChatService Create(ConnectionConfiguration configuration, IChatServiceCallback callbackClient);

        void Dispose(IChatService chatService, ConnectionConfiguration configuration);
    }
}