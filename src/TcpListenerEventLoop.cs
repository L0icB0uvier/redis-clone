using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public class TcpListenerEventLoop
{
    private readonly TcpListener _listener;
    private readonly List<TcpClient> _clients = new();
    
    private readonly Dictionary<string, ICommandHandler> _commandHandlers = new()
    {
        {"ping", new PingCommand()},
        {"echo", new EchoCommand()}
    };

    public TcpListenerEventLoop(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public async Task RunAsync()
    {
        _listener.Start();
        Console.WriteLine("Server started, waiting for clients...");

        _ = AcceptClientsAsync();

        while (true)
        {
            TcpClient[] clientsCopy;
            lock (_clients)
            {
                clientsCopy = _clients.ToArray();
            }

            foreach (var client in clientsCopy)
            {
                if (!client.Connected)
                {
                    RemoveClient(client);
                    continue;
                }
                
                NetworkStream stream = client.GetStream();

                if (stream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    Console.WriteLine($"Received {bytesRead} bytes from {client.Client.RemoteEndPoint}");
                    try
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        RemoveClient(client);
                        continue;
                    }

                    if (bytesRead == 0)
                    {
                        RemoveClient(client);
                        continue;
                    }
                    
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(message);
                    using var reader = new StringReader(message);
                    var parser = new RespParser(reader);
                    var obj = parser.Parse();
                    
                    if (obj is not RespArray array)
                    {
                        throw new InvalidDataException(
                            $"Command should be an array object but was: {obj.GetType().Name}");
                    }

                    if (array.Elements[0] is not RespBulkString cmd)
                    {
                        throw new InvalidDataException(
                            $"Command should have a bulk string as first element but was: {array.Elements[0].GetType().Name}");
                    }

                    var commandName = cmd.Value.ToLower();

                    string? commandArg = null;
                    if (array.Elements.Count > 1)
                    {
                        if (array.Elements[1] is not RespBulkString arg)
                        {
                            throw new InvalidDataException(
                                $"Command should have a bulk string as second element but was: {array.Elements[1].GetType().Name}");
                        }

                        commandArg = arg.Value;
                    }
                    
                    if (_commandHandlers.ContainsKey(commandName) == false)
                    {
                        throw new InvalidOperationException($"Can't find command: {commandName}");
                    }
                    
                    var responseText = _commandHandlers[commandName.ToLower()].HandleCommand(commandArg);
                    Console.WriteLine($"Sending Response: {responseText}");
                    byte[] response = Encoding.UTF8.GetBytes(responseText);
                    await stream.WriteAsync(response, 0, response.Length);
                }

                await Task.Delay(10);
            }
        }
    }

    private async Task AcceptClientsAsync()
    {
        while (true)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

            lock (_clients)
            {
                _clients.Add(client);
            }
        }
    }

    private void RemoveClient(TcpClient client)
    {
        lock (_clients)
        {
            if (_clients.Remove(client))
            {
                Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}");
                client.Close();
            }
        }
    }
}