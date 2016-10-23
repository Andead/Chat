using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Andead.Chat.Client.Uwp
{
    public abstract class View : Page
    {
        protected static async Task ShowError(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = e.Parameter as ViewModel;
            if (viewModel == null)
            {
                return;
            }

            DataContext = viewModel;
            viewModel.Error += ViewModelOnError;
        }

        private static async void ViewModelOnError(object sender, ErrorEventArgs eventArgs)
        {
            await ShowError(eventArgs.GetException().Message);
        }
    }
}