using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Andead.Chat.Client.Uwp
{
    public abstract class View : Page
    {
        protected new ViewModel DataContext
        {
            get { return (ViewModel) base.DataContext; }
            set { base.DataContext = value; }
        }

        protected static async Task ShowError(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = e.Parameter as ViewModel;
            if (viewModel == null)
            {
                return;
            }

            DataContext = viewModel;
            DataContext.Load();

            viewModel.Error += ViewModelOnError;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataContext.Unload();
        }

        private static async void ViewModelOnError(object sender, ErrorEventArgs eventArgs)
        {
            await ShowError(eventArgs.GetException().Message);
        }
    }
}