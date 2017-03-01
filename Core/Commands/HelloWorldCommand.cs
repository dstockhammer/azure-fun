using System;
using paramore.brighter.commandprocessor;

namespace AzureFun.Core.Commands
{
    public sealed class HelloWorldCommand : Command
    {
        public string Message { get; }

        public HelloWorldCommand(string message) : base(Guid.NewGuid())
        {
            Message = message;
        }
    }
}