using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TcpP43.Common;
using TcpP43.Common.Models;

namespace TcpP43.Server.Models;

public class ServerCore
{
    public string FileRoot { get; set; }

    public async Task HandleClientAsync(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();

        var commandType = await NetworkHelper.ReceiveStringAsync(stream);
        Console.WriteLine($"Received command: {commandType}");
        switch (commandType)
        {
            case CommandType.Auth:
                await HandleAuthCommandAsync(stream);
                break;
            case CommandType.GetFilesList:
                await HandleGetFilesListAsync(stream);
                break;
            case CommandType.DownloadFile:
                await HandleDownloadFileCommandAsync(stream);
                break;
            case CommandType.UploadFile:
                await HandleUploadFileCommandAsync(stream);
                break;
            default:
                // Handle unknown command
                break;
        }

        client.Close();
    }

    private async Task HandleUploadFileCommandAsync(NetworkStream stream)
    {
        // Implement the logic to handle the UploadFile command
        // For example, you can receive a file name and content from the client and save it to the FileRoot directory
        await NetworkHelper.ReceiveFileAsync(stream, FileRoot);
        Console.WriteLine($"Received and saved file.");
    }


    private async Task HandleDownloadFileCommandAsync(NetworkStream stream)
    {
        // Implement the logic to handle the DownloadFile command
        // For example, you can receive a file name from the client and send the file content
        var fileName = await NetworkHelper.ReceiveStringAsync(stream);
        var filePath = Path.Combine(FileRoot, fileName);
        if (File.Exists(filePath))
        {
            await NetworkHelper.SendFileAsync(stream, filePath);
            Console.WriteLine($"Sent file: {fileName}");
        }
        else
        {
            // TODO: Подумати як повідомити клієнту, що файл не знайдено
            // а клієнт міг це обробити і вивести повідомлення про помилку
        }
    }

    private async Task HandleGetFilesListAsync(NetworkStream stream)
    {
        // Implement the logic to handle the GetFileList command
        // For example, you can send a list of files from the FileRoot directory
        var files = Directory.GetFiles(FileRoot);
        Console.WriteLine($"Sending files list: {string.Join(", ", files)}");
        await NetworkHelper.SendObjectAsync(stream, files);
    }

    private async Task HandleAuthCommandAsync(NetworkStream stream)
    {
        var authRequest = await NetworkHelper.ReceiveObjectAsync<AuthRequest>(stream);

        Console.WriteLine($"Received auth request: Login={authRequest.Login}, Password={authRequest.Password}");

        var authResponse = new AuthResponse();
        
        var validLogin = "admin";
        var validPassword = "password";

        var isAuthenticated = authRequest.Login == validLogin && authRequest.Password == validPassword;



        if (isAuthenticated)
        {
            authResponse.IsAuthenticated = true;
            authResponse.Message = "Authentication successful.";
        }
        else
        {
            authResponse.IsAuthenticated = false;
            
            if (authRequest.Login != validLogin)
            {
                authResponse.Message = "Invalid login.";
            }
            else if (authRequest.Password != validPassword)
            {
                authResponse.Message = "Invalid password.";
            }
        }

        Console.WriteLine($"Sending auth response: IsAuthenticated={authResponse.IsAuthenticated}, Message={authResponse.Message}");

        await NetworkHelper.SendObjectAsync(stream, authResponse);
    }


}
