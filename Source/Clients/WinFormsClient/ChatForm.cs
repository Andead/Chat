using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Andead.Chat.Client.Entities;
using Andead.Chat.Client.Interfaces;

namespace Andead.Chat.Client.WinForms
{
    public partial class ChatForm : Form
    {
        private readonly string _serverName;
        private readonly IAsyncServiceClient _client;

        private int? _onlineCount;
        private Timer _onlineCountTimer;

        public ChatForm(IAsyncServiceClient client)
        {
            _client = client;
            _serverName = client.ServerName;

            client.MessageReceived += ClientOnMessageReceived;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _onlineCountTimer = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            _onlineCountTimer.Tick += OnTimerOnTick;
            _onlineCountTimer.Start();
        }

        private async void OnTimerOnTick(object sender, EventArgs args)
        {
            int? onlineCount = await _client.GetOnlineCountAsync();
            if (_onlineCount == onlineCount)
            {
                return;
            }

            _onlineCount = onlineCount;
            UpdateOnlineCount(onlineCount);

            string[] names = await _client.GetNamesOnlineAsync();
            UpdateNames(names.ToArray<object>());
        }

        private void UpdateNames(object[] names)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string[]>(UpdateNames), names);
                return;
            }

            namesListBox.Items.Clear();
            namesListBox.Items.AddRange(names);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _client.SignOutAsync();
            _client.MessageReceived -= ClientOnMessageReceived;

            _onlineCountTimer?.Stop();

            base.OnClosing(e);
        }

        private void UpdateOnlineCount(int? onlineCount)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int?>(UpdateOnlineCount), onlineCount);
                return;
            }

            Text = onlineCount.HasValue ? $"{_serverName} ({onlineCount} users)" : $"{_serverName}";
        }

        private void ClientOnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ClientOnMessageReceived(sender, args)));
                return;
            }

            string message = args.Message;
            chatTextBox.AppendText(message + Environment.NewLine);
        }

        private async void button1_Click(Object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(messageTextBox.Text))
            {
                return;
            }

            string message = messageTextBox.Text;

            SendMessageResult result;
            try
            {
                sendButton.Enabled = false;
                result = await _client.SendAsync(message);
            }
            finally
            {
                sendButton.Enabled = true;
            }

            if (result.Success)
            {
                messageTextBox.Clear();
            }
        }

        private void textBox1_TextChanged(Object sender, EventArgs e)
        {
            sendButton.Enabled = ((TextBox) sender).Text.Length > 0;
        }

        private void namesListBox_Click(Object sender, EventArgs e)
        {
            var selectedName = ((ListBox) sender).SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedName))
            {
                messageTextBox.AppendText($"{selectedName}, ");
            }
        }
    }
}