using WingetNexus.Shared.Models.Dtos;
using Version = WingetNexus.Shared.Models.Entities.Version;
using System;
using System.Text.Json;
using System.Reflection;
using WingetNexus.Shared.Helpers;

namespace WingetNexus.Shared.Mappers
{
    public class VersionMapper
    {
        private readonly ApplicationMapper _applicationMapper;
        public VersionMapper(ApplicationMapper applicationMapper)
        {
            _applicationMapper = applicationMapper;
        }

        public VersionDto ToLightDto(Version version)
        {
            return new VersionDto
            {
                Id = version.Id,
                VersionNumber = version.VersionNumber,
                GitHubUrl = version.GitHubUrl,
                ApplicationIdentifier = version.Application?.PackageIdentifier,
                InstallersDatasJson = version.InstallersDatasJson,
                LocalsDatasJson = version.LocalsDatasJson,
                DefaultLocaleKey = version.DefaultLocaleKey,
                ManifestVersion = version.ManifestVersion,
                ShortDescription = version.ShortDescription
            };
        }

        public VersionDto ToFullDto(Version version)
        {

            return new VersionDto
            {
                Id = version.Id,
                VersionNumber = version.VersionNumber,
                GitHubUrl = version.GitHubUrl,
                Installers = EntitiesHelpers.GetInstallersFromJson(version.InstallersDatasJson, version.ManifestVersion),
                Locales = EntitiesHelpers.GetLocalesFromJson(version.LocalsDatasJson, version.ManifestVersion),
                DefaultLocale = EntitiesHelpers.GetDefaultLocaleFromJson(version.DefaultLocaleJson, version.ManifestVersion),
                InstallersDatasJson = version.InstallersDatasJson,
                LocalsDatasJson = version.LocalsDatasJson,
                DefaultLocaleKey = version.DefaultLocaleKey,
                ManifestVersion = version.ManifestVersion,
                Channel = version.Channel,
                CreatedDate = version.CreatedDate,
                ModifiedDate = version.ModifiedDate,
                UserCreated = version.UserCreated,
                UserLastModified = version.UserLastModified,
                Application = version.Application != null ? _applicationMapper.ToDto(version.Application) : null,
                ApplicationIdentifier = version.Application?.Id.ToString() ?? string.Empty,
                ShortDescription = version.ShortDescription
            };
        }

        public Version ToEntity(VersionDto versionDto)
        {
            return new Version
            {
                Id = versionDto.Id,
                VersionNumber = versionDto.VersionNumber,
                GitHubUrl = versionDto.GitHubUrl,
                Installers = versionDto.Installers,
                Locales = versionDto.Locales,
                DefaultLocale = versionDto.DefaultLocale,
                InstallersDatasJson = EntitiesHelpers.SerializeInstallersToJson(versionDto.Installers, versionDto.ManifestVersion),
                LocalsDatasJson = EntitiesHelpers.SerializeLocalesToJson(versionDto.Locales, versionDto.ManifestVersion),
                DefaultLocaleJson = EntitiesHelpers.SerializeDefaultLocaleToJson(versionDto.DefaultLocale, versionDto.ManifestVersion),
                DefaultLocaleKey = versionDto.DefaultLocaleKey,
                ManifestVersion = versionDto.ManifestVersion,
                Channel = versionDto.Channel,
                CreatedDate = versionDto.CreatedDate,
                ModifiedDate = versionDto.ModifiedDate,
                UserCreated = versionDto.UserCreated,
                UserLastModified = versionDto.UserLastModified,
                ApplicationId = int.TryParse(versionDto.ApplicationIdentifier, out var appId) ? appId : 0,
                Application = null, // Application mapping logic can be added if needed,
                ShortDescription = versionDto.ShortDescription
            };
        }
    }
}
