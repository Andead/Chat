using System;
using Andead.Chat.Client;

namespace Andead.Chat.Clients.Wpf.Interfaces
{
    internal class ViewFactory : IViewFactory
    {
        private static readonly string ViewModelNamespace = typeof(ViewModel).Namespace;
        private static readonly string ViewNamespace = typeof(View).Namespace;
        private readonly Func<Type, View> _resolver;

        public ViewFactory(Func<Type, View> resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            _resolver = resolver;
        }

        public View GetView(ViewModel viewModel)
        {
            return GetView(viewModel.GetType().FullName);
        }

        public View GetView<TViewModel>()
        {
            return GetView(typeof(TViewModel).FullName);
        }

        public View GetView(string viewModelTypeName)
        {
            string viewTypeName = viewModelTypeName
                .Replace(ViewModelNamespace, ViewNamespace)
                .Replace("ViewModel", "View");

            Type type = Type.GetType(viewTypeName);

            return _resolver(type);
        }
    }
}