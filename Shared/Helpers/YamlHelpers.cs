using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WingetNexus.Shared.Helpers
{
    public class YamlHelpers
    {
        public async Task<object> DeserializeYamlContent(string fileType, string version, string content)
        {
            // Transformer la première lettre de fileType en majuscule
            if (!string.IsNullOrEmpty(fileType))
            {
                fileType = char.ToUpper(fileType[0]) + fileType.Substring(1);
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var transformedVersion = TransformVersionString(version);
            if (transformedVersion == null)
            {
                throw new ArgumentException("Invalid version format.");
            }

            var namespaceName = $"WingetNexus.Shared.Models.Yaml.{transformedVersion}";
            var typeName = $"{namespaceName}.{fileType}.{fileType}Class, WingetNexus.Shared";
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new ArgumentException("Unsupported file type or version.");
            }

            var instance = deserializer.Deserialize(content, type);
            if (instance == null)
            {
                throw new InvalidOperationException("Unable to deserialize the content into the specified type.");
            }

            return instance;
        }


        private string TransformVersionString(string version)
        {
            var versionParts = version.Split('.');
            if (versionParts.Length != 3 || !int.TryParse(versionParts[0], out _) || !int.TryParse(versionParts[1], out _) || !int.TryParse(versionParts[2], out _))
            {
                return null;
            }
            return $"v{versionParts[0]}._{versionParts[1]}";
        }
    }
}
