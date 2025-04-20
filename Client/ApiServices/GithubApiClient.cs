using WingetNexus.Shared.Models.Github;

namespace WingetNexus.Client.ApiServices;

public class GithubApiClient(HttpClient httpClient)
{
    public async Task<DirectoryContent[]> GetDirectoryContentAsync(string path, CancellationToken cancellationToken = default)
    {
        List<DirectoryContent>? contents = null;

        await foreach (var content in httpClient.GetFromJsonAsAsyncEnumerable<DirectoryContent>($"/getdirectorycontent?path={path}", cancellationToken))
        {
            if (content is not null)
            {
                contents ??= [];
                contents.Add(content);
            }
        }

        return contents?.ToArray() ?? [];
    }

    public async Task<string> DownloadFileAsync(string filePath, string publisher, string application, string version, CancellationToken cancellationToken = default)
    {
        var request = new DownloadRequest 
        { 
            FilePath = filePath,
            Publisher = publisher,
            Application = application,
            Version = version
        };
        var response = await httpClient.PostAsJsonAsync("/downloadfile", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DownloadResponse>(cancellationToken: cancellationToken);
        return result?.FilePath ?? string.Empty;
    }
}
