using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using WingetNexus.Shared.Models.Yaml;

namespace WingetNexus.Shared.Mappers
{
    public class ApplicationMapper
    {
        public ApplicationDto ToDto(Application application)
        {
            return new ApplicationDto
            {
                Id = application.Id,
                Name = application.Name,
                //Publisher = application.Publisher,
                //PackageIdentifier = application.PackageIdentifier,
                //GitHubUrl = application.GitHubUrl
                //Versions = application.Versions?.Select(v => new VersionDto
                //{
                //    Id = v.Id,
                //    VersionNumber = v.VersionNumber,
                //    InstallersDatasJson = v.InstallersDatasJson,
                //    ManifestVersion = v.ManifestVersion,
                //    DefaultLocaleKey = v.DefaultLocaleKey,
                //    LocalsDatasJson = v.LocalsDatasJson,
                //    CreatedDate = v.CreatedDate,
                //    ModifiedDate = v.ModifiedDate,
                //    Installers = GetInstallersFromJson(v.InstallersDatasJson, v.ManifestVersion),
                //}).ToList()
            };
        }

        private static List<IInstallerClass> GetInstallersFromJson(string datasJson, string manifestVersion)
        {
            // Déterminez le namespace et la classe appropriés en fonction de la version du manifeste
            string namespacePrefix = "WingetNexus.Shared.Models.Yaml";
            string versionNamespace = manifestVersion.Replace('.', '_');
            string className = $"{namespacePrefix}.v{versionNamespace}.Installer.InstallerClass";

            // Utilisez la réflexion pour obtenir le type de la classe
            Type installerType = Type.GetType(className);
            if (installerType == null)
            {
                throw new InvalidOperationException($"Cannot find type {className}");
            }

            // Désérialisez le JSON en utilisant la classe appropriée
            var installers = JsonSerializer.Deserialize(datasJson, installerType) as List<IInstallerClass>;

            // Convertissez les objets désérialisés en InstallerDto
            //var installerDtos = new List<InstallerDto>();
            //foreach (var installer in (IEnumerable<object>)installers)
            //{
            //    var installerDto = new InstallerDto
            //    {
            //        // Mappez les propriétés de l'installer à InstallerDto
            //        // Assurez-vous que les propriétés de l'installer correspondent à celles de InstallerDto
            //    };
            //    installerDtos.Add(installerDto);
            //}

            return installers;
        }

        //public static Application ToEntity(this ApplicationDto applicationDto)
        //{
        //    var application = new Application
        //    {
        //        Id = applicationDto.Id,
        //        Name = applicationDto.Name,
        //        PackageIdentifier = applicationDto.PackageIdentifier,
        //        GitHubUrl = applicationDto.GitHubUrl,
        //        CreatedDate = applicationDto.DateAdded,
        //        ModifiedDate = DateTime.Now,
        //        Versions = applicationDto.Versions?.Select(v => new Version
        //        {
        //            Id = v.Id,
        //            VersionNumber = v.VersionCode,
        //            DefaultLocale = v.PackageLocale,
        //            CreatedDate = v.DateAdded,
        //            ModifiedDate = DateTime.Now,
        //            Installers = v.Installers?.Select(i => new Installer
        //            {
        //                Id = i.Id,
        //                Architecture = i.Architecture,
        //                InstallerType = i.InstallerType,
        //                InstallerPath = i.InstallerPath,
        //                IsLocalPackage = i.IsLocalPackage,
        //                InstallerSha256 = i.InstallerSha256,
        //                Scope = i.Scope,
        //                NestedInstallerType = i.NestedInstallerType,
        //                CreatedDate = i.DateAdded,
        //                ModifiedDate = i.DateModified
        //            }).ToList()
        //        }).ToList()
        //    };

        //    if (!string.IsNullOrEmpty(applicationDto.Publisher))
        //    {
        //        application.Publisher = new Publisher { Name = applicationDto.Publisher };
        //    }

        //    return application;
        //}
    }
}
