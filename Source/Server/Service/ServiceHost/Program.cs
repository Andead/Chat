using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using Andead.Chat.Server;
using Andead.Chat.Server.Wcf;
using NLog;

namespace ServiceHost
{
    internal class Program
    {
        private static System.ServiceModel.ServiceHost _host;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static bool _useSsl;
        private static string _thumbprint;

        private static void Main(params string[] args)
        {
            RenderArgs(args);

            Console.CancelKeyPress += (o, e) => CloseHost();

            try
            {
                var logger = new NLogLogger(LogManager.GetCurrentClassLogger(typeof(ChatService)));
                _host = new System.ServiceModel.ServiceHost(
                    new Service(new ChatService(new WcfChatClientsProvider(logger), logger)));

                if (_useSsl)
                {
                    var binding = _host.Description.Endpoints[0].Binding as NetTcpBinding;
                    if (binding != null)
                    {
                        binding.Security.Mode = SecurityMode.Transport;
                        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

                        _host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My,
                            X509FindType.FindByThumbprint, _thumbprint);
                    }
                }

                // Start
                _host.Open();

                Logger.Info(
                    $"The service is ready at {_host.BaseAddresses.Select(uri => uri.ToString()).Aggregate((s, s1) => s + ", " + s1)}.");

                if (_host.Credentials?.ServiceCertificate != null)
                {
                    Logger.Info($"Security is enabled.");
                }

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

        private static void RenderArgs(IReadOnlyCollection<string> args)
        {
            if (args == null || args.Count < 1)
            {
                return;
            }

            if (args.Any(arg => String.Equals(arg, "/hide", StringComparison.OrdinalIgnoreCase)))
            {
                IntPtr handle = NativeMethods.GetConsoleWindow();

                NativeMethods.ShowWindow(handle, NativeMethods.SW_HIDE);
            }

            if (args.Any(arg => string.Equals(arg, "/secure", StringComparison.OrdinalIgnoreCase)))
            {
                string thumbprint = args.FirstOrDefault(arg => arg.StartsWith("/thumbprint", StringComparison.OrdinalIgnoreCase));
                if (thumbprint != null)
                {
                    _useSsl = true;

                    _thumbprint = thumbprint.Split('=')[1];
                }
            }

        }

        private static void CloseHost()
        {
            _host?.Close();
        }
    }
}