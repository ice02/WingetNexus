using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget
{
    /// <summary>
    /// ManifestSearchRequest.
    /// </summary>
    public class ManifestSearchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestSearchRequest"/> class.
        /// </summary>
        public ManifestSearchRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestSearchRequest"/> class.
        /// </summary>
        /// <param name="manifestSearchRequest">ManifestSearchRequest.</param>
        public ManifestSearchRequest(ManifestSearchRequest manifestSearchRequest)
        {
            this.Update(manifestSearchRequest);
        }

        /// <summary>
        /// Gets or sets MaximumResults.
        /// </summary>
        public int? MaximumResults { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to FetchAllManifests.
        /// </summary>
        public bool? FetchAllManifests { get; set; }

        /// <summary>
        /// Gets or sets Query.
        /// </summary>
        public SearchRequestMatch? Query { get; set; }

        /// <summary>
        /// Gets or sets Query.
        /// </summary>
        public SearchRequestPackageMatchFilter[]? Inclusions { get; set; }

        /// <summary>
        /// Gets or sets Query.
        /// </summary>
        public SearchRequestPackageMatchFilter[]? Filters { get; set; }

        /// <summary>
        /// Operator==.
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        /// <returns>Bool.</returns>
        public static bool operator ==(ManifestSearchRequest left, ManifestSearchRequest right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Operator!=.
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        /// <returns>Bool.</returns>
        public static bool operator !=(ManifestSearchRequest left, ManifestSearchRequest right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public void Update(ManifestSearchRequest obj)
        {
            this.MaximumResults = obj.MaximumResults;
            this.FetchAllManifests = obj.FetchAllManifests;
            this.Query = obj.Query;
            this.Inclusions = obj.Inclusions;
            this.Filters = obj.Filters;
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Create Validation Results
            var results = new List<ValidationResult>();

            // Verify Optional Fields
            if (this.Query != null)
            {
                //TODO: validate query inclusions and filters
                //ApiDataValidator.Validate(this.Query, results);
            }

            if (this.Inclusions != null)
            {
                //ApiDataValidator.Validate(this.Inclusions, results);
            }

            if (this.Filters != null)
            {
                //ApiDataValidator.Validate(this.Filters, results);
            }

            // Return Results
            return results;
        }

        /// <inheritdoc />
        public bool Equals(ManifestSearchRequest other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(this.MaximumResults, other.MaximumResults)
                   && Equals(this.FetchAllManifests, other.FetchAllManifests)
                   && Equals(this.Query, other.Query)
                   && Equals(this.Inclusions, other.Inclusions)
                   && Equals(this.Filters, other.Filters);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ManifestSearchRequest manifestSearchRequest && this.Equals(manifestSearchRequest);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.MaximumResults, this.FetchAllManifests, this.Query, this.Inclusions, this.Filters);
        }
    }
}
