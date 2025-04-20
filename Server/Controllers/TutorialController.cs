using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WingetNexus.Data.Models;
using WingetNexus.Server.Services;

namespace WingetNexus.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TutorialController : ControllerBase
    {
        private readonly TutorialService _tutorialService;

        public TutorialController(TutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        [HttpGet("dismissed-states")]
        public async Task<ActionResult<List<TutorialDismissedState>>> GetDismissedStates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var states = await _tutorialService.GetDismissedStatesAsync(userId);
            return Ok(states);
        }

        [HttpPost("dismiss-state")]
        public async Task<IActionResult> SaveDismissedState([FromBody] TutorialDismissedState state)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            await _tutorialService.SaveDismissedStateAsync(userId, state.TutorialId, state.IsDismissed);
            return NoContent();
        }
    }
}