using System;
using System.Collections.Generic;

namespace Andead.Chat.Server
{
    public class ClientsRemovedEventArgs : EventArgs
    {
        public ClientsRemovedEventArgs(IReadOnlyDictionary<IChatClient, string> removedClients)
        {
            RemovedClients = removedClients;
        }

        public IReadOnlyDictionary<IChatClient, string> RemovedClients { get; }
    }
}