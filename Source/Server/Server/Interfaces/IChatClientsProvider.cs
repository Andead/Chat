using System;
using System.Collections.Generic;

namespace Andead.Chat.Server
{
    public interface IChatClientsProvider
    {
        IChatClient GetCurrentClient();

        bool IsClientActive(IChatClient chatClient);

        bool IsNameActive(string name);

        void AddCurrent(string name);

        int GetClientsCount();

        string GetClientName(IChatClient chatClient);

        void RemoveClient(IChatClient client);

        IReadOnlyList<string> GetClientsNames();


        event EventHandler<ClientsRemovedEventArgs> ClientsRemoved;

        void PerformAction(Action<IChatClient> action);

        bool IsClientSignedIn(IChatClient chatClient);
    }
}