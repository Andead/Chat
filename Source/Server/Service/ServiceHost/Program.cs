using System;
using System.Collections.Generic;
using System.Linq;
using Andead.Chat.Server;
using Andead.Chat.Server.Wcf;
using NLog;

namespace ServiceHost
{
    internal class Program
    {
        private static System.ServiceModel.ServiceHost _host;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(params string[] args)
        {
            RenderArgs(args);

            Console.CancelKeyPress += (o, e) => CloseHost();

            try
            {
                var logger = new NLogLogger(LogManager.GetCurrentClassLogger(typeof(ChatService)));
                _host = new System.ServiceModel.ServiceHost(
                    new Service(new ChatService(new WcfChatClientsProvider(logger), logger)));

                // Start
                _host.Open();

                Logger.Info(
                    $"The service is ready at {_host.BaseAddresses.Select(uri => uri.ToString()).Aggregate((s, s1) => s + ", " + s1)}.");
                Logger.Info("Press <Enter> to stop the service.\n");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Logger.Error("Service can not be started \n\nError Message [" + exception.Message + "]");
            }
            finally
            {
                // Stop
                CloseHost();
            }
        }

        private static void RenderArgs(IEnumerable<string> args)
        {
            if (String.Equals(args?.ElementAtOrDefault(0), "/hide", StringComparison.OrdinalIgnoreCase))
            {
                IntPtr handle = NativeMethods.GetConsoleWindow();

                NativeMethods.ShowWindow(handle, NativeMethods.SW_HIDE);
            }
        }

        private static void CloseHost()
        {
            _host?.Close();
        }
    }
}