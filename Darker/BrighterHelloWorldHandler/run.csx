#r "Microsoft.ServiceBus"

#load "../Commands/HelloWorldCommand.csx"
#load "HelloWorldCommandHandler.csx"
#load "HelloWorldCommandMessageMapper.csx"

using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Brighter.AzureExtensions.Functions;

public static void Run(BrokeredMessage msg, TraceWriter log)
{
    var pipelineInvoker = new PipelineInvoker(s => log.Info(s));
    pipelineInvoker.Execute<HelloWorldCommand, HelloWorldCommandHandler, HelloWorldCommandMessageMapper>(msg);
}