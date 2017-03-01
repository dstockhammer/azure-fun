using System;
using paramore.brighter.commandprocessor;

namespace AzureFun.Core.Commands
{
    public sealed class ProcessMessageCommand : Command
    {
        public string Message { get; }

        public ProcessMessageCommand(string message) : base(Guid.NewGuid())
        {
            Message = message;
        }
    }
}