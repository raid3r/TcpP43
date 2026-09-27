using System;
using System.Collections.Generic;
using System.Text;

namespace TcpP43.Common.Models;

public static class CommandType
{
    public const string Auth = "Auth";

    public const string GetFilesList = "GetFilesList";

    public const string DownloadFile = "DownloadFile";

    public const string UploadFile = "UploadFile";
}
