using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Db;

namespace WingetNexus.Shared.Models.Winget
{
    /// <summary>
    /// SearchVersions.
    /// </summary>
    public class SearchVersions
    {
        //private const bool Nullable = true;
        //private const bool Unique = true;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="SearchVersions"/> class.
        ///// </summary>
        //public SearchVersions()
        //{
        //    this.AllowNull = Nullable;
        //    this.UniqueItems = Unique;
        //}

        ///// <summary>
        ///// Merge in a Search Version.
        ///// </summary>
        ///// <param name="searchVersions">Search Versions.</param>
        //public void Merge(SearchVersions searchVersions)
        //{
        //    foreach (SearchVersion searchVersion in searchVersions)
        //    {
        //        if (this.Exists(searchVersion.ConsolidationExpression))
        //        {
        //            int ind = this.FindIndex(searchVersion.ConsolidationExpression);
        //            this[ind].Merge(searchVersion);
        //        }
        //        else
        //        {
        //            this.Add(searchVersion);
        //        }
        //    }
        //}

        public string PackageVersion { get; set; }
        //public List<Installer> Installers { get; set; }
        //public string Channel { get; set; }
        //public string[] ProductCodes { get; set; }
        //public string[] AppsAndFeaturesEntryVersions { get; set; }
        //public string[] UpgradeCodes { get; set; }


    }
}
