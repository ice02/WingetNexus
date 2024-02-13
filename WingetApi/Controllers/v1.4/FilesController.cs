using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WingetNexus.Shared.Helpers;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.WingetApi.Services;

namespace WingetNexus.WingetApi.Controllers.v1._4
{
    [Route("api/v1.4/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<FilesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;

        public FilesController(IHostEnvironment env,
                ILogger<FilesController> logger, 
                IConfiguration configuration,
                IStorageService storageService
            )
        {
            _env = env;
            _logger = logger;
            _configuration = configuration;
            _storageService = storageService;
        }

        [HttpGet("{filename}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string filename)
        {
            if (filename == null)
            {
                   return NoContent();
            }

            var file = await _storageService.GetAsync(filename);

            return File(file, "application/octet-stream");
        }
    }
}
