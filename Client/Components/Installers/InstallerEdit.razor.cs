using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WingetNexus.Shared.Helpers;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Client.Components.Installers
{
    public partial class InstallerEdit
    {
        [Parameter]
        public InstallerDto InstallerModel { get; set; }

        bool isSilentVisible = false;
        bool isLocationVisible = false;
        bool isLogpathVisible = false;
        bool isUpgradeVisible = false;
        bool isSilentProgressVisible
        {
            get { return (InstallerModel != null && InstallerModel.Switches != null && InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "SilentProgress") != null) 
                    ? bool.Parse(InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "SilentProgress").Value) 
                    : false; }
            set { 
                if (InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "SilentProgress") == null)
                    InstallerModel.Switches.Add(new SwitchDto() { Parameter = "SilentProgress", Value = value.ToString() });
                else
                    InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "SilentProgress").Value = value.ToString(); 
            }
        }
        bool isInteractiveVisible
        {
            get { return (InstallerModel != null && InstallerModel.Switches != null && InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Interactive") != null) 
                    ? bool.Parse(InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Interactive").Value) 
                    : false; }
            set { 
                if (InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Interactive") == null)
                    InstallerModel.Switches.Add(new SwitchDto() { Parameter = "Interactive", Value = value.ToString() });
                else
                    InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Interactive").Value = value.ToString(); 
            }
        }

        bool isCustomVisible = false;

        string silentParameter 
        { 
            get { return (InstallerModel != null && InstallerModel.Switches != null) 
                    ? InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Silent")?.Value 
                    : string.Empty; }
            set {
                if (InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Silent") == null)
                    InstallerModel.Switches.Add(new SwitchDto() { Parameter = "Silent", Value = value });
                else
                    InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Silent").Value = value; 
            }
        }
        string installLocation
        {
            get { return (InstallerModel != null && InstallerModel.Switches != null) 
                    ? InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "InstallLocation")?.Value 
                    : string.Empty; }
            set { 
                if (InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "InstallLocation") == null)
                    InstallerModel.Switches.Add(new SwitchDto() { Parameter = "InstallLocation", Value = value });
                else
                    InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "InstallLocation").Value = value; 
            }
        }
        string logFilesLocation
        {
            get { return (InstallerModel != null && InstallerModel.Switches != null) 
                    ? InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Logfile")?.Value 
                    : string.Empty; }
            set { 
                if (InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Logfile") == null)
                    InstallerModel.Switches.Add(new SwitchDto() { Parameter = "Logfile", Value = value });
                else
                    InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Logfile").Value = value; 
            }
        }
        string upgradeCode
        {
            get { return (InstallerModel != null && InstallerModel.Switches != null) 
                    ? InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Upgrade")?.Value 
                    : string.Empty; }
            set { 
                if (InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Upgrade") == null)
                    InstallerModel.Switches.Add(new SwitchDto() { Parameter = "Upgrade", Value = value });
                else
                    InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Upgrade").Value = value; 
            }
        }
        string customValue
        {
            get { return (InstallerModel != null && InstallerModel.Switches != null) 
                    ? InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Custom")?.Value 
                    : string.Empty; }
            set { 
                if (InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Custom") == null)
                    InstallerModel.Switches.Add(new SwitchDto() { Parameter = "Custom", Value = value });
                else
                    InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Custom").Value = value; 
            }
        }

        HttpClient httpClient = null;
        string relativeFilePath = string.Empty;
        string shortcutName = string.Empty;
        string selectedValue
        {
            get { return InstallerModel != null ? InstallerModel.InstallerType : "msix"; }
            set
            {
                InstallerModel.InstallerType = value;

                if (InstallerModel.InstallerType == "zip")
                {
                    InstallerModel.NestedInstallerFiles = new List<NestedInstallerFileDto>()
                {
                    new NestedInstallerFileDto()
                    {
                        PortableCommandAlias = shortcutName,
                        RelativeFilePath = relativeFilePath,
                    }
                };
                }
                else
                {
                    InstallerModel.NestedInstallerFiles = null;
                }
            }
        }

        // private async void onValueChanged(string value)
        // {
        //     InstallerModel.InstallerType = value;

        //     if (InstallerModel.InstallerType == "zip")
        //     {
        //         InstallerModel.NestedInstallerFiles = new List<NestedInstallerFileDto>()
        //         {
        //             new NestedInstallerFileDto()
        //             {
        //                 PortableCommandAlias = string.Empty,
        //                 RelativeFilePath = relativeFilePath
        //             }
        //         };
        //     }
        //     else
        //     {
        //         InstallerModel.NestedInstallerFiles = null;
        //     }
        // }

        private int activeIndex
        {
            get
            {
                if (InstallerModel == null)
                    return 0;
                return (InstallerModel.IsLocalPackage ? 0 : 1);
            }
            set
            {
                InstallerModel.IsLocalPackage = (value != 1);
            }
        }

        protected async override Task OnInitializedAsync()
        {
            httpClient = await httpClientFactory.CreateClientAsync();

            if (InstallerModel == null)
            {
                InstallerModel = new InstallerDto();
                InstallerModel.Scope = "user";
                InstallerModel.Architecture = "x64";
                InstallerModel.InstallerType = "msix";
                InstallerModel.InstallerPath = string.Empty;
                InstallerModel.InstallerSha256 = string.Empty;
                InstallerModel.NestedInstallerFiles = null;
                InstallerModel.Switches = new List<SwitchDto>()
                {
                    new SwitchDto() { Parameter="Silent", Value="True" },
                    new SwitchDto() { Parameter="InstallLocation", Value="" },
                    new SwitchDto() { Parameter="SilentProgress", Value="True" },
                    new SwitchDto() { Parameter="Interactive", Value="False" },
                    new SwitchDto() { Parameter="Upgrade", Value="False" },
                    new SwitchDto() { Parameter="Logfile", Value="" },
                    new SwitchDto() { Parameter="Custom", Value="" }
                };
            }
            else
            {
                selectedValue = InstallerModel.InstallerType;
                if (InstallerModel.NestedInstallerFiles != null && InstallerModel.NestedInstallerFiles.Count > 0 && InstallerModel.InstallerType == "zip")
                {
                    relativeFilePath = InstallerModel.NestedInstallerFiles[0].RelativeFilePath;
                }

                if(InstallerModel.Switches != null && InstallerModel.Switches.Count >0)
                {
                    if (InstallerModel.Switches.Any(x => x.Parameter == "Silent"))
                    {
                        isSilentVisible = true;
                        silentParameter = InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Silent").Value;
                    }
                    if (InstallerModel.Switches.Any(x => x.Parameter == "InstallLocation"))
                    {
                        isLocationVisible = true;
                        installLocation = InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "InstallLocation").Value;
                    }
                    if (InstallerModel.Switches.Any(x => x.Parameter == "Logfile"))
                    {
                        isLogpathVisible = true;
                        logFilesLocation = InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Logfile").Value;
                    }
                    if (InstallerModel.Switches.Any(x => x.Parameter == "Upgrade"))
                    {
                        isUpgradeVisible = true;
                        upgradeCode = InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Upgrade").Value;
                    }
                    if (InstallerModel.Switches.Any(x => x.Parameter == "Custom"))
                    {
                        isCustomVisible = true;
                        customValue = InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Custom").Value;
                    }
                    if (InstallerModel.Switches.Any(x => x.Parameter == "SilentProgress"))
                    {
                        isSilentProgressVisible = bool.Parse(InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "SilentProgress").Value);
                    }
                    if (InstallerModel.Switches.Any(x => x.Parameter == "Interactive"))
                    {
                        isInteractiveVisible = bool.Parse(InstallerModel.Switches.FirstOrDefault(x => x.Parameter == "Interactive").Value);
                    }
                }
            }
        }

        IList<IBrowserFile> files = new List<IBrowserFile>();
        private async void UploadFiles(IBrowserFile file)
        {
            files = new List<IBrowserFile>();
            files.Add(file);

            InstallerModel.InstallerType = GetInstallerTypeFromContentType(file.ContentType);
            //TODO : check if file extension is valid
            // if (string.IsNullOrEmpty(model.InstallerType))
            // {
            //     return;
            // }

            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream(128 * 1024 * 1024)); // 128 Mo Max

            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(file.Name, out contentType))
            {
                contentType = "application/octet-stream";
            }

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            content.Add(
                content: fileContent,
                name: "\"files\"",
                fileName: file.Name);

            var result = await httpClient.PostAsync($"api/v1/files", content);
            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadFromJsonAsync<IList<UploadResult>>();
                if (resultContent == null || resultContent.Count == 0)
                {
                    snackBar.Add("Error during upload", Severity.Error);
                }
                else
                {
                    foreach (var item in resultContent)
                    {
                        if (item.ErrorCode > 0)
                        {
                            // TODO: map error code on a message
                            snackBar.Add(item.ErrorCode.ToString(), Severity.Error);
                        }
                        else
                        {
                            InstallerModel.InstallerPath = item.FileName;
                            InstallerModel.InstallerSha256 = item.Checksum;
                        }
                    }
                }
            }
            else
            {
                snackBar.Add(result.ReasonPhrase, Severity.Error);
            }
        }

        private bool CheckInstallerType => InstallerModel.InstallerType != "zip";

        //Based on content type value, give the installer type
        private string GetInstallerTypeFromContentType(string contentType)
        {
            var installerType = "";
            switch (contentType)
            {
                case "application/zip":
                    installerType = "zip";
                    break;
                case "application/msi":
                    installerType = "msi";
                    break;
                case "application/msix":
                    installerType = "msix";
                    break;
            }

            return installerType;
        }
    }
}
