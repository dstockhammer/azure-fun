using AzureFun.Core.Commands;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Attributes;
using Serilog;

namespace AzureFun.Core.Handlers
{
    public sealed class ProcessMessageCommandHandler : RequestHandler<ProcessMessageCommand>
    {
        private readonly ILogger _logger;

        public ProcessMessageCommandHandler(ILogger logger)
        {
            _logger = logger;
        }

        [RequestLogging(1, HandlerTiming.Before)]
        public override ProcessMessageCommand Handle(ProcessMessageCommand command)
        {
            _logger.Information($"Handling ProcessMessageCommand {command.Id}");
            _logger.Information(command.Message);

            return base.Handle(command);
        }
    }
}