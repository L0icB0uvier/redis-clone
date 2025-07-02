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
        switch (arguments.Length)
        {
            case 0:
            case 1:
                throw new ArgumentException("Set command should have minimum 2 arguments");
            case 2:
                _store.Set(arguments[0], arguments[1]);
                break;
            case 3:
                throw new ArgumentException("Set command should have 2 or 4 arguments, not 3");
            case 4:
                _store.Set(arguments[0], arguments[1], int.Parse(arguments[3]));
                break;
        }
        
        return "+OK\r\n";
    }
}