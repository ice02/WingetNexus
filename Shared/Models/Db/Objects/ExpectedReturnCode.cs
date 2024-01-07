using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Db.Objects
{
    public  class ExpectedReturnCode
    {
        /// <summary>
        /// Gets or sets InstallerReturnCode.
        /// </summary>
        public long InstallerReturnCode { get; set; }

        /// <summary>
        /// Gets or sets ReturnResponse.
        /// </summary>
        public string ReturnResponse { get; set; }

        /// <summary>
        /// Gets or sets ReturnResponseUrl.
        /// </summary>
        public string ReturnResponseUrl { get; set; }
    }
}
