using System;
using System.Windows.Forms;
using Andead.Chat.Client.Wcf;
using Andead.Chat.Client.WinForms.Properties;
using NLog;

namespace Andead.Chat.Client.WinForms
{
    public class Bootstrapper
    {
        public void Run()
        {
            ILogger logger = LogManager.GetLogger("Default");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var configuration = new ConnectionConfiguration
            {
                ServerName = Settings.Default.ServerName,
                Protocol = Settings.Default.Protocol,
                Port = Settings.Default.Port,
                TimeOut = Settings.Default.Timeout
            };

            using (var client = new ServiceClient())
            {
                client.Connect(configuration);

                var loginViewModel = new LoginViewModel(client);
                var loginForm = new LoginView(loginViewModel);

                try
                {
                    Application.Run(loginForm);
                }
                catch (Exception e)
                {
                    logger.Fatal(e);
                }
            }
        }
    }
}