using System.Runtime.Serialization;

namespace Andead.Chat.Server
{
    [DataContract]
    public class SendMessageResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        public static SendMessageResponse Failed(string errorMessage)
        {
            return new SendMessageResponse
            {
                Success = false,
                Message = errorMessage
            };
        }

        public static SendMessageResponse Successful()
        {
            return new SendMessageResponse
            {
                Success = true
            };
        }
    }
}