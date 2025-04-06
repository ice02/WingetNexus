// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;
using WingetNexus.ClientManifestClassGenerator;

//get version, target folder and type of the manifest from command line, if not present, use default values
var manifestType = args.Length > 0 ? args[0] : "all";
var version = args.Length > 1 ? args[1] : "all";
var targetFolder = args.Length > 2 ? args[2] : ".\\Generated\\";

// validate version is like x.x.x
if (!Regex.IsMatch(version, @"^\d+\.\d+\.\d+$") && version != "all")
{
    Console.WriteLine("Invalid version format, should be x.x.x");
    return;
}

// Help trigger
if (args.Length > 0 && args[0] == "-help")
{
    Console.WriteLine("Usage: WingetNexus.ClientManifestClassGenerator [version] [targetFolder] [manifestType]");
    Console.WriteLine("version: the version of the manifest to generate classes for, default is all");
    Console.WriteLine("targetFolder: the folder where to write the generated classes, default is .\\Generated\\");
    Console.WriteLine("manifestType: the type of the manifest to generate classes for, default is all, possible values are: all, installer, version, locale, singleton");
    return;
}

string[] versions = [];

if (version == "all")
{
    Console.WriteLine("Generating classes for all versions");
    versions = ["1.4.0", "1.5.0", "1.6.0", "1.7.0", "1.9.0", "1.10.0"];
}

if (version == "latest")
{
    Console.WriteLine("Generating classes for the latest version");
    versions = ["1.10.0"];
}

foreach (var v in versions)
{
    //create and call the github service
    var githubService = new GithubService();
    var files = await githubService.GetManifestFilesForVersion(v, manifestType);

    // create and call the class generator service
    var classGeneratorService = new ClassGeneratorService();
    var ns = v.Split('.');
    foreach (var file in files)
    {
        var classFile = await classGeneratorService.GenerateClassFromJsonSchemaAsync(file.Value, $"WingetNexus.Shared.Models.Yaml.v{ns[0]}._{ns[1]}.{file.Key}", $"{file.Key}Class");
        // writel the class file to disk to a dedicated folder based on a version string, create it if notr exist, overwrite if exist
        var path = Path.Combine(targetFolder, v);
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{file.Key}Class.cs"), classFile);

    }
}



