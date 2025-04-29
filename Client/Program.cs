using Blazored.LocalStorage;
using MudBlazor.Services;
using WingetNexus.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var services = builder.Services;

services.AddOptions();
services.AddAuthorizationCore();
services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
services.TryAddSingleton(sp => (HostAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
services.AddTransient<AuthorizedHandler>();

services.AddApplicationServices();

builder.RootComponents.Add<App>("#app");

services.AddHttpClient("default", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

services.AddHttpClient(AuthDefaults.AuthorizedClientName, client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<AuthorizedHandler>();

services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
services.AddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();

// services.AddTransient<HttpClient>(sp =>
// {
//     var antiforgeryHttpClientFactory = sp.GetRequiredService<IAntiforgeryHttpClientFactory>();
//     return antiforgeryHttpClientFactory.CreateClientAsync().GetAwaiter().GetResult();
// });

services.AddBlazoredLocalStorage();

services.AddMudServices();

await builder.Build().RunAsync();
