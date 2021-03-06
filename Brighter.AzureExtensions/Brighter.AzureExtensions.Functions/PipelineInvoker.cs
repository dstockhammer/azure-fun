﻿using System;
using System.Reflection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using paramore.brighter.commandprocessor;
using Serilog;
using TinyIoC;

namespace Brighter.AzureExtensions.Functions
{
    public sealed class PipelineInvoker
    {
        private static HandlerConfig _handlerConfig;
        private static CommandProcessor _commandProcessor;
        private static readonly object _initialisationLock = new object();
        private static string _initialisedByRequestId;

        public PipelineInvoker(string requestId, TraceWriter traceWriter, Assembly coreAssembly, IAmAMessageProducer producer = null)
        {
            // todo: logger is static, but traceWriter is not. probably bad.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(new TraceWriterSink(traceWriter))
                .CreateLogger();

            // todo: again, logg provider is static, but traceWriter is not. probably bad.
            // yep, turns out this doesn't work at all
            //LogProvider.SetCurrentLogProvider(new TraceWriterLogProvider(log));

            TinyIoCContainer.Current.Register(Log.Logger);

            if (_handlerConfig == null)
            {
                lock (_initialisationLock)
                {
                    if (_handlerConfig == null)
                    {
                        Log.Logger.Information("Initialising ({requestId})...", requestId);
                        _initialisedByRequestId = requestId;

                        _handlerConfig = new HandlerConfig();
                        _handlerConfig.RegisterDefaultHandlers();
                        _handlerConfig.RegisterMappersFromAssembly(coreAssembly);
                        _handlerConfig.RegisterSubscribersFromAssembly(coreAssembly);

                        if (producer != null)
                        {
                            _commandProcessor = CommandProcessorBuilder.With()
                                .Handlers(_handlerConfig.HandlerConfiguration)
                                .DefaultPolicy()
                                .TaskQueues(new MessagingConfiguration(new InMemoryMessageStore(), producer, _handlerConfig.MessageMapperRegistry))
                                .RequestContextFactory(new InMemoryRequestContextFactory())
                                .Build();
                        }
                        else
                        {
                            _commandProcessor = CommandProcessorBuilder.With()
                                .Handlers(_handlerConfig.HandlerConfiguration)
                                .DefaultPolicy()
                                .NoTaskQueues()
                                .RequestContextFactory(new InMemoryRequestContextFactory())
                                .Build();
                        }

                        TinyIoCContainer.Current.Register<IAmACommandProcessor>(_commandProcessor);
                    }
                }
            }
            else
            {
                Log.Logger.Information("Already initialised by {requestId}", _initialisedByRequestId);
            }
        }

        public void Execute<TRequest>(BrokeredMessage msg)
            where TRequest : class, IRequest
        {
            Log.Logger.Information("Invoking Brighter pipeline for {request} {id}", typeof(TRequest).Name, msg.MessageId);

            var messageId = Guid.Parse(msg.MessageId);
            var correlationId = Guid.Parse(msg.CorrelationId);
            var message = new Message(
                new MessageHeader(messageId, "topic todo", MessageType.MT_COMMAND, msg.EnqueuedTimeUtc, correlationId, null, msg.ContentType),
                new MessageBody(msg.GetBody<string>()));

            var messageMapper = _handlerConfig.MessageMapperRegistry.Get<TRequest>();
            var request = messageMapper.MapToRequest(message);

            DispatchRequest(message.Header, request);
        }

        private static void DispatchRequest<TRequest>(MessageHeader messageHeader, TRequest request)
            where TRequest : class, IRequest
        {
            if (messageHeader.MessageType == MessageType.MT_COMMAND && request is IEvent)
                throw new ConfigurationException($"Message {request.Id} mismatch. Message type is '{MessageType.MT_COMMAND}' yet mapper produced message of type IEvent");

            if (messageHeader.MessageType == MessageType.MT_EVENT && request is ICommand)
                throw new ConfigurationException($"Message {request.Id} mismatch. Message type is '{MessageType.MT_EVENT}' yet mapper produced message of type ICommand");

            switch (messageHeader.MessageType)
            {
                case MessageType.MT_COMMAND:
                    _commandProcessor.Send(request);
                    break;
                    
                case MessageType.MT_DOCUMENT:
                case MessageType.MT_EVENT:
                    _commandProcessor.Publish(request);
                    break;
            }
        }
    }
}