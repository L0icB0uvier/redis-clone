using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();

var socket = server.AcceptSocket(); // wait for client
while (true)
{
    byte[] buffer = new byte[1024];
    int bytesReceived = socket.Receive(buffer);

    if (bytesReceived == 0)
        break;
    
    socket.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
}

try
{
    socket.Shutdown(SocketShutdown.Both);
}
catch (SocketException) 
{
    // Ignore if already closed
}

socket.Close();