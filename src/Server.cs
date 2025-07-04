using codecrafters_redis;

Console.WriteLine("Logs from program starts here!");

var port = GetCommandPort(args);

foreach (var se in args)
{
    Console.WriteLine(se);
}

TcpListenerEventLoop server = new TcpListenerEventLoop(port);
await server.RunAsync();

int GetCommandPort(string[] strings)
{
    if (strings.Length > 0)
    {
        for (var i = 0; i < strings.Length; i++)
        {
            if (strings[i] == "--port" && i + 1 < strings.Length)
            {
                return int.Parse(strings[i + 1]);
            }
        }
    }

    return 6379;
}