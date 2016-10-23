using Windows.UI.Xaml.Navigation;

namespace Andead.Chat.Client.Uwp
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public new LoginViewModel DataContext
        {
            get { return (LoginViewModel) base.DataContext; }
            set { base.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext.Entered += OnEntered;
        }

        private async void OnEntered(object sender, SignInEventArgs args)
        {
            if (args.Result.Success)
            {
                Frame.Navigate(typeof(ChatView), args.ChatViewModel);
                return;
            }

            await ShowError(args.Result.Message);
        }
    }
}