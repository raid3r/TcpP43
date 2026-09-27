using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpP43.Common;
using TcpP43.Common.Models;

namespace TcpP43.Client.Models;

public class Client
{
    private readonly IPAddress _serverIp;
    private readonly int _serverPort;

    public Client(IPAddress serverIp, int serverPort)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;
    }


    private async Task<TcpClient> ConnectAsync()
    {
        Console.WriteLine("Client");

        int port = _serverPort;
        IPAddress ipAddress = _serverIp;

        TcpClient client = new TcpClient();
        await client.ConnectAsync(ipAddress, port);

        Console.WriteLine($"Connected to server at {ipAddress}:{port}");

        return client;
    }

    public async Task<AuthResponse> AuthenticateAsync(string login, string password)
    {
        using TcpClient client = await ConnectAsync();
        using NetworkStream stream = client.GetStream();
        // Send command type
        await NetworkHelper.SendStringAsync(stream, CommandType.Auth);
        // Send authentication request
        var authRequest = new AuthRequest { Login = login, Password = password };
        await NetworkHelper.SendObjectAsync(stream, authRequest);
        // Receive authentication response
        var authResponse = await NetworkHelper.ReceiveObjectAsync<AuthResponse>(stream);
        return authResponse;
    }

    public async Task<string[]> GetFilesListAsync()
    {
        using TcpClient client = await ConnectAsync();
        using NetworkStream stream = client.GetStream();
        // Send command type
        await NetworkHelper.SendStringAsync(stream, CommandType.GetFilesList);
        // Receive files list
        var filesList = await NetworkHelper.ReceiveObjectAsync<string[]>(stream);
        return filesList;
    }

    public async Task DownloadFileAsync(string fileName, string destinationPath)
    {
        using TcpClient client = await ConnectAsync();
        using NetworkStream stream = client.GetStream();
        // Send command type
        await NetworkHelper.SendStringAsync(stream, CommandType.DownloadFile);
        // Send file name
        await NetworkHelper.SendStringAsync(stream, fileName);
        // Compute destination directory and file name
        var destDir = Path.GetDirectoryName(destinationPath);
        var destFileName = Path.GetFileName(destinationPath);
        if (string.IsNullOrEmpty(destDir))
            destDir = Directory.GetCurrentDirectory();
        // Receive file content and save to destination directory (pass file name)
        await NetworkHelper.ReceiveFileAsync(stream, destDir, string.IsNullOrEmpty(destFileName) ? null : destFileName);
    }

    public async Task UploadFileAsync(string filePath)
    {
        using TcpClient client = await ConnectAsync();
        using NetworkStream stream = client.GetStream();
        // Send command type
        await NetworkHelper.SendStringAsync(stream, CommandType.UploadFile);
        // Send file content (SendFileAsync уже отправляет имя и размер файла)
        await NetworkHelper.SendFileAsync(stream, filePath);
    }

}
