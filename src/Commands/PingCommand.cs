namespace codecrafters_redis;

public class PingCommand : ICommandHandler
{
    public string HandleCommand(string? argument)
    {
        return "+PONG\r\n";
    }
}