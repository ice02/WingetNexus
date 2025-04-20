using WingetNexus.Shared.Models.Dtos;

public interface IApplicationService
{
    Task<int> GetNumberOfApplicationsAsync();
    Task<List<Tutorial>> GetTutorialsAsync();
    Task SaveTutorialDismissedStateAsync(int tutorialId, bool dismissed);
}