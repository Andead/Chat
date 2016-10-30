using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Andead.Chat.Client
{
    public interface IAsyncServiceClient : IServiceClient
    {
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
        Task<ReadOnlyCollection<string>> GetNamesOnlineAsync();

        /// <summary>
        ///     Sends message to the chat asynchronously.
        /// </summary>
        Task<SendMessageResult> SendAsync(string message);
    }
}