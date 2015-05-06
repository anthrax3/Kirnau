namespace Kirnau.Survey.Workers.QueueHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Kirnau.Survey.Web.Shared.Helpers;
    using Kirnau.Survey.Web.Shared.Stores.AzureStorage;

    public abstract class GenericQueueHandler<T> where T : AzureQueueMessage
    {
        protected static void ProcessMessages(IAzureQueue<T> queue, IEnumerable<T> messages, Func<T, bool> action)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            foreach (var message in messages)
            {
                var allowDelete = false;
                var corruptMessage = false;

                try
                {
                    allowDelete = action(message);
                }
                catch (Exception ex)
                {
                    TraceHelper.TraceError(ex.TraceInformation());
                    corruptMessage = true;
                }
                finally
                {
                    if (allowDelete || (corruptMessage && message.GetMessageReference().DequeueCount > 5))
                    {
                        try
                        {
                            queue.DeleteMessage(message);
                        }
                        catch (Exception ex)
                        {
                            TraceHelper.TraceError(
                                "Error deleting message type '{0}' id {1}: {2}", 
                                message.GetType().Name,
                                message.GetMessageReference().Id,
                                ex.TraceInformation());
                        }
                    }
                }
            }
        }

        protected virtual void Sleep(TimeSpan interval)
        {
            Thread.Sleep(interval);
        }
    }
}