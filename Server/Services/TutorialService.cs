using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WingetNexus.Data;
using WingetNexus.Data.Models;

namespace WingetNexus.Server.Services
{
    public class TutorialService
    {
        private readonly WingetNexusContext _context;

        public TutorialService(WingetNexusContext context)
        {
            _context = context;
        }

        public async Task<List<TutorialDismissedState>> GetDismissedStatesAsync(string userId)
        {
            return await _context.TutorialDismissedStates
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveDismissedStateAsync(string userId, int tutorialId, bool isDismissed)
        {
            var existingState = await _context.TutorialDismissedStates
                .FirstOrDefaultAsync(t => t.UserId == userId && t.TutorialId == tutorialId);

            if (existingState != null)
            {
                existingState.IsDismissed = isDismissed;
            }
            else
            {
                var newState = new TutorialDismissedState
                {
                    UserId = userId,
                    TutorialId = tutorialId,
                    IsDismissed = isDismissed
                };
                await _context.TutorialDismissedStates.AddAsync(newState);
            }

            await _context.SaveChangesAsync();
        }
    }
}