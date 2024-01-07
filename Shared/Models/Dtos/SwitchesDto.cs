using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Dtos
{
    public class SwitchesDto
    {
        public bool IsSilent { get; set; } = false;
        public bool IsSilentWithProgress { get; set; } = false;
        public bool IsInteractive { get; set; } = false;
        public bool IsInstallLocation { get; set; } = false;
        public bool IsLog { get; set; } = false;
        public bool IsUpgrade { get; set; } = false;
        public bool IsCustom { get; set; } = false;

        public string InstallLocation { get; set; } = "Default value";
    }
}
