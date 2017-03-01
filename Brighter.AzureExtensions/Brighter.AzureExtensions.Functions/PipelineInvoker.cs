using System;
using Microsoft.ServiceBus.Messaging;
using paramore.brighter.commandprocessor;
using TinyIoC;

namespace Brighter.AzureExtensions.Functions
{
    public sealed class PipelineInvoker
    {
        // todo proper logger adapter
        private readonly Action<string> _log;

        public PipelineInvoker(Action<string> log)
        {
            _log = log;
        }

        public void Execute<TMessage, THandler, TMapper>(BrokeredMessage msg)
            where TMessage : class, IRequest
            where THandler : class, IHandleRequests<TMessage>
            where TMapper : class, IAmAMessageMapper<TMessage>
        {
            _log($"Invoking Brighter pipeline for {typeof(TMessage).Name} {msg.MessageId}");

            var messageId = Guid.Parse(msg.MessageId);
            var correlationId = Guid.Parse(msg.CorrelationId);
            var message = new Message(
                new MessageHeader(messageId, "topic todo", MessageType.MT_COMMAND, msg.EnqueuedTimeUtc, correlationId, null, msg.ContentType),
                new MessageBody(msg.GetBody<string>()));

            var container = TinyIoCContainer.Current;
            container.Register(_log); // todo

            container.Register<TMapper>();
            container.Register<THandler>();

            var messageMapper = container.Resolve<TMapper>();
            var handler = container.Resolve<THandler>();

            var command = messageMapper.MapToRequest(message);
            handler.Handle(command);
        }
    }
}