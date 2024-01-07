using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db
{
    public class InstallerSwitch
    {
        public int Id { get; set; }
        [JsonIgnore]
        public virtual Installer Installer { get; set; }
        [ForeignKey("Installer")]
        public int InstallerId { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }

        public JObject ToJson()
        {
            return new JObject(
                new JProperty("id", Id),
                new JProperty("installer_id", InstallerId),
                new JProperty("parameter", Parameter),
                new JProperty("value", Value)
            );
        }
    }
}
