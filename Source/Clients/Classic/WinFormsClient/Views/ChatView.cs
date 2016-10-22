using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Andead.Chat.Client.WinForms.Utilities;

namespace Andead.Chat.Client.WinForms
{
    internal partial class ChatView : View
    {
        public new ChatViewModel ViewModel => (ChatViewModel) base.ViewModel;

        public ChatView(ChatViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();

            ViewModel.Messages.CollectionChanged += MessagesOnCollectionChanged;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.Messages.CollectionChanged -= MessagesOnCollectionChanged;

            base.OnClosing(e);
        }

        private void MessagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            this.InvokeIfRequired(() =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (string message in args.NewItems.OfType<string>())
                    {
                        chatTextBox.AppendText(message + Environment.NewLine);
                    }
                }

                if (WindowState == FormWindowState.Minimized)
                {
                    Task.Run(new Action(FlashWhileMinimized));
                }
            });
        }

        private void FlashWhileMinimized()
        {
            Invoke(new Action(() => FlashWindow.Start(this)));

            var minimized = true;
            while (minimized)
            {
                Thread.Sleep(100);
                Invoke(new Action(() => minimized = WindowState == FormWindowState.Minimized));
            }

            Invoke(new Action(() => FlashWindow.Stop(this)));
        }

        protected override void Set(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(ChatViewModel.Title):
                    Text = ViewModel.Title;
                    return;
                case nameof(ChatViewModel.OnlineNames):
                    UpdateOnlineNames();
                    return;
                case nameof(ChatViewModel.SendEnabled):
                    sendButton.Enabled = ViewModel.SendEnabled;
                    return;
                case nameof(ChatViewModel.Messages):
                    chatTextBox.Clear();
                    chatTextBox.Text = string.Join(Environment.NewLine, ViewModel.Messages);
                    return;
                case nameof(ChatViewModel.Message):
                    messageTextBox.Text = ViewModel.Message;
                    return;
                default:
                    base.Set(propertyName);
                    return;
            }
        }

        private void UpdateOnlineNames()
        {
            namesListBox.Items.Clear();

            if (ViewModel.OnlineNames != null)
            {
                namesListBox.Items.AddRange(ViewModel.OnlineNames.ToArray<object>());
            }
        }

        private void sendButton_Click(Object sender, EventArgs e)
        {
            ViewModel.SendMessageCommand.TryExecute();
        }

        private void messageTextBox_TextChanged(Object sender, EventArgs e)
        {
            ViewModel.Message = messageTextBox.Text;
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