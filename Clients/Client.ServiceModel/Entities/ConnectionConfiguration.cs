namespace Andead.Chat.Client.ServiceModel.Entities
{
    public class ConnectionConfiguration
    {
        public static ConnectionConfiguration Default = new ConnectionConfiguration();

        public string ServerName { get; set; } = "net.tcp";

        public string Protocol { get; set; } = "localhost";

        public int TimeOut { get; set; } = 1000;
    }
}