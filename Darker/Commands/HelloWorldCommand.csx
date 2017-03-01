using paramore.brighter.commandprocessor;

public sealed class HelloWorldCommand : Command
{
    public string Message { get; }

    public HelloWorldCommand(string message) : base(Guid.NewGuid())
    {
        Message = message;
    }
}