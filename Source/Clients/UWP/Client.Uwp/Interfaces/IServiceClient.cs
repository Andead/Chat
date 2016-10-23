using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Andead.Chat.Client.Uwp
{
    public interface IServiceClient
    {
        /// <summary>
        ///     Gets a boolean value indicating that the client is currently signed in to server.
        /// </summary>
        bool SignedIn { get; }

        /// <summary>
        ///     Gets the server name.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        ///     Gets a boolean value indicating whether the client is using SSL connection.
        /// </summary>
        bool UsesSsl { get; }

        /// <summary>
        ///     Opens the connection.
        /// </summary>
        void Connect(ConnectionConfiguration configuration);

        /// <summary>
        ///     Closes the connection.
        /// </summary>
        void Disconnect();

        /// <summary>
        ///     Performs sign-in with a specified username.
        /// </summary>
        /// <param name="name">Username.</param>
        SignInResult SignIn(string name);

        /// <summary>
        ///     Performs sign-out.
        /// </summary>
        void SignOut();

        /// <summary>
        ///     Gets the names of online users.
        /// </summary>
        ObservableCollection<string> GetNamesOnline();

        /// <summary>
        ///     Sends message to the chat.
        /// </summary>
        SendMessageResult Send(string message);

        /// <summary>
        ///     Performs sign-in with a specified username asynchronously.
        /// </summary>
        /// <param name="name">Username.</param>
        Task<SignInResult> SignInAsync(string name);

        /// <summary>
        ///     Performs sign-out asynchronously.
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        ///     Gets the names of online users asynchronously.
        /// </summary>
        Task<ObservableCollection<string>> GetNamesOnlineAsync();

        /// <summary>
        ///     Sends message to the chat asynchronously.
        /// </summary>
        Task<SendMessageResult> SendAsync(string message);

        /// <summary>
        ///     Raises when a message is received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        ///     Raises when online users count is updated.
        /// </summary>
        event EventHandler<OnlineCountUpdatedEventArgs> OnlineCountUpdated;
    }
}