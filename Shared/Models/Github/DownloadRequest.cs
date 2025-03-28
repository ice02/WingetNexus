using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Github
{
    public class DownloadRequest
    {
        public string FilePath { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Application { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }

    public class DownloadResponse
    {
        public string FilePath { get; set; } = string.Empty;
    }
}
