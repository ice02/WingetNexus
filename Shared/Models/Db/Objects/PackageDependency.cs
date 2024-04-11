using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db.Objects
{
    public class PackageDependency
    {

        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets PackageIdentifier.
        /// </summary>
        [JsonIgnore]
        public virtual Package Package { get; set; }
        [ForeignKey("Package")]
        public int PackageId { get; set; }

        /// <summary>
        /// Gets or sets MinimumVersion.
        /// </summary>
        public string? MinimumVersion { get; set; }
    }
}
