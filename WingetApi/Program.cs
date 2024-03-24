using HealthChecks.UI.Client;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text.Json.Serialization;
using WingetNexus.Data;
using WingetNexus.Data.DataStores;
using WingetNexus.Data.Extensions;
using WingetNexus.WingetApi.Services;
using WingetNexus.WingetApi.Settings;

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

services.AddOptions();

services.AddFeatureManagement();

var featureManager = services.BuildServiceProvider().GetRequiredService<IFeatureManager>();

services
    .AddControllersWithViews(options =>options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
    .AddJsonOptions(
    options =>
    { 
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

        var enumConverter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    });

services.AddSingleton<IWingetNexusDataStore, WingetNexusDataStore>();

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
}
else
{
    services.AddPGSqlWithCache<WingetNexusContext>(
        configuration.GetConnectionString("WingetPGSqlContext") ?? throw new InvalidOperationException("Connection String is not found"));

    if (featureManager.IsEnabledAsync("HealthChecks").Result)
    {
        services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("WingetPGSqlContext"));
    }
}

// add swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Winget Nexus API", Version = "v1" });

    //use fully qualified object names
    c.CustomSchemaIds(x => x.FullName);
});

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();

app.UseRouting();

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

// add swagger ui
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Winget Nexus client API v1"));

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

app.MapControllers();

app.Run();
