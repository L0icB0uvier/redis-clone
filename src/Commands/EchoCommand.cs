namespace codecrafters_redis;

public class EchoCommand : ICommandHandler
{
    public string HandleCommand(string[] arguments)
    {
        string arg = "";

        if (arguments.Length == 1)
        {
            arg = arguments[0];
        }
        
        else if (arguments.Length > 1)
        {
            arg = string.Join(" ", arguments);
        }
        
        var sw = new StringWriter();
        var writer = new RespWriter(sw);
        writer.Write(new RespBulkString(arg));
        return sw.ToString();
    }
}