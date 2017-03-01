#r "Microsoft.ServiceBus"

using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using paramore.brighter.commandprocessor;

public sealed class HelloWorldCommand : Command
{
    public string Message { get; }

    public HelloWorldCommand(string message) : base(Guid.NewGuid())
    {
        Message = message;
    }
}

public static void Run(BrokeredMessage msg, TraceWriter log)
{
    log.Info($"C# ServiceBus queue trigger function processed message: {msg.MessageId}");

    var command = JsonConvert.DeserializeObject<HelloWorldCommand>(msg.GetBody<string>());
    log.Info($"Message: {command.Message}");
}