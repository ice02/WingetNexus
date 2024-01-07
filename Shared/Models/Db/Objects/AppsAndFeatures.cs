using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db.Objects
{
    public class AppsAndFeatures
    {
        /// <summary>
        /// Gets or sets DisplayName.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Publisher.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets DisplayVersion.
        /// </summary>
        public string DisplayVersion { get; set; }

        /// <summary>
        /// Gets or sets ProductCode.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets UpgradeCode.
        /// </summary>
        public string UpgradeCode { get; set; }

        /// <summary>
        /// Gets or sets InstallerType.
        /// </summary>
        public string InstallerType { get; set; }
    }
}
