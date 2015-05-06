namespace Kirnau.Survey.Workers.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Storage;
    using Moq;
    using Kirnau.Survey.Workers.Commands;
    using Kirnau.Survey.Workers.QueueHandlers;
    using Survey.Web.Shared.Stores.AzureStorage;
    using Microsoft.WindowsAzure.Storage.Queue;

    [TestClass]
    public class QueueHandlerFixture
    {
        [TestMethod]
        public void ForCreatesHandlerForGivenQueue()
        {
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();

            var queueHandler = QueueHandler.For(mockQueue.Object);

            Assert.IsInstanceOfType(queueHandler, typeof(QueueHandler<MessageStub>));
        }

        [TestMethod]
        public void EveryReturnsSameHandlerForGivenQueue()
        {
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            var queueHandler = new QueueHandlerStub(mockQueue.Object);

            var returnedQueueHandler = queueHandler.Every(TimeSpan.Zero);

            Assert.AreSame(queueHandler, returnedQueueHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForThrowsWhenQueueIsNull()
        {
            QueueHandler.For(default(IAzureQueue<MessageStub>));
        }

        [TestMethod]
        public void DoRunsGivenCommandForEachMessage()
        {
            var message1 = new MessageStub();
            var message2 = new MessageStub();
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            mockQueue.Setup(q => q.GetMessages(1)).Returns(() => new[] { message1, message2 });
            var command = new Mock<ICommand<MessageStub>>();
            var queueHandler = new QueueHandlerStub(mockQueue.Object);

            queueHandler.Do(command.Object);

            command.Verify(c => c.Run(It.IsAny<MessageStub>()), Times.Exactly(2));
            command.Verify(c => c.Run(message1));
            command.Verify(c => c.Run(message2));
        }

        [TestMethod]
        public void DoDeletesMessageWhenRunIsSuccessfull()
        {
            var message = new MessageStub();
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            mockQueue.Setup(q => q.GetMessages(1)).Returns(() => new[] { message });
            var command = new Mock<ICommand<MessageStub>>();
            command.Setup(c => c.Run(It.IsAny<MessageStub>())).Returns(true);
            var queueHandler = new QueueHandlerStub(mockQueue.Object);

            queueHandler.Do(command.Object);

            mockQueue.Verify(q => q.DeleteMessage(message));
        }

        [TestMethod]
        [Ignore]//Hieu: should not run this time
        public void DoDeletesMessageWhenRunIsNotSuccessfullAndMessageHasBeenDequeuedMoreThanFiveTimes()
        {  
            var message = new MessageStub();
            message.SetMessageReference(new CloudQueueMessageStub(string.Empty).QueueMessage);
            var mockQueue = new Mock<IAzureQueue<MessageStub>>();
            mockQueue.Setup(q => q.GetMessages(1)).Returns(() => new[] { message });
            var command = new Mock<ICommand<MessageStub>>();
            command.Setup(c => c.Run(It.IsAny<MessageStub>())).Throws(new Exception("This will cause the command to fail"));
            var queueHandler = new QueueHandlerStub(mockQueue.Object);

            queueHandler.Do(command.Object);
            //Hieu: since DequeueCount is readonly properties, have no solution so far for dequeued more than 5 times.
            mockQueue.Verify(q => q.DeleteMessage(message));
        }

        public class MessageStub : AzureQueueMessage
        {
        }
       
        public class CloudQueueMessageStub
        {
            public CloudQueueMessage QueueMessage { get; set; }
            public CloudQueueMessageStub(string content)
            {
                this.QueueMessage = new CloudQueueMessage(content);
                //this.QueueMessage.DequeueCount = 6;
            }
        }

        private class QueueHandlerStub : QueueHandler<MessageStub>
        {
            public QueueHandlerStub(IAzureQueue<MessageStub> queue)
                : base(queue)
            {
            }

            public override void Do(ICommand<MessageStub> batchCommand)
            {
                this.Cycle(batchCommand);
            }
        }
    }
}