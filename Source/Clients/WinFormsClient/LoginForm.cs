using System;
using System.Windows.Forms;
using Andead.Chat.Client.Entities;
using Andead.Chat.Client.Wcf;
using Andead.Chat.Client.WinForms.Properties;

namespace Andead.Chat.Client.WinForms
{
    public partial class LoginForm : Form
    {
        private readonly ServiceClient _client;

        public LoginForm()
        {
            InitializeComponent();

            _client = new ServiceClient();
            _client.Connect(new ConnectionConfiguration
            {
                ServerName = Settings.Default.ServerName,
                Protocol = Settings.Default.Protocol,
                Port = Settings.Default.Port,
                TimeOut = Settings.Default.Timeout
            });

            nameTextBox.Text = Settings.Default.Username;
        }

        private void textBox1_TextChanged(Object sender, EventArgs e)
        {
            signInButton.Enabled = ((TextBox) sender).Text.Length > 0;
        }

        private async void signInButton_Click(Object sender, EventArgs e)
        {
            string name = nameTextBox.Text;
            SignInResult result;

            try
            {
                ((Button) sender).Enabled = false;
                result = await _client.SignInAsync(name);
            }
            finally
            {
                ((Button) sender).Enabled = true;
            }

            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chatForm = new ChatForm(_client);
            chatForm.Closed += (o, args) => Show();
            chatForm.Show();

            Hide();
        }
    }
}