using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpP43.Common;
using TcpP43.Server.Models;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.WriteLine("Server");

int port = 5000;
IPAddress ipAddress = IPAddress.Parse("192.168.1.141");

TcpListener listener = new TcpListener(ipAddress, port);


listener.Start();
Console.WriteLine($"Server started on {ipAddress}:{port}");


var serverCore = new ServerCore()
{
    FileRoot = @"c:\demo\server"
};

while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();
    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
    _ = Task.Run(() => serverCore.HandleClientAsync(client));
}


