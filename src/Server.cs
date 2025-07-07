using codecrafters_redis;

Console.WriteLine("Logs from program starts here!");
Dictionary<string, List<string>> arguments = new(); 

Console.WriteLine("Arguments:");
foreach (var se in args)
{
    Console.WriteLine(se);
}

ParseArguments(args);

RedisServerInfo serverInfo = new();
serverInfo.Role = arguments.ContainsKey("replicaof")? "slave" : "master";
serverInfo.Port = GetCommandPort();

TcpListenerEventLoop server = new TcpListenerEventLoop(serverInfo);
await server.RunAsync();

void ParseArguments(string[] args)
{
    string argumentName = "";
    foreach (var arg in args)
    {
        if (arg.StartsWith("--"))
        {
            argumentName = arg.Substring(2);
            arguments.Add(argumentName, new List<string>());
        }
        
        else
        {
            arguments[argumentName].Add(arg);
        }
    }
}

int GetCommandPort()
{
    if(arguments.ContainsKey("port") == false || arguments["port"].Count == 0) return 6379;
    
    return int.Parse(arguments["port"][0]);
}