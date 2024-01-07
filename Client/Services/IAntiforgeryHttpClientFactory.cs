namespace WingetNexus.Client.Services;

public interface IAntiforgeryHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(string clientName = AuthDefaults.AuthorizedClientName);
}
