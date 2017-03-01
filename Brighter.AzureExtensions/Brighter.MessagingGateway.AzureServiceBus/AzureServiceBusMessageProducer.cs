using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.Logging;

namespace Brighter.MessagingGateway.AzureServiceBus
{
    public class AzureServiceBusMessageProducer : IAmAMessageProducerSupportingDelay
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(LogProvider.For<AzureServiceBusMessageProducer>);

        private readonly NamespaceManager _namespaceManager;
        private readonly string _connectionString;

        public bool DelaySupported => true;

        public AzureServiceBusMessageProducer(string connectionString)
        {
            _connectionString = connectionString;
            _namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
        }

        public void Send(Message message)
        {
            EnsureTopicExists(message.Header.Topic);

            var brokeredMessage = BuildBrokeredMessage(message);

            TopicClient
                .CreateFromConnectionString(_connectionString, message.Header.Topic)
                .Send(brokeredMessage);

            _logger.Value.DebugFormat("Published message {0} to topic {1}.",
                message.Id, message.Header.Topic);
        }

        public void SendWithDelay(Message message, int delayMilliseconds = 0)
        {
            EnsureTopicExists(message.Header.Topic);

            var brokeredMessage = BuildBrokeredMessage(message);
            var scheduledEnqueueTime = DateTime.UtcNow.AddMilliseconds(delayMilliseconds);

            TopicClient
                .CreateFromConnectionString(_connectionString, message.Header.Topic)
                .ScheduleMessageAsync(brokeredMessage, scheduledEnqueueTime);

            _logger.Value.DebugFormat("Published message {0} with delay of {1} ms to topic {2}.",
                message.Id, delayMilliseconds, message.Header.Topic);
        }

        public void Dispose()
        {
            // nothing
        }

        private void EnsureTopicExists(string topic)
        {
            if (!_namespaceManager.TopicExists(topic))
            {
                _logger.Value.InfoFormat("Topic {0} does not exist, creating it.");
                _namespaceManager.CreateTopic(topic);
            }
        }

        private static BrokeredMessage BuildBrokeredMessage(Message message)
        {
            return new BrokeredMessage(message.Body.Value)
            {
                MessageId = message.Header.Id.ToString(),
                CorrelationId = message.Header.CorrelationId.ToString(),
                ContentType = message.Header.ContentType
            };
        }
    }
}
