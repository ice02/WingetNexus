using WingetNexus.Data.DataStores;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using Version = WingetNexus.Shared.Models.Entities.Version;

namespace WingetNexus.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "IsAuthorized", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class PackagesController : ControllerBase
    {
        private readonly ILogger<PackagesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IWingetAppDatastore _dataStore;

        public PackagesController(
                                  ILogger<PackagesController> logger, 
                                  IConfiguration configuration, 
                                  IHostEnvironment env, 
                                  IWingetAppDatastore dataStore)
        {
            _logger = logger;
            _configuration = configuration;
            _env = env;
            _dataStore = dataStore;
        }

        /// <summary>
        /// Get all packages with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IQueryable<ApplicationDto>>> Get(
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] string? filter,
            [FromQuery] string? orderby,
            [FromQuery] string? orderway)
        {
            var filterDto = new FilterDto()
            {
                PageNumber = page,
                PageSize = pageSize,
                Filter = filter,
                OrderBy = orderby,
                OrderWay = orderway
            };

            var result = await _dataStore.GetAllApplicationsAsync(filterDto);
            var packageCnt = await _dataStore.GetApplicationCountAsync(filter);

            Response.Headers.Append("X-Total-Count", packageCnt.ToString());

            return Ok(result);
        }

        // get all package count based on a filter
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetTotalCount()
        {
            var packageCnt = await _dataStore.GetApplicationCountAsync();

            return Ok(packageCnt);
        }

        /// <summary>
        /// Get full package details by identifier
        /// </summary>
        /// <param name="packageIdentifier"></param>
        /// <returns></returns>
        [HttpGet("{packageIdentifier}")]
        //[Authorize]
        //[Authorize(Policy = "get:package")]
        public async Task<ActionResult<ApplicationDto>> GetPackage(string packageIdentifier)
        {
            var result = await _dataStore.GetApplicationByPackageIdentifierAsync(packageIdentifier);

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageForm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApplicationDto>> Post([FromBody] ApplicationDto packageForm)
        {
            var hasValidationErrors = false;
            string validationErrors = "";

            ValidateCreateForm(packageForm, ref hasValidationErrors, ref validationErrors);

            if (hasValidationErrors)
            {
                return StatusCode(500, validationErrors);
            }

            if (string.IsNullOrEmpty(packageForm.PackageIdentifier))
            {
                packageForm.PackageIdentifier = $"{packageForm.Publisher.Name.Replace(" ", "")}.{packageForm.Name.Replace(" ", "")}";
            }

            if (await _dataStore.GetApplicationByPackageIdentifierAsync(packageForm.PackageIdentifier) != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Package identifier must be unique");
            }

            var newApp = await _dataStore.CreateApplicationAsync(packageForm);

            //TODO: if requested, get binaries from all installer urls and download them to the server based on repo configured

            return Ok(newApp);
        }

        



        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageForm"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        //[HttpPut("{identifier}/infos")]
        //public async Task<ActionResult<VersionDto>> Put([FromBody] NewPackageDto packageForm, string identifier)
        //{
        //    var hasValidationErrors = false;
        //    string validationErrors = "";
        //    Package package = null;

        //    //ValidateCreateForm(packageForm, ref hasValidationErrors, ref validationErrors);

        //    if (string.IsNullOrEmpty(identifier))
        //    {
        //        hasValidationErrors = true;
        //        validationErrors += "Package identifier is missing";
        //    }

        //    if (hasValidationErrors)
        //    {
        //        return StatusCode(500, validationErrors);
        //    }

        //    try
        //    {
        //        package = _context.Packages.FirstOrDefault(p => p.Identifier == identifier);
        //        if (package == null)
        //        {
        //            return StatusCode(204, "Package not found");
        //        }

        //        package.Name = packageForm.Name;
        //        package.Publisher = packageForm.Publisher;

        //        await _context.SaveChangesAsync();
        //        _logger.LogDebug("Updated to db");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogDebug($"Error committing to the database: {e}");
        //        return StatusCode(500, "Database error");
        //    }

        //    return Ok(package);
        //}

        /// <summary>
        /// Delete specified package with cascading delete
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [HttpDelete("{packageID}")]
        //[Authorize]
        public ActionResult DeletePackage(string packageID)
        {
            _dataStore.DeleteApplicationAsync(packageID);

            return NoContent();
        }

        private void ValidateUpdateForm(ApplicationDto packageForm, ref bool hasValidationErrors, ref string validationErrors)
        {
            if (packageForm == null)
            {
                hasValidationErrors = true;
                validationErrors += "No package form data provided";
                return;
            }

            if (string.IsNullOrEmpty(packageForm.Name))
            {
                hasValidationErrors = true;
                validationErrors += "No package name provided";
            }

            if (packageForm.Publisher == null)
            {
                hasValidationErrors = true;
                validationErrors += "No package publisher provided";
            }

            if (hasValidationErrors)
            {
                throw new Exception(validationErrors);
            }
        }

        private void ValidateCreateForm(ApplicationDto packageForm, ref bool hasValidationErrors, ref string validationErrors)
        {
            if (packageForm == null)
            {
                hasValidationErrors = true;
                validationErrors += "No package form data provided";
                return;
            }

            if (string.IsNullOrEmpty(packageForm.Name))
            {
                hasValidationErrors = true;
                validationErrors += "No package name provided";
            }

            if (packageForm.Publisher == null)
            {
                hasValidationErrors = true;
                validationErrors += "No package publisher provided";
            }

            //try
            //{
            //    var cnt = _context.Packages.Count(p=>p.Identifier == packageForm.Identifier);
            //    if (cnt > 0)
            //    {
            //        hasValidationErrors = true;
            //        validationErrors += "Identifier (publisher.name) must be unique";
            //    }
            //}
            //catch (Exception e)
            //{
            //    _logger.LogDebug($"Error accessing to the database: {e}");
            //}

            if (hasValidationErrors)
            {
                throw new Exception(validationErrors);
            }
        }

    }
}
