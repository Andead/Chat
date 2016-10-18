using System;
using System.Threading.Tasks;
using Andead.Chat.Client.Entities;

namespace Andead.Chat.Client.Interfaces
{
    public interface IServiceClient : IDisposable
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
        ///     Performs sign-in with a specified username asynchronously.
        /// </summary>
        /// <param name="name">Username.</param>
        Task<SignInResult> SignInAsync(string name);

        /// <summary>
        ///     Performs sign-out asynchronously.
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        ///     Gets the number of online users asynchronously.
        /// </summary>
        Task<int?> GetOnlineCountAsync();

        /// <summary>
        ///     Gets the names of online users asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<string[]> GetNamesOnlineAsync();

        /// <summary>
        ///     Sends message to the chat asynchronously.
        /// </summary>
        Task<SendMessageResult> SendAsync(string message);

        /// <summary>
        ///     Raises when a message is received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}