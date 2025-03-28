using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Github
{
    public class DirectoryContent
    {
        public string Name { get; }
        public string Path { get; }
        public StringEnum<ContentType> Type { get; }
        public string? DownloadUrl { get; }
        public string DownloadStatus { get; set; }
        public double DownloadProgress { get; set; }

        public DirectoryContent(string name, string path, StringEnum<ContentType> type, string? downloadUrl)
        {
            Name = name;
            Path = path;
            Type = type;
            DownloadUrl = downloadUrl;
            DownloadStatus = "Not Started";
            DownloadProgress = 0.0;
        }
    }
}
