using System.Collections.ObjectModel;
using System.Linq;
using Andead.Chat.Common.Logging;
using Andead.Chat.Common.Policy;
using Andead.Chat.Common.Resources.Strings;
using Andead.Chat.Common.Utilities;

namespace Andead.Chat.Server
{
    public class ChatService : IChatService
    {
        private readonly object _access = new object();
        private readonly IChatClientsProvider _chatClientsProvider;
        private readonly ILogger _logger;

        public ChatService(IChatClientsProvider chatClientsProvider, ILogger logger)
        {
            chatClientsProvider.IsNotNull(nameof(chatClientsProvider));
            logger.IsNotNull(nameof(logger));

            _chatClientsProvider = chatClientsProvider;
            _logger = logger;

            chatClientsProvider.ClientsRemoved += ChatClientProviderOnClientsRemoved;
        }

        public SignInResponse SignIn(SignInRequest request)
        {
            lock (_access)
            {
                IChatClient currentClient = _chatClientsProvider.GetCurrentClient();
                if (currentClient == null)
                {
                    _logger.Warn("Sign in failure: wrong callback channel.", WarnCategory.InvalidRequest);
                    return SignInResponse.Failed(Errors.CallbackChannelFailure);
                }

                if (_chatClientsProvider.IsClientActive(currentClient) &&
                    _chatClientsProvider.IsClientSignedIn(currentClient))
                {
                    _logger.Warn("Sign in failure: already signed in.", WarnCategory.InvalidRequest);
                    return SignInResponse.Failed(Errors.AlreadySignedIn);
                }

                string name = request.Name;
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.Info("Sign in failure: empty name.", InfoCategory.Validation);
                    return SignInResponse.Failed(Errors.EmptyNameNotAllowed);
                }

                if (name.Length > Limits.UsernameMaxLength)
                {
                    _logger.Info("Sign in failure: name length exceeded limits.", InfoCategory.Validation);
                    return SignInResponse.Failed(Errors.NameLengthExceededLimits);
                }

                if (_chatClientsProvider.IsNameActive(name))
                {
                    _logger.Info("Sign in failure: name already taken.", InfoCategory.Validation);
                    return SignInResponse.Failed(Errors.NameAlreadyTaken);
                }

                _chatClientsProvider.AddCurrent(name);

                BroadcastUpdateOnlineCount();
                BroadcastMessage($"{name} has joined the chat.");

                SignInResponse response = SignInResponse.Successful();
                response.OnlineCount = _chatClientsProvider.GetClientsCount();

                return response;
            }
        }

        public void SignOut()
        {
            lock (_access)
            {
                IChatClient client = _chatClientsProvider.GetCurrentClient();
                if (!_chatClientsProvider.IsClientActive(client))
                {
                    _logger.Warn("Sign out denied for a non-active client.", WarnCategory.AccessDenied);
                    return;
                }

                string name = _chatClientsProvider.GetClientName(client);
                client.ReceiveMessage($"Goodbye, {name}!");

                _chatClientsProvider.RemoveClient(client);

                BroadcastUpdateOnlineCount();
                BroadcastMessage($"{name} has left the chat.");
            }
        }

        public SendMessageResponse SendMessage(SendMessageRequest request)
        {
            lock (_access)
            {
                if (request == null)
                {
                    _logger.Warn("Sending message denied for a null request.", WarnCategory.InvalidRequest);
                    return SendMessageResponse.Failed(Errors.InvalidRequest);
                }

                IChatClient client = _chatClientsProvider.GetCurrentClient();
                if (!_chatClientsProvider.IsClientActive(client))
                {
                    _logger.Warn("Sending message denied for a non-active client.", WarnCategory.AccessDenied);
                    return SendMessageResponse.Failed(Errors.AccessDenied);
                }

                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    _logger.Warn("Sending empty message denied.", WarnCategory.Validate);
                    return SendMessageResponse.Failed(Errors.MessageEmpty);
                }

                if (request.Message.Length > Limits.MessageMaxLength)
                {
                    _logger.Warn("Sending a message with a length exceeding limits was denied.", WarnCategory.Validate);
                    return SendMessageResponse.Failed(Errors.MessageLengthMustBeWithinLimits);
                }

                string name = _chatClientsProvider.GetClientName(client);

                BroadcastMessage($"{name}: {request.Message.Trim()}");

                return SendMessageResponse.Successful();
            }
        }

        public int? GetOnlineCount()
        {
            lock (_access)
            {
                IChatClient currentClient = _chatClientsProvider.GetCurrentClient();
                if (!_chatClientsProvider.IsClientActive(currentClient) ||
                    !_chatClientsProvider.IsClientSignedIn(currentClient))
                {
                    _logger.Warn("Getting online count denied for a non-active client.", WarnCategory.AccessDenied);
                    return null;
                }

                return _chatClientsProvider.GetClientsCount();
            }
        }

        public ReadOnlyCollection<string> GetNamesOnline()
        {
            lock (_access)
            {
                IChatClient currentClient = _chatClientsProvider.GetCurrentClient();
                if (!_chatClientsProvider.IsClientActive(currentClient))
                {
                    _logger.Warn("Getting names online denied for a non-active client.", WarnCategory.AccessDenied);
                    return null;
                }

                return _chatClientsProvider.GetClientsNames().ToList().AsReadOnly();
            }
        }

        private void ChatClientProviderOnClientsRemoved(object sender, ClientsRemovedEventArgs args)
        {
            lock (_access)
            {
                BroadcastMessage(
                    $"{string.Join(", ", args.RemovedClients.Values)} {(args.RemovedClients.Count > 1 ? "were" : "was")} lost.");
                BroadcastUpdateOnlineCount();
            }
        }

        private void BroadcastUpdateOnlineCount()
        {
            lock (_access)
            {
                int onlineCount = _chatClientsProvider.GetClientsCount();
                _chatClientsProvider.PerformAction(c => c.UpdateOnlineCount(onlineCount));

                _logger.Trace($"Updated online count for all clients: {onlineCount}.");
            }
        }

        private void BroadcastMessage(string message)
        {
            lock (_access)
            {
                _chatClientsProvider.PerformAction(c => c.ReceiveMessage(message));

                _logger.Trace(message);
            }
        }
    }
}