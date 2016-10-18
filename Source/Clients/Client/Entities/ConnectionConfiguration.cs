namespace Andead.Chat.Client.Entities
{
    public class ConnectionConfiguration
    {
        public static ConnectionConfiguration Default = new ConnectionConfiguration();

        public string ServerName { get; set; } = "localhost";

        public string Protocol { get; set; } = "net.tcp";

        public short Port { get; set; } = 808;

        public int TimeOut { get; set; } = 1000;
    }
}