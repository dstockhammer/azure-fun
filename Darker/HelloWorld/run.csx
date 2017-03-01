#r "Microsoft.ServiceBus"

using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Brighter.AzureExtensions.Functions;
using Brighter.MessagingGateway.AzureServiceBus;
using paramore.brighter.commandprocessor;
using AzureFun.Core.Commands;
using AzureFun.Core.Handlers;
using AzureFun.Core.MessageMappers;

public static void Run(BrokeredMessage msg, TraceWriter log)
{
    var messageMapperRegistry = new MessageMapperRegistry(new ActivatorFactory())
    {
        { typeof(ProcessMessageCommand), typeof(ProcessMessageCommandMessageMapper) }
    };

    var connectionString = System.Configuration.ConfigurationManager.AppSettings["Darker SB"];
    var producer = new AzureServiceBusMessageProducer(connectionString);
    var messagingConfiguration = new MessagingConfiguration(new InMemoryMessageStore(), producer, messageMapperRegistry);

    var pipelineInvoker = new PipelineInvoker(s => log.Info(s), messagingConfiguration);
    pipelineInvoker.Execute<HelloWorldCommand, HelloWorldCommandHandler, HelloWorldCommandMessageMapper>(msg);
}

class ActivatorFactory : IAmAMessageMapperFactory, IAmAHandlerFactory
{
    IAmAMessageMapper IAmAMessageMapperFactory.Create(Type messageMapperType)
    {
        return Activator.CreateInstance(messageMapperType) as IAmAMessageMapper;
    }
    IHandleRequests IAmAHandlerFactory.Create(Type handlerType)
    {
        return Activator.CreateInstance(handlerType) as IHandleRequests;
    }

    public void Release(IHandleRequests handler)
    {
    }
}