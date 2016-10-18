using System.ServiceModel;

namespace Andead.Chat.Server
{
    [ServiceContract]
    public interface IChatClient
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string message);
    }
}