using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Andead.Chat.Common.Logging;
using Andead.Chat.Common.Utilities;

namespace Andead.Chat.Server.Wcf
{
    public class WcfChatClientsProvider : IChatClientsProvider
    {
        private readonly ILogger _logger;

        private readonly IDictionary<IChatClient, string> _clients
            = new Dictionary<IChatClient, string>();

        public WcfChatClientsProvider(ILogger logger)
        {
            logger.IsNotNull(nameof(logger));

            _logger = logger;
        }

        public IChatClient GetCurrentClient()
        {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IChatClient>();

            return callbackChannel;
        }

        public bool IsClientActive(IChatClient chatClient)
        {
            return (chatClient as ICommunicationObject)?.State == CommunicationState.Opened;
        }

        public bool IsClientSignedIn(IChatClient chatClient)
        {
            return _clients.ContainsKey(chatClient);
        }

        public bool IsNameActive(string name)
        {
            if (!_clients.Values.Contains(name))
            {
                return false;
            }

            IChatClient chatClient = _clients.First(pair => pair.Value == name).Key;
            return IsClientActive(chatClient);
        }

        public void AddCurrent(string name)
        {
            _clients.Add(GetCurrentClient(), name);
        }

        public int GetClientsCount()
        {
            return _clients.Count;
        }

        public string GetClientName(IChatClient chatClient)
        {
            return _clients[chatClient];
        }

        public void RemoveClient(IChatClient client)
        {
            _clients.Remove(client);
        }

        public IReadOnlyList<string> GetClientsNames()
        {
            return _clients.Values.ToList().AsReadOnly();
        }

        public event EventHandler<ClientsRemovedEventArgs> ClientsRemoved;

        protected virtual void OnClientsRemoved(ClientsRemovedEventArgs e)
        {
            ClientsRemoved?.Invoke(this, e);
        }

        public void PerformAction(Action<IChatClient> action)
        {
            var toRemove = new Dictionary<IChatClient, string>();

            foreach (KeyValuePair<IChatClient, string> pair in _clients)
            {
                try
                {
                    action(pair.Key);
                    continue;
                }
                catch (CommunicationObjectAbortedException e)
                {
                    _logger.Warn(
                        $"The channel {pair.Value} aborted the connection and will be removed. {e.Message}",
                        WarnCategory.UnexpectedBehavior);
                }
                catch (CommunicationObjectFaultedException e)
                {
                    _logger.Warn(
                        $"The channel {pair.Value} connection faulted and will be removed. {e.Message}",
                        WarnCategory.UnexpectedBehavior);
                }
                catch (Exception exception)
                {
                    _logger.Warn($"The channel {pair.Value} caused an exception and will be removed. {exception.Message}",
                        WarnCategory.UnexpectedBehavior);
                }

                toRemove.Add(pair.Key, pair.Value);
            }

            if (toRemove.Any())
            {
                foreach (KeyValuePair<IChatClient, string> pair in toRemove)
                {
                    _clients.Remove(pair);
                }

                OnClientsRemoved(new ClientsRemovedEventArgs(toRemove));
            }
        }
    }
}