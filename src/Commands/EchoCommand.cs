namespace codecrafters_redis;

public class EchoCommand : ICommandHandler
{
    public string HandleCommand(string? argument)
    {
        var sw = new StringWriter();
        var writer = new RespWriter(sw);
        writer.Write(new RespBulkString(argument));
        return sw.ToString();
    }
}