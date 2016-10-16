using System.ServiceModel;

namespace Andead.Chat.Server.Wcf
{
    public class CurrentOperationContextChatClientProvider : IChatClientProvider
    {
        public IChatClient GetCurrent()
        {
            return OperationContext.Current.GetCallbackChannel<IChatClient>();
        }
    }
}