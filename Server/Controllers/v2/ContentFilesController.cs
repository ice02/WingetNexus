using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WingetNexus.Data.DataStores;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Server.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "IsAuthorized", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ContentFilesController : ControllerBase
    {
        private readonly ILogger<ContentFilesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IContentFilesDataStore _contentFilesDataStore;

        public ContentFilesController(
                                      ILogger<ContentFilesController> logger,
                                      IConfiguration configuration,
                                        IContentFilesDataStore contentFilesDataStore
                                      )
        {
            _logger = logger;
            _configuration = configuration;
            _contentFilesDataStore = contentFilesDataStore;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var contentFile = _contentFilesDataStore.GetContentFileByIdAsync(id);

            if (contentFile == null)
            {
                return NotFound();
            }

            return Ok(contentFile);
        }
    }
}
