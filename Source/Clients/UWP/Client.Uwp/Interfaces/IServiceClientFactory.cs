namespace Andead.Chat.Client.Uwp
{
    public interface IServiceClientFactory
    {
        IAsyncServiceClient GetAsyncServiceClient();

        IServiceClient GetServiceClient();
    }
}