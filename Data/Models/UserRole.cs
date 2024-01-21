﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Data.Models
{
    public class UserRole
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string? RoleName { get; set; }
        public string? UserId { get; set; }
    }
}
