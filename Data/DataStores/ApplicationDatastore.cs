using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Data.Models;

namespace WingetNexus.Data.DataStores
{
    public class ApplicationDatastore : IApplicationDatastore
    {
        private readonly ILogger<WingetNexusDataStore> _logger;
        private readonly IConfiguration _configuration;
        private AppDbContext _context;

        public ApplicationDatastore(ILogger<WingetNexusDataStore> logger, IConfiguration configuration, AppDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public List<string> GetUserAcl(string userId)
        {
            //TODO get user roles from db
            return new List<string>()
            {
                "admin"
            };
        }

        public async Task<bool> SetUserAcl(string userId, string role)
        {
            var user = _context.UserRoles.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                user = new Data.Models.UserRole()
                {
                    UserId = userId,
                    RoleName = role
                };

                _context.UserRoles.Add(user);
            }
            else
            {
                user.RoleName = role;
            }

            await _context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public IQueryable<UserRole> GetAllUsers()
        {
            return _context.UserRoles.AsQueryable();
        }
    }
}
