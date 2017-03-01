#load "../Commands/HelloWorldCommand.csx"

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using paramore.brighter.commandprocessor;

public sealed class HelloWorldCommandMessageMapper : IAmAMessageMapper<HelloWorldCommand>
{
    public Message MapToMessage(HelloWorldCommand request)
    {
        return new Message(
            new MessageHeader(request.Id, "brighter-helloworld", MessageType.MT_COMMAND),
            new MessageBody(JsonConvert.SerializeObject(request)));
    }

    public HelloWorldCommand MapToRequest(Message message)
    {
        return JsonConvert.DeserializeObject<HelloWorldCommand>(message.Body.Value);
    }
}