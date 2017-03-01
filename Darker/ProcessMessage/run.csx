#r "Microsoft.ServiceBus"

using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Brighter.AzureExtensions.Functions;
using AzureFun.Core.Commands;
using AzureFun.Core.Handlers;
using AzureFun.Core.MessageMappers;

public static void Run(BrokeredMessage msg, TraceWriter log)
{
    var pipelineInvoker = new PipelineInvoker(s => log.Info(s));
    pipelineInvoker.Execute<ProcessMessageCommand, ProcessMessageCommandHandler, ProcessMessageCommandMessageMapper>(msg);
}