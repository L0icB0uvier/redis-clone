namespace codecrafters_redis;

public class RedisServerInfo
{
    public int Port { get; set; }
    public string Role { get; set; }

    public RedisServerInfo(int port = 6379, string role = "master")
    {
        Role = role;
    }
}