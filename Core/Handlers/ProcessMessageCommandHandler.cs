using System;
using AzureFun.Core.Commands;
using paramore.brighter.commandprocessor;

namespace AzureFun.Core.Handlers
{
    public sealed class ProcessMessageCommandHandler : RequestHandler<ProcessMessageCommand>
    {
        private readonly Action<string> _log;

        public ProcessMessageCommandHandler(Action<string> log)
        {
            _log = log;
        }

        public override ProcessMessageCommand Handle(ProcessMessageCommand command)
        {
            _log($"Handling ProcessMessageCommand {command.Id}");
            _log(command.Message);

            return base.Handle(command);
        }
    }
}