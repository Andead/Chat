using System;
using System.ComponentModel;
using Andead.Chat.Client.WinForms.Properties;
using Andead.Chat.Common.Utilities;

namespace Andead.Chat.Client.WinForms
{
    internal partial class LoginView : View
    {
        public LoginView(LoginViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            nameTextBox.Text = Settings.Default.Username;

            ViewModel.SignIn += ViewModelOnSignIn;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.SignIn -= ViewModelOnSignIn;

            base.OnClosing(e);
        }

        public new LoginViewModel ViewModel => (LoginViewModel) base.ViewModel;

        private void ViewModelOnSignIn(object sender, SignInEventArgs args)
        {
            if (!args.Result.Success)
            {
                ShowError(args.Result.Message);
                return;
            }

            var chatForm = new ChatView(args.ChatViewModel);
            chatForm.Closed += new OneTimeEventHandler(() =>
            {
                Show();
                ViewModel.Reload();
            });

            chatForm.Show();

            Hide();
        }

        protected override void Set(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(LoginViewModel.SignInEnabled):
                    signInButton.Enabled = ViewModel.SignInEnabled;
                    return;
                default:
                    base.Set(propertyName);
                    return;
            }
        }

        private void nameTextBox_TextChanged(Object sender, EventArgs e)
        {
            ViewModel.Name = nameTextBox.Text;
        }

        private void signInButton_Click(Object sender, EventArgs e)
        {
            ViewModel.SignInCommand.TryExecute();
        }
    }
}