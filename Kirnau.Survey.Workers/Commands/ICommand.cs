namespace Kirnau.Survey.Workers.Commands
{
    using Kirnau.Survey.Web.Shared.Stores.AzureStorage;

    public interface ICommand<in T> where T : AzureQueueMessage
    {
        bool Run(T message);
    }
}