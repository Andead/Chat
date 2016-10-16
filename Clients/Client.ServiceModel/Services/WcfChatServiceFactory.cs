using System;
using System.ServiceModel;
using Andead.Chat.Client.ServiceModel.ChatService;
using Andead.Chat.Client.ServiceModel.Entities;
using Andead.Chat.Client.ServiceModel.Interfaces;

namespace Andead.Chat.Client.ServiceModel.Services
{
    public class WcfChatServiceFactory : IChatServiceFactory
    {
        public IChatService Create(ConnectionConfiguration configuration, IChatServiceCallback callbackClient)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            IChatService channel;
            switch (configuration.Protocol)
            {
                case "net.tcp":
                    channel = DuplexChannelFactory<IChatService>.CreateChannel(
                        new InstanceContext(callbackClient),
                        new NetTcpBinding(SecurityMode.None),
                        new EndpointAddress($"net.tcp://{configuration.ServerName}:{configuration.Port}/Service.svc"));
                    break;
                case "http":
                    channel = DuplexChannelFactory<IChatService>.CreateChannel(
                        new InstanceContext(callbackClient),
                        new WSDualHttpBinding(WSDualHttpSecurityMode.None),
                        new EndpointAddress($"http://{configuration.ServerName}:{configuration.Port}/Service.svc"));
                    break;
                default:
                    throw new NotSupportedException("Supported protocols are only net.tcp and http.");
            }

            return channel;
        }

        public void Dispose(IChatService chatService, ConnectionConfiguration configuration)
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(configuration.TimeOut);

            (chatService as ICommunicationObject)?.Close(timeout);
        }
    }
}