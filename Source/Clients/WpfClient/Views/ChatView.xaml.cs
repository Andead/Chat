using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Andead.Chat.Client;

namespace Andead.Chat.Clients.Wpf
{
    /// <summary>
    ///     Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView
    {
        public ChatView()
        {
            InitializeComponent();
        }

        public ChatView(ChatViewModel chatViewModel)
            : base(chatViewModel)
        {
            InitializeComponent();
        }

        protected override void Load(ViewModel viewModel)
        {
            ((ChatViewModel) viewModel).PropertyChanged += OnPropertyChanged;
            ((ChatViewModel) viewModel).Messages.CollectionChanged += MessagesOnCollectionChanged;

            base.Load(viewModel);
        }

        protected override void Unload(ViewModel viewModel)
        {
            ((ChatViewModel) viewModel).Messages.CollectionChanged += MessagesOnCollectionChanged;
            ((ChatViewModel) viewModel).PropertyChanged -= OnPropertyChanged;

            base.Unload(viewModel);
        }

        private void MessagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string message in args.NewItems.OfType<string>())
                {
                    MessagesTextBox.AppendText(message + Environment.NewLine);
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(ChatViewModel.Title):
                    UpdateTitle();
                    return;
                case nameof(ChatViewModel.Messages):
                    MessagesTextBox.Clear();
                    foreach (string message in ((ChatViewModel) DataContext).Messages)
                    {
                        MessagesTextBox.AppendText(message);
                    }

                    break;
            }
        }

        private void UpdateTitle()
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Title = ((ChatViewModel) DataContext).Title;
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = e.AddedItems.OfType<string>().FirstOrDefault();
            if (name != null)
            {
                ((ChatViewModel) DataContext).Message += $"{name}, ";
            }
        }
    }
}