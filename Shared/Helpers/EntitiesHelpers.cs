namespace WingetNexus.Shared.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using WingetNexus.Shared.Models.Yaml;

public static class EntitiesHelpers
{
    public static List<IInstallerClass> GetInstallersFromJson(string installersDatasJson, string manifestVersion)
    {
        try
        {
            if (string.IsNullOrEmpty(installersDatasJson) || string.IsNullOrEmpty(manifestVersion))
            {
                return null;
            }

            // Calculate the target namespace based on the manifest version
            var versionParts = manifestVersion.Split('.');
            if (versionParts.Length < 2)
            {
                throw new ArgumentException("Invalid manifest version format.");
            }

            var major = versionParts[0];
            var minor = versionParts[1];
            var targetNamespace = $"WingetNexus.Shared.Models.Yaml.v{major}_{minor}.Installer";

            // Dynamically load the InstallerClass type
            var assembly = Assembly.GetExecutingAssembly();
            var installerClassType = assembly.GetType($"{targetNamespace}.InstallerClass");
            if (installerClassType == null)
            {
                throw new TypeLoadException($"Type {targetNamespace}.InstallerClass not found.");
            }

            // Deserialize the JSON into a list of the dynamically loaded type
            var genericListType = typeof(List<>).MakeGenericType(installerClassType);
            var deserializedList = JsonSerializer.Deserialize(installersDatasJson, genericListType);

            // Cast the deserialized list to a list of IInstallerClass
            return ((IEnumerable<object>)deserializedList).Cast<IInstallerClass>().ToList();
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new InvalidOperationException("Error while getting installers from JSON.", ex);
        }
    }

    public static List<ILocaleClass> GetLocalesFromJson(string localsDatasJson, string manifestVersion)
    {
        try
        {
            if (string.IsNullOrEmpty(localsDatasJson) || string.IsNullOrEmpty(manifestVersion))
            {
                return null;
            }

            // Calculate the target namespace based on the manifest version
            var versionParts = manifestVersion.Split('.');
            if (versionParts.Length < 2)
            {
                throw new ArgumentException("Invalid manifest version format.");
            }

            var major = versionParts[0];
            var minor = versionParts[1];
            var targetNamespace = $"WingetNexus.Shared.Models.Yaml.v{major}_{minor}.Locale";

            // Dynamically load the LocaleClass type
            var assembly = Assembly.GetExecutingAssembly();
            var localClassType = assembly.GetType($"{targetNamespace}.LocaleClass");
            if (localClassType == null)
            {
                throw new TypeLoadException($"Type {targetNamespace}.LocaleClass not found.");
            }

            // Deserialize the JSON into a list of the dynamically loaded type
            var genericListType = typeof(List<>).MakeGenericType(localClassType);
            var deserializedList = JsonSerializer.Deserialize(localsDatasJson, genericListType);

            // Cast the deserialized list to a list of ILocalClass
            return ((IEnumerable<object>)deserializedList).Cast<ILocaleClass>().ToList();
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new InvalidOperationException("Error while getting locales from JSON.", ex);
        }
    }

    public static IDefaultLocaleClass GetDefaultLocaleFromJson(string defaultLocaleJson, string manifestVersion)
    {
        try
        {
            if (string.IsNullOrEmpty(defaultLocaleJson) || string.IsNullOrEmpty(manifestVersion))
            {
                return null;
            }

            // Calculate the target namespace based on the manifest version
            var versionParts = manifestVersion.Split('.');
            if (versionParts.Length < 2)
            {
                throw new ArgumentException("Invalid manifest version format.");
            }

            var major = versionParts[0];
            var minor = versionParts[1];
            var targetNamespace = $"WingetNexus.Shared.Models.Yaml.v{major}_{minor}.DefaultLocale";

            // Dynamically load the DefaultLocaleClass type
            var assembly = Assembly.GetExecutingAssembly();
            var defaultLocaleClassType = assembly.GetType($"{targetNamespace}.DefaultLocaleClass");
            if (defaultLocaleClassType == null)
            {
                throw new TypeLoadException($"Type {targetNamespace}.DefaultLocaleClass not found.");
            }

            // Deserialize the JSON into the dynamically loaded type
            var deserializedObject = JsonSerializer.Deserialize(defaultLocaleJson, defaultLocaleClassType);

            // Cast the deserialized object to IDefaultLocaleClass
            return (IDefaultLocaleClass)deserializedObject;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new InvalidOperationException("Error while getting default locale from JSON.", ex);
        }
    }

    public static string SerializeLocalesToJson(List<ILocaleClass> locales, string manifestVersion)
    {
        try
        {
            if (locales == null || string.IsNullOrEmpty(manifestVersion))
            {
                return null;
            }

            // Calculate the target namespace based on the manifest version
            var versionParts = manifestVersion.Split('.');
            if (versionParts.Length < 2)
            {
                throw new ArgumentException("Invalid manifest version format.");
            }

            var major = versionParts[0];
            var minor = versionParts[1];
            var targetNamespace = $"WingetNexus.Shared.Models.Yaml.v{major}_{minor}.Locale";

            // Dynamically load the LocaleClass type
            var assembly = Assembly.GetExecutingAssembly();
            var localClassType = assembly.GetType($"{targetNamespace}.LocaleClass");
            if (localClassType == null)
            {
                throw new TypeLoadException($"Type {targetNamespace}.LocaleClass not found.");
            }

            // Serialize the list of locales to JSON
            var genericListType = typeof(List<>).MakeGenericType(localClassType);
            var castedLocales = locales.Cast<object>().ToList();
            var json = JsonSerializer.Serialize(castedLocales, genericListType);

            return json;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new InvalidOperationException("Error while serializing locales to JSON.", ex);
        }
    }

    public static string SerializeInstallersToJson(List<IInstallerClass> installers, string manifestVersion)
    {
        try
        {
            if (installers == null || string.IsNullOrEmpty(manifestVersion))
            {
                return null;
            }

            // Calculate the target namespace based on the manifest version
            var versionParts = manifestVersion.Split('.');
            if (versionParts.Length < 2)
            {
                throw new ArgumentException("Invalid manifest version format.");
            }

            var major = versionParts[0];
            var minor = versionParts[1];
            var targetNamespace = $"WingetNexus.Shared.Models.Yaml.v{major}_{minor}.Installer";

            // Dynamically load the InstallerClass type
            var assembly = Assembly.GetExecutingAssembly();
            var installerClassType = assembly.GetType($"{targetNamespace}.InstallerClass");
            if (installerClassType == null)
            {
                throw new TypeLoadException($"Type {targetNamespace}.InstallerClass not found.");
            }

            // Serialize the list of installers to JSON
            var genericListType = typeof(List<>).MakeGenericType(installerClassType);
            var castedInstallers = installers.Cast<object>().ToList();
            var json = JsonSerializer.Serialize(castedInstallers, genericListType);

            return json;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new InvalidOperationException("Error while serializing installers to JSON.", ex);
        }
    }

    public static string SerializeDefaultLocaleToJson(IDefaultLocaleClass defaultLocale, string manifestVersion)
    {
        try
        {
            if (defaultLocale == null || string.IsNullOrEmpty(manifestVersion))
            {
                return null;
            }

            // Calculate the target namespace based on the manifest version
            var versionParts = manifestVersion.Split('.');
            if (versionParts.Length < 2)
            {
                throw new ArgumentException("Invalid manifest version format.");
            }

            var major = versionParts[0];
            var minor = versionParts[1];
            var targetNamespace = $"WingetNexus.Shared.Models.Yaml.v{major}_{minor}.DefaultLocale";

            // Dynamically load the DefaultLocaleClass type
            var assembly = Assembly.GetExecutingAssembly();
            var defaultLocaleClassType = assembly.GetType($"{targetNamespace}.DefaultLocaleClass");
            if (defaultLocaleClassType == null)
            {
                throw new TypeLoadException($"Type {targetNamespace}.DefaultLocaleClass not found.");
            }

            // Serialize the default locale to JSON
            var json = JsonSerializer.Serialize(defaultLocale, defaultLocaleClassType);

            return json;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new InvalidOperationException("Error while serializing default locale to JSON.", ex);
        }
    }
}