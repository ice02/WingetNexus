using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Entities
{
    public class ContentFiles
    {
        public int Id { get; set; }
        public string FileContent { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty; // YAML or Json
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty; // URL from github
        public string FileType { get; set; } = string.Empty; // Manifest Type
        public string FileVersion { get; set; } = string.Empty; // Manifest Version

        public string Checksum { get; set; } = string.Empty; // Checksum of the file
        public string RepositoryType { get; set; } = string.Empty; // Type of repository (e.g., Database, Mongo, CosmosDb, ...)
        public string UserCreated { get; set; } = string.Empty;
        public string UserLastModified { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public virtual Version? VersionContent { get; set; }
        public virtual Version? VersionContentForLocal { get; set; } // For Locales
        public virtual Version? VersionContentForInstaller { get; set; } // For Installers
        public virtual Version? VersionContentForDefaultLocale { get; set; } // For DefaultLocale
    }
}
