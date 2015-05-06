namespace Kirnau.Survey.Workers.Commands
{
    using Kirnau.Survey.Web.Shared.Stores.AzureStorage;

    public interface IBatchCommand<in T> : ICommand<T> where T : AzureQueueMessage
    {
        void PreRun();
        void PostRun();
    }
}