using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public class TcpListenerEventLoop
{
    private readonly TcpListener _listener;
    private readonly List<TcpClient> _clients = new();

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
                    Console.WriteLine($"Received {bytesRead} bytes from {client.Client.RemoteEndPoint}");
                    
                    byte[] response = Encoding.UTF8.GetBytes("+PONG\r\n");
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