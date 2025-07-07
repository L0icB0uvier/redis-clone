namespace codecrafters_redis;

public class InfoCommand : ICommandHandler
{
    private readonly RedisServerInfo _serverInfo;
    public string HandleCommand(string[] arguments)
    {
        List<InfoEntry> infoEntries = new();
        
        if (arguments[0] == "replication")
        {
            InfoEntry roleKey = new InfoEntry("role", _serverInfo.Role);
            infoEntries.Add(roleKey);
        }

        string infoText = infoEntries[0].GetInfo();
        var sw = new StringWriter();
        var writer = new RespWriter(sw);
        writer.Write(new RespBulkString(infoText));
        return sw.ToString();
    }
    
    public InfoCommand(RedisServerInfo serverInfo)
    {
        _serverInfo = serverInfo;
    }
}

public class InfoEntry
{
    public string Key { get; set; }
    public string Value { get; set; }

    public InfoEntry(string key)
    {
        Key = key;
        Value = "";  
    }

    public InfoEntry(string key, string value)
    {
        Key = key;
        Value = value;   
    }

    public void SetValue(string value)
    {
        Value = value;   
    }

    public string GetInfo()
    {
        return $"{Key}:{Value}";
    }
}