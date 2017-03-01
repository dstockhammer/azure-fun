#load "../Commands/HelloWorldCommand.csx"

using System;
using System.Threading.Tasks;
using paramore.brighter.commandprocessor;

public sealed class HelloWorldCommandHandler : RequestHandler<HelloWorldCommand>
{
    private readonly Action<string> _log;

    public HelloWorldCommandHandler(Action<string> log)
    {
        _log = log;
    }

    public override HelloWorldCommand Handle(HelloWorldCommand command)
    {
        _log($"Handling HelloWorldCommand {command.Id}");
        _log(command.Message);
        return base.Handle(command);
    }
}