using Andead.Chat.Client;

namespace Andead.Chat.Clients.Wpf.Interfaces
{
    public interface IViewFactory
    {
        View GetView(ViewModel viewModel);

        View GetView<TViewModel>();

        View GetView(string viewModelTypeName);
    }
}