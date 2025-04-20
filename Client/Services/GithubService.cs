using Octokit;
using WingetNexus.Shared.Models.Github;

public interface IGithubService
{
    Task<List<DirectoryContent>> GetDirectoryContent(string path);
    Task<string> DownloadFile(DownloadRequest request);
}

public class GithubService : IGithubService
{
    private readonly IConfiguration _configuration;

    public GithubService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<DirectoryContent>> GetDirectoryContent(string path)
    {
        var client = new GitHubClient(new Octokit.ProductHeaderValue("WingetNexus"));
        var contents = await client.Repository.Content.GetAllContents("microsoft", "winget-pkgs", path);
        return contents.Select(content => new DirectoryContent(content.Name, content.Path, content.Type, content.DownloadUrl)).ToList();
    }

    public async Task<string> DownloadFile(DownloadRequest request)
    {
        var client = new GitHubClient(new Octokit.ProductHeaderValue("WingetNexus"));
        var fileContent = await client.Repository.Content.GetRawContent("microsoft", "winget-pkgs", request.FilePath);

        var yamlDirectory = _configuration.GetValue<string>("TempDirectory");
        if (yamlDirectory == null)
        {
            throw new InvalidOperationException("TempDirectory configuration is missing.");
        }
        yamlDirectory = Path.Combine(yamlDirectory, request.Publisher, request.Application, request.Version);
        if (!Directory.Exists(yamlDirectory))
            Directory.CreateDirectory(yamlDirectory);

        var fileName = Path.GetFileName(request.FilePath);
        var filePath = Path.Combine(yamlDirectory, fileName);

        await File.WriteAllBytesAsync(filePath, fileContent);

        return fileName;
    }
}
