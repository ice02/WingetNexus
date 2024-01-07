using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db.Objects
{
    public class NestedInstallerFile
    {
        public int Id { get; set; }
        [JsonIgnore]
        public virtual Installer Installer { get; set; }
        [ForeignKey("Installer")]
        public int InstallerId { get; set; }

        public string RelativeFilePath { get; set; }
        public string PortableCommandAlias { get; set; }
    }
}
