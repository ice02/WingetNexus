using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingetNexus.Data;
using WingetNexus.Shared.Models;

namespace WingetNexus.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NexusController : ControllerBase
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<NexusController> _logger;
        private readonly IConfiguration Configuration;
        private readonly IHostEnvironment env;

        public NexusController(WingetNexusContext context, ILogger<NexusController> logger, IConfiguration configuration, IHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            Configuration = configuration;
            this.env = env;
        }

        
    }
}
