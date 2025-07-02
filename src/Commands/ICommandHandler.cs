namespace codecrafters_redis;

public interface ICommandHandler
{
    public string HandleCommand(string[] arguments);
}