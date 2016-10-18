using System.Runtime.Serialization;

namespace Andead.Chat.Server
{
    [DataContract]
    public class SignInRequest
    {
        [DataMember]
        public string Name { get; set; }
    }
}