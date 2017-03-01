using AzureFun.Core.Commands;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Attributes;
using Serilog;

namespace AzureFun.Core.Handlers
{
    public sealed class HelloWorldCommandHandler : RequestHandler<HelloWorldCommand>
    {
        private readonly ILogger _logger;
        private readonly IAmACommandProcessor _commandProcessor;

        public HelloWorldCommandHandler(ILogger logger, IAmACommandProcessor commandProcessor)
        {
            _logger = logger;
            _commandProcessor = commandProcessor;
        }

        [RequestLogging(1, HandlerTiming.Before)]
        public override HelloWorldCommand Handle(HelloWorldCommand command)
        {
            _logger.Information($"Handling HelloWorldCommand {command.Id}");
            _logger.Information(command.Message);

            _commandProcessor.Post(new ProcessMessageCommand(command.Message));

            return base.Handle(command);
        }
    }
}