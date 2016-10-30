namespace Andead.Chat.Client
{
    public interface IServiceClientFactory
    {
        IAsyncServiceClient GetAsyncServiceClient();

        IServiceClient GetServiceClient();
    }
}