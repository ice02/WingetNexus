using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Db;

namespace WingetNexus.Shared.Models.Dtos
{
    public class NestedInstallerFileDto
    {
        public int Id { get; set; }
        public int InstallerId { get; set; }

        public string RelativeFilePath { get; set; }
        public string PortableCommandAlias { get; set; }
    }
}
