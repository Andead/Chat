using System.ServiceModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UWPClient.ServiceReference1;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPClient
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IChatServiceCallback
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var factory = new DuplexChannelFactory<IChatService>(new InstanceContext(this),
                new NetTcpBinding(SecurityMode.None), new EndpointAddress("net.tcp://<your_host>:808/Service.svc"));
            var client = factory.CreateChannel();

            SignInResponse inResponse = await client.SignInAsync(new SignInRequest {Name = "UWP Client"});

            SendMessageResponse response =
                await client.SendMessageAsync(new SendMessageRequest {Message = "Hello, UWP!"});

            ((ICommunicationObject) client).Close();
        }

        public void ReceiveMessage(string message)
        {
            // todo
        }

        public void UpdateOnlineCount(int value)
        {
            // todo
        }
    }
}