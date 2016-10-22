using Andead.Chat.Client.Uwp.Policy;

namespace Andead.Chat.Client.Uwp.Resources
{
    /// <summary>
    ///     Contains server error messages.
    /// </summary>
    public static class Errors
    {
        public static readonly string CallbackChannelFailure = "The callback channel provided was not correct.";

        public static readonly string EmptyNameNotAllowed = "You must have a name to sign in.";

        public static readonly string AlreadySignedIn = "You are already in the chat.";

        public static readonly string NameAlreadyTaken = "The name has been taken by someone else.";

        public static readonly string NameLengthExceededLimits = "The name length must be not greater than 50.";

        public static readonly string AccessDenied = "Access denied";

        public static readonly string InvalidRequest = "Invalid request";

        public static readonly string MessageEmpty = "Message cannot be empty";

        public static readonly string MessageLengthMustBeWithinLimits =
            $"Message length must be not greater than {Limits.MessageMaxLength}";

        public static readonly string ConnectionHasBeenLostTryToRelogin = "Connection has been lost. Try to relogin.";
    }
}