using HealthChecks.UI.Client;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WingetNexus.Data;
using WingetNexus.Data.DataStores;
using WingetNexus.Data.Extensions;
using WingetNexus.Server.Security;
using WingetNexus.Server.Settings;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

//if (OperatingSystem.IsWindows())
//{
//    // get congifuration from config file
//    configuration = new ConfigurationBuilder()
//        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
//        .AddEnvironmentVariables()
//        .Build();
//}
//else
//{ 
//    //get configuration from environment variables
//    configuration = new ConfigurationBuilder()
//        .AddEnvironmentVariables()
//        .Build();
//}

services.AddAntiforgery(options =>
{
    options.HeaderName = AntiforgeryDefaults.HeaderName;
    options.Cookie.Name = AntiforgeryDefaults.CookieName;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

services.AddHttpClient();
services.AddOptions();

services.AddFeatureManagement();

var featureManager = services.BuildServiceProvider().GetRequiredService<IFeatureManager>();

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    configuration.GetSection("OpenIDConnectSettings").Bind(options);

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ResponseType = OpenIdConnectResponseType.Code;

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
    options.Events = new OpenIdConnectEvents
    {
        // called if user clicks Cancel during login
        OnAccessDenied = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/");
            return Task.CompletedTask;
        }
    };
});

services
    .AddControllersWithViews(options =>options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
    .AddJsonOptions(options =>options.JsonSerializerOptions.PropertyNamingPolicy = null);

services.AddSingleton<IWingetNexusDataStore, WingetNexusDataStore>();
services.AddTransient<IApplicationDatastore, ApplicationDatastore>();

//Storage management
if (featureManager.IsEnabledAsync("S3Storage").Result)
{
    //services.AddAWSService<IAmazonS3>();
    services.Configure<S3StorageSettings>(configuration.GetSection("S3StorageSettings"));
    services.AddSingleton<IStorageService, S3StorageService>();
}
else
{
    services.Configure<LocalStorageSettings>(configuration.GetSection("LocalStorageSettings"));
    services.AddSingleton<IStorageService, LocalStorageService>();
}

services.AddRazorPages().AddMvcOptions(options =>
{
    //var policy = new AuthorizationPolicyBuilder()
    //    .RequireAuthenticatedUser()
    //    .Build();
    //options.Filters.Add(new AuthorizeFilter(policy));
});

//services.AddAuthorization(config =>
//{
//    config.AddPolicy("IsAuthorized",
//        policy => policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "email")
//        );
//});

services.AddScoped<IDatabaseAuthenticationManager, DatabaseAuthenticationManager>();

services.AddScoped<IAuthorizationHandler, DatabasePolicyAuthorizationHandler>();
services.AddAuthorization(options =>
{
    options.AddPolicy("IsAuthorized", policy => policy.Requirements.Add(new DatabasePolicyRequirement(new string[]{ "contributor", "admin" })));
    options.AddPolicy("IsAdmin", policy => policy.Requirements.Add(new DatabasePolicyRequirement(new string[] { "admin" })));
});



services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});

//TODO: add feature triggering
if (featureManager.IsEnabledAsync("HealthChecks").Result)
{
    services.AddHealthChecks()
        .AddDiskStorageHealthCheck(p => p.CheckAllDrives = true)
        //.AddPrivateMemoryHealthCheck(10000)
        //.AddVirtualMemorySizeHealthCheck(10000)
        //.AddWorkingSetHealthCheck(10000)
        // add check on redis
        //.AddRedis(builder.Configuration.GetConnectionString("RedisConnection"))

        ;
    services.AddHealthChecksUI().AddInMemoryStorage();
}


//builder.Services.AddControllersWithViews().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.PropertyNamingPolicy = null;
//});

if (configuration["DatabaseType"] == "SQLite")
{
    services.AddSqliteWithCache<WingetNexusContext>(
        configuration.GetConnectionString("WingetSqLiteContext") ?? throw new InvalidOperationException("Connection String is not found"));
    services.AddSqlite<AppDbContext>(
        configuration.GetConnectionString("AppSqLiteContext") ?? throw new InvalidOperationException("Connection String is not found"));
}
else
{
    services.AddPGSqlWithCache<WingetNexusContext>(
        configuration.GetConnectionString("WingetPGSqlContext") ?? throw new InvalidOperationException("Connection String is not found"));
    services.AddNpgsql<AppDbContext>(
        configuration.GetConnectionString("AppPGSqlContext") ?? throw new InvalidOperationException("Connection String is not found"));

    if (featureManager.IsEnabledAsync("HealthChecks").Result)
    {
        services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("WingetPGSqlContext"));
        services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("AppPGSqlContext"));
    }
}

// add swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Winget Nexus API", Version = "v1" });
});

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

//app.UseSecurityHeaders(
//    SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment(),
//        configuration["OpenIDConnectSettings:Authority"]!));

app.UseHttpsRedirection();
app.UseHsts();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseNoUnauthorizedRedirect("/api");

app.UseAuthentication();
app.UseAuthorization();

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

// add swagger ui
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Winget Nexus API v1"));

//TODO: add feature triggering
if (featureManager.IsEnabledAsync("HealthChecks").Result)
{
    app.UseHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    app.UseHealthChecksUI();
}

app.MapRazorPages();
app.MapControllers();
app.MapNotFound("/api/{**segment}");
app.MapFallbackToPage("/_Host");

app.Run();
