using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using WingetNexus.Shared.Helpers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public interface IYamlFilesService
{
    Task<dynamic> LoadYaml(string filePath);
    Task<object> DeserializeYamlContent(string fileType, string version, string content);
    Task CreateNewApplicationAsync(string apiUrl, string applicationName, string version, string yamlContent);
}

public class YamlFilesService : IYamlFilesService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<YamlFilesService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public YamlFilesService(IConfiguration configuration, ILogger<YamlFilesService> logger, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<dynamic> LoadYaml(string filePath)
    {
        var yamlDirectory = _configuration.GetValue<string>("TempDirectory");
        if (yamlDirectory == null)
        {
            _logger.LogError("TempDirectory configuration is missing.");
            throw new InvalidOperationException("TempDirectory configuration is missing.");
        }
        var fullPath = Path.Combine(yamlDirectory, filePath);

        if (!System.IO.File.Exists(fullPath))
        {
            _logger.LogError($"File not found: {fullPath}");
            throw new FileNotFoundException($"File not found: {fullPath}");
        }

        var yamlContent = await System.IO.File.ReadAllTextAsync(fullPath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yamlData = deserializer.Deserialize<dynamic>(yamlContent);
        var version = yamlData["version"];
        var transformedVersion = TransformVersionString(version);
        if (transformedVersion == null)
        {
            _logger.LogError("Invalid version format.");
            throw new ArgumentException("Invalid version format.");
        }

        var deserializedObject = DeserializeYamlByVersion(deserializer, yamlContent, transformedVersion);
        if (deserializedObject == null)
        {
            _logger.LogError("Unsupported version.");
            throw new NotSupportedException("Unsupported version.");
        }

        return deserializedObject;
    }

    public async Task<object> DeserializeYamlContent(string fileType, string version, string content)
    {
        var yamlHelpers = new YamlHelpers();

        return yamlHelpers.DeserializeYamlContent(fileType, version, content);
    }

    public async Task CreateNewApplicationAsync(string apiUrl, string applicationName, string version, string yamlContent)
    {
        if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(applicationName) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(yamlContent))
        {
            _logger.LogError("Invalid input parameters for creating a new application.");
            throw new ArgumentException("All input parameters must be provided.");
        }

        var httpClient = _httpClientFactory.CreateClient("default");

        var deserializedYaml = await DeserializeYamlContent("Application", version, yamlContent);
        if (deserializedYaml == null)
        {
            _logger.LogError("Failed to deserialize YAML content.");
            throw new InvalidOperationException("Failed to deserialize YAML content.");
        }

        var payload = new
        {
            Name = applicationName,
            Version = version,
            Data = deserializedYaml
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(apiUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to create application. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            throw new HttpRequestException($"Failed to create application. Status Code: {response.StatusCode}");
        }

        _logger.LogInformation("Application created successfully.");
    }

    

    private object DeserializeYamlByVersion(IDeserializer deserializer, string yamlContent, string version)
    {
        var namespaceName = $"WingetNexus.Shared.Models.Yaml.{version}";
        var className = "VersionClass";
        var typeName = $"{namespaceName}.{className}";
        var type = Type.GetType(typeName);

        if (type == null)
        {
            return null;
        }

        var method = typeof(Deserializer).GetMethod("Deserialize", new[] { typeof(string) });
        var genericMethod = method.MakeGenericMethod(type);
        return genericMethod.Invoke(deserializer, new object[] { yamlContent });
    }
}
