namespace Andead.Chat.Client.Uwp
{
    public class ConnectionConfiguration
    {
        public static ConnectionConfiguration Default = new ConnectionConfiguration();

        public string ServerName { get; set; } = "localhost";

        public string Protocol { get; set; } = "net.tcp";

        public bool UseSsl { get; set; } = false;

        public short Port { get; set; } = 808;

        public int TimeOut { get; set; } = 1000;
    }
}