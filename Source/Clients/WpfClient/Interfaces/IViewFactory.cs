using Andead.Chat.Client;

namespace Andead.Chat.Clients.Wpf.Interfaces
{
    public interface IViewFactory
    {
        object GetView(ViewModel viewModel);
    }
}