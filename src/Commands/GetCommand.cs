namespace codecrafters_redis;

public class GetCommand : ICommandHandler
{
    private readonly RedisStore _store;
    public GetCommand(RedisStore store)
    {
        _store = store;
    }
    public string HandleCommand(string[] argument)
    {
        if (argument.Length != 1)
            return "$-1\r\n";
        
        return _store.TryGet(argument[0], out var value)
            ? $"${value.Length}\r\n{value}\r\n" 
            : "$-1\r\n";
    }
}