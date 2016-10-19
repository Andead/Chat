using System;

namespace Andead.Chat.Client
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
        string[] GetNamesOnline();

        /// <summary>
        ///     Sends message to the chat.
        /// </summary>
        SendMessageResult Send(string message);

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