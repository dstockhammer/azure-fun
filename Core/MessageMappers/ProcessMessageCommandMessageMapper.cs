using AzureFun.Core.Commands;
using Newtonsoft.Json;
using paramore.brighter.commandprocessor;

namespace AzureFun.Core.MessageMappers
{
    public sealed class ProcessMessageCommandMessageMapper : IAmAMessageMapper<ProcessMessageCommand>
    {
        public Message MapToMessage(ProcessMessageCommand request)
        {
            return new Message(
                new MessageHeader(request.Id, "brighter-processmessage", MessageType.MT_COMMAND),
                new MessageBody(JsonConvert.SerializeObject(request)));
        }

        public ProcessMessageCommand MapToRequest(Message message)
        {
            return JsonConvert.DeserializeObject<ProcessMessageCommand>(message.Body.Value);
        }
    }
}