using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpP43.Client.Models;
using TcpP43.Common;
using TcpP43.Common.Models;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;


var serverIp = "192.168.1.141";
var serverPort = 5000;
var client = new Client(IPAddress.Parse(serverIp), serverPort);

Console.WriteLine("Client");

var isAuthenticated = false;

do
{
    Console.Write($"Login: ");
    var login = Console.ReadLine();

    Console.Write($"Password: ");
    var password = Console.ReadLine();

    var authResponse = await client.AuthenticateAsync(login, password);
    isAuthenticated = authResponse.IsAuthenticated;

    if (isAuthenticated)
    {
        Console.WriteLine(authResponse.Message);
    }
    else
    {
        Console.WriteLine($"Authentication failed: {authResponse.Message}");
    } 

} while (!isAuthenticated);

var isRunning = true;

do
{
    // show menu
    Console.WriteLine("Menu:");
    Console.WriteLine("1. Get files list");
    Console.WriteLine("2. Download file");
    Console.WriteLine("3. Upload file");
    Console.WriteLine("5. Exit");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            var filesList = await client.GetFilesListAsync();
            Console.WriteLine("Files list:");
            foreach (var file in filesList)
            {
                Console.WriteLine(file);
            }
            break;
        case "2":
            
                var downloadFilesRoot = @"c:\demo\client";

                // get all files list
                var downloadFilesList = await client.GetFilesListAsync();
                // show menu with files list
                Console.WriteLine("Files list:");
                for (int i = 0; i < downloadFilesList.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {downloadFilesList[i]}");
                }
                // user selects file to download
                Console.Write("Enter the number of the file to download: ");
                if (int.TryParse(Console.ReadLine(), out int fileNumber) && fileNumber >= 1 && fileNumber <= downloadFilesList.Length)
                {
                    var fileName = Path.GetFileName(downloadFilesList[fileNumber - 1]);
                    var destinationPath = Path.Combine(downloadFilesRoot, fileName);
                    // download file
                    await client.DownloadFileAsync(fileName, destinationPath);
                    Console.WriteLine($"File '{fileName}' downloaded to '{destinationPath}'.");
                }
                else
                {
                    Console.WriteLine("Invalid file number.");
                }

            
            break;
        case "3":
            var uploadFilesRoot = @"c:\demo\client";
            // get all files list from local folder
            var uploadFilesList = Directory.GetFiles(uploadFilesRoot);
            // show menu with files list
            Console.WriteLine("Files list:");
            for (int i = 0; i < uploadFilesList.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(uploadFilesList[i])}");
            }
            // user selects file to upload
            Console.Write("Enter the number of the file to upload: ");
            if (int.TryParse(Console.ReadLine(), out int uploadFileNumber) && uploadFileNumber >= 1 && uploadFileNumber <= uploadFilesList.Length)
            {
                var filePath = uploadFilesList[uploadFileNumber - 1];
                var fileName = Path.GetFileName(filePath);
                // upload file
                await client.UploadFileAsync(filePath);
                Console.WriteLine($"File '{fileName}' uploaded successfully.");
            }
            else
            {
                Console.WriteLine("Invalid file number.");
            }
            break;


        case "5":
            isRunning = false;
            break;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }

} while (isRunning);

