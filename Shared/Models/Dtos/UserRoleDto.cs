using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Dtos
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }
        public string? UserId { get; set; }
    }
}
