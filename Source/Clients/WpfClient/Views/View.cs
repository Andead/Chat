using System.IO;
using System.Windows;
using System.Windows.Controls;
using Andead.Chat.Client;
using Andead.Chat.Clients.Wpf.Views;

namespace Andead.Chat.Clients.Wpf
{
    public abstract class View : UserControl
    {
        protected View()
        {
            DataContextChanged += OnDataContextChanged;
        }

        protected View(ViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        protected new ViewModel DataContext
        {
            get { return (ViewModel) base.DataContext; }
            set { base.DataContext = value; }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            if (args.NewValue == null)
            {
                Unload(args.OldValue as ViewModel);
                return;
            }

            if (args.NewValue != null)
            {
                if (args.OldValue != null)
                {
                    Unload(args.OldValue as ViewModel);
                }

                var viewModel = args.NewValue as ViewModel;
                if (viewModel == null)
                {
                    return;
                }

                Load(viewModel);
            }
        }

        protected virtual void Load(ViewModel viewModel)
        {
            DataContext.Load();
            DataContext.Error += ViewModelOnError;
        }

        protected virtual void Unload(ViewModel viewModel)
        {
            DataContext.Error -= ViewModelOnError;
            DataContext.Unload();
        }

        private static void ViewModelOnError(object sender, ErrorEventArgs args)
        {
            ShowError(args.GetException().Message);
        }

        protected static void ShowError(string message)
        {
            try
            {
                var window = new ClientWindow
                {
                    Title = "Error",
                    ContentControl = {Content = new ErrorView {DataContext = message}},
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize
                };

                window.ShowDialog();
            }
            catch
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}