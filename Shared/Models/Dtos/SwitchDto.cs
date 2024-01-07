using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Dtos
{
    public class SwitchDto
    {
        public int Id { get; set; }
        public int InstallerId { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
    }
}
