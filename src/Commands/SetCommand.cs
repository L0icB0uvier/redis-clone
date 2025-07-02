namespace codecrafters_redis;

public class SetCommand : ICommandHandler
{
    private readonly RedisStore _store;
    public SetCommand(RedisStore store)
    {
        _store = store;
    }
    
    public string HandleCommand(string[] arguments)
    {
        if(arguments.Length != 2)
            throw new ArgumentException("Set command should have 2 arguments");
        
        _store.Set(arguments[0], arguments[1]);
        
        return "+OK\r\n";
    }
}