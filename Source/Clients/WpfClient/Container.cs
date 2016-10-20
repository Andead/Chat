using System.Windows;
using Andead.Chat.Client;
using Andead.Chat.Client.Wcf;
using Andead.Chat.Clients.Wpf.Interfaces;
using Andead.Chat.Clients.Wpf.Properties;
using Autofac;
using NLog;

namespace Andead.Chat.Clients.Wpf
{
    public class Container : DependencyObject
    {
        private readonly IContainer _container;

        public Container()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(LogManager.GetLogger("Default"))
                .As<ILogger>();

            var configuration = new ConnectionConfiguration
            {
                ServerName = Settings.Default.ServerName,
                Protocol = Settings.Default.Protocol,
                Port = Settings.Default.Port,
                TimeOut = Settings.Default.Timeout
            };

            builder.RegisterInstance(new ViewFactory(type => _container.Resolve(type) as View))
                .As<IViewFactory>();

            builder.RegisterType<ServiceClient>()
                .As<IAsyncServiceClient>()
                .OnActivated(args => args.Instance.Connect(configuration));

            builder.RegisterType<LoginViewModel>();

            builder.RegisterType<LoginView>();
            builder.RegisterType<ChatView>();

            _container = builder.Build();
        }

        public void Run()
        {
            var window = new ClientWindow
            {
                Title = "Login",
                DataContext = _container.Resolve<LoginViewModel>(),
                ContentControl = {Content = _container.Resolve<IViewFactory>().GetView<LoginViewModel>()},
                SizeToContent = SizeToContent.WidthAndHeight,
                MinWidth = 300,
                ResizeMode = ResizeMode.NoResize
            };

            window.Show();
        }
    }
}