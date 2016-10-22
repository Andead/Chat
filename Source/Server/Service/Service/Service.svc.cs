using System.Collections.ObjectModel;
using System.ServiceModel;
using Andead.Chat.Common.Logging;

namespace Andead.Chat.Server.Wcf
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IChatService
    {
        private readonly IChatService _service;

        public Service(IChatService service)
        {
            _service = service;
        }

        public Service()
        {
            var logger = new NullLogger();
            _service = new ChatService(new WcfChatClientsProvider(logger), logger);
        }

        public SignInResponse SignIn(SignInRequest request)
        {
            return _service.SignIn(request);
        }

        public void SignOut()
        {
            _service.SignOut();
        }

        public SendMessageResponse SendMessage(SendMessageRequest request)
        {
            return _service.SendMessage(request);
        }

        public int? GetOnlineCount()
        {
            return _service.GetOnlineCount();
        }

        public ReadOnlyCollection<string> GetNamesOnline()
        {
            return _service.GetNamesOnline();
        }
    }
}