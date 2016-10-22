using System;
using System.Windows.Forms;
using Andead.Chat.Client.Wcf;
using Andead.Chat.Client.WinForms.Properties;
using Andead.Chat.Common.Utilities;
using NLog;

namespace Andead.Chat.Client.WinForms
{
    public class Bootstrapper
    {
        public void Run()
        {
            Handle.Logger = new NLogLogger(LogManager.GetLogger("Default"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var configuration = new ConnectionConfiguration
            {
                ServerName = Settings.Default.ServerName,
                Protocol = Settings.Default.Protocol,
                Port = Settings.Default.Port,
                TimeOut = Settings.Default.Timeout
            };

            var loginViewModel = new LoginViewModel(new ServiceClientFactory(), configuration);
            var loginForm = new LoginView(loginViewModel);

            try
            {
                Application.Run(loginForm);
            }
            catch (Exception e)
            {
                Handle.Logger.Fatal(e.ToString());
            }
        }

        private class ServiceClientFactory : IServiceClientFactory
        {
            public IAsyncServiceClient GetAsyncServiceClient()
            {
                return new ServiceClient();
            }

            public IServiceClient GetServiceClient()
            {
                return new ServiceClient();
            }
        }
    }
}