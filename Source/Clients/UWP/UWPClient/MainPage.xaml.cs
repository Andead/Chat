using Windows.UI.Xaml;
using Andead.Chat.Client.Uwp.Wcf;

namespace Andead.Chat.Client.Uwp
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            using (var client = new ServiceClient())
            {
                client.Connect(ConnectionConfiguration.Default);

                SignInResult result = await client.SignInAsync("UWP");

                if (!result.Success)
                {
                    throw new OperationFailedException("Failed to log in. " + result.Message);
                }

                SendMessageResult response = await client.SendAsync("Hello from UWP");

                if (!response.Success)
                {
                    throw new OperationFailedException(response.Message);
                }

                await client.SignOutAsync();
                client.Disconnect();
            }
        }
    }
}