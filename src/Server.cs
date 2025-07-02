using codecrafters_redis;

Console.WriteLine("Logs from program starts here!");

TcpListenerEventLoop server = new TcpListenerEventLoop(6379);
await server.RunAsync();