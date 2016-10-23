using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Andead.Chat.Client.Uwp
{
    public sealed partial class ChatView
    {
        public ChatView()
        {
            InitializeComponent();
        }

        public new ChatViewModel DataContext
        {
            get { return (ChatViewModel) base.DataContext; }
            set { base.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext.SendMessage += OnSendMessage;
            DataContext.Messages.CollectionChanged += MessagesOnCollectionChanged;
            DataContext.PropertyChanged += OnPropertyChanged;
        }

        private async void MessagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => AppendMessages(args.NewItems.OfType<string>()));
            }
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ChatViewModel.Messages))
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => AppendMessages(DataContext.Messages, true));
            }
        }

        private async void OnSendMessage(object sender, SendMessageEventArgs args)
        {
            if (!args.Result.Success)
            {
                await ShowError(args.Result.Message);
                return;
            }

            AppendMessage(args.Result.Message);
        }

        private void AppendMessages(IEnumerable<string> messages, bool clear = false)
        {
            if (clear)
            {
                MessagesPanel.Children.Clear();
            }

            foreach (string message in messages)
            {
                AppendMessage(message);
            }
        }

        private void AppendMessage(string message)
        {
            MessagesPanel.Children.Add(new TextBlock { Text = $"{message}", Margin = new Thickness(0)});
            ScrollViewer.ChangeView(0.0f, double.MaxValue, 1.0f);
        }
    }
}