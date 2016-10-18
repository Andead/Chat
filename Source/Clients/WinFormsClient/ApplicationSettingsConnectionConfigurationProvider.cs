using Andead.Chat.Client.ServiceModel.Entities;
using Andead.Chat.Client.ServiceModel.Interfaces;
using Andead.Chat.Client.WinForms.Properties;

namespace Andead.Chat.Client.WinForms
{
    public class ApplicationSettingsConnectionConfigurationProvider : IConnectionConfigurationProvider
    {
        private static ConnectionConfiguration _connectionConfiguration;

        public ConnectionConfiguration GetConfiguration()
        {
            return _connectionConfiguration ??
                   (_connectionConfiguration = new ConnectionConfiguration
                   {
                       ServerName = Settings.Default.ServerName,
                       Protocol = Settings.Default.Protocol,
                       Port = Settings.Default.Port,
                       TimeOut = Settings.Default.Timeout
                   });
        }
    }
}