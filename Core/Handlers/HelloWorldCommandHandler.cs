using System;
using AzureFun.Core.Commands;
using paramore.brighter.commandprocessor;

namespace AzureFun.Core.Handlers
{
    public sealed class HelloWorldCommandHandler : RequestHandler<HelloWorldCommand>
    {
        private readonly Action<string> _log;
        private readonly IAmACommandProcessor _commandProcessor;

        public HelloWorldCommandHandler(Action<string> log, IAmACommandProcessor commandProcessor)
        {
            _log = log;
            _commandProcessor = commandProcessor;
        }

        public override HelloWorldCommand Handle(HelloWorldCommand command)
        {
            _log($"Handling HelloWorldCommand {command.Id}");
            _log(command.Message);

            _commandProcessor.Post(new ProcessMessageCommand(command.Message));

            return base.Handle(command);
        }
    }
}