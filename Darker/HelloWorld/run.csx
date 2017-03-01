#r "Microsoft.ServiceBus"

using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Brighter.AzureExtensions.Functions;
using Brighter.MessagingGateway.AzureServiceBus;
using AzureFun.Core.Commands;

public static void Run(BrokeredMessage msg, TraceWriter log)
{
    var connectionString = System.Configuration.ConfigurationManager.AppSettings["Darker SB"];
    var producer = new AzureServiceBusMessageProducer(connectionString);

    var pipelineInvoker = new PipelineInvoker(s => log.Info(s), typeof(HelloWorldCommand).Assembly, producer);
    pipelineInvoker.Execute<HelloWorldCommand>(msg);
}