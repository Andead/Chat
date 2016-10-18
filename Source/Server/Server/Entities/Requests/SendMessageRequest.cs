using System.Runtime.Serialization;

namespace Andead.Chat.Server
{
    [DataContract]
    public class SendMessageRequest
    {
        [DataMember]
        public string Message { get; set; }
    }
}