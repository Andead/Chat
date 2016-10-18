namespace Andead.Chat.Server
{
    public interface IChatClientProvider
    {
        IChatClient GetCurrent();
    }
}