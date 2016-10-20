using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Andead.Chat.Common.Logging;
using Andead.Chat.Common.Policy;
using Andead.Chat.Common.Resources.Strings;

namespace Andead.Chat.Server
{
    public class ChatService : IChatService
    {
        private readonly IChatClientProvider _chatClientProvider;

        private readonly IDictionary<IChatClient, string> _clients
            = new Dictionary<IChatClient, string>();

        private readonly ILogger _logger;

        public ChatService(IChatClientProvider chatClientProvider, ILogger logger)
        {
            if (chatClientProvider == null)
            {
                throw new ArgumentNullException(nameof(chatClientProvider));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _chatClientProvider = chatClientProvider;
            _logger = logger;
        }

        public SignInResponse SignIn(SignInRequest request)
        {
            lock (_clients)
            {
                IChatClient currentClient = _chatClientProvider.GetCurrent();
                if (currentClient == null)
                {
                    _logger.Warn("Sign in failure: wrong callback channel.", WarnCategory.InvalidRequest);
                    return SignInResponse.Failed(Errors.CallbackChannelFailure);
                }

                if (_clients.ContainsKey(currentClient))
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

                if (_clients.Values.Contains(name))
                {
                    _logger.Info("Sign in failure: name already taken.", InfoCategory.Validation);
                    return SignInResponse.Failed(Errors.NameAlreadyTaken);
                }

                _clients.Add(currentClient, name);

                BroadcastUpdateOnlineCount();
                BroadcastMessage($"{name} has joined the chat.");

                SignInResponse response = SignInResponse.Successful();
                response.OnlineCount = _clients.Count;

                return response;
            }
        }

        public void SignOut()
        {
            lock (_clients)
            {
                IChatClient currentClient = _chatClientProvider.GetCurrent();
                if (!_clients.ContainsKey(currentClient))
                {
                    _logger.Warn("Sign out denied for a non-active client.", WarnCategory.AccessDenied);
                    return;
                }

                string name = _clients[currentClient];
                currentClient.ReceiveMessage($"Goodbye, {name}!");

                _clients.Remove(currentClient);

                BroadcastUpdateOnlineCount();
                BroadcastMessage($"{name} has left the chat.");
            }
        }

        public SendMessageResponse SendMessage(SendMessageRequest request)
        {
            lock (_clients)
            {
                if (request == null)
                {
                    _logger.Warn("Sending message denied for a null request.", WarnCategory.InvalidRequest);
                    return SendMessageResponse.Failed(Errors.InvalidRequest);
                }

                IChatClient currentClient = _chatClientProvider.GetCurrent();
                if (!_clients.ContainsKey(currentClient))
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

                string name = _clients[currentClient];

                BroadcastMessage($"{name}: {request.Message}");

                return SendMessageResponse.Successful();
            }
        }

        public int? GetOnlineCount()
        {
            lock (_clients)
            {
                IChatClient currentClient = _chatClientProvider.GetCurrent();
                if (!_clients.ContainsKey(currentClient))
                {
                    _logger.Warn("Getting online count denied for a non-active client.", WarnCategory.AccessDenied);
                    return null;
                }

                return _clients.Count;
            }
        }

        public List<string> GetNamesOnline()
        {
            lock (_clients)
            {
                IChatClient currentClient = _chatClientProvider.GetCurrent();
                if (!_clients.ContainsKey(currentClient))
                {
                    _logger.Warn("Getting names online denied for a non-active client.", WarnCategory.AccessDenied);
                    return null;
                }

                return _clients.Values.ToList();
            }
        }

        private void BroadcastUpdateOnlineCount()
        {
            PerformAction(c => c.UpdateOnlineCount(_clients.Count));

            _logger.Trace($"Updated online count for all clients: {_clients.Count}.");
        }

        private void BroadcastMessage(string message)
        {
            PerformAction(c => c.ReceiveMessage(message));

            _logger.Trace(message);
        }

        private void PerformAction(Action<IChatClient> action)
        {
            var toRemove = new Dictionary<IChatClient, string>();

            lock (_clients)
            {
                foreach (KeyValuePair<IChatClient, string> pair in _clients)
                {
                    try
                    {
                        action(pair.Key);
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                        _logger.Warn($"The channel {pair.Value} is absent and will be removed. ",
                            WarnCategory.UnexpectedBehavior);

                        toRemove.Add(pair.Key, pair.Value);
                    }
                }

                if (toRemove.Any())
                {
                    foreach (KeyValuePair<IChatClient, string> pair in toRemove)
                    {
                        _clients.Remove(pair);
                    }

                    BroadcastMessage(
                        $"{string.Join(", ", toRemove.Values)} {(toRemove.Count > 1 ? "were" : "was")} lost.");
                    BroadcastUpdateOnlineCount();
                }
            }
        }
    }
}