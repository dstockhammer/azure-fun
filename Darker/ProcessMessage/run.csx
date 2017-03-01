#r "Microsoft.ServiceBus"

using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Brighter.AzureExtensions.Functions;
using AzureFun.Core.Commands;

public static void Run(BrokeredMessage msg, TraceWriter log)
{
    var pipelineInvoker = new PipelineInvoker(s => log.Info(s), typeof(ProcessMessageCommand).Assembly);
    pipelineInvoker.Execute<ProcessMessageCommand>(msg);
}