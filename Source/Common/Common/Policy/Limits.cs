namespace Andead.Chat.Common.Policy
{
    public static class Limits
    {
        public static readonly int UsernameMaxLength = 50;

        public static readonly int MessageMaxLength = 2048;

        public static readonly int ReconnectTimes = 5;
    }
}