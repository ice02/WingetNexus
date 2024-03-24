using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Models.Winget._1._7
{
    /// <summary>
    /// This will wrap API responses that need additional data.
    /// </summary>
    /// <typeparam name="T">Response Type.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="continuationToken">Continuation Token.</param>
        public ApiResponse(T data, string continuationToken = null)
        {
            this.Data = data;
            this.ContinuationToken = continuationToken;
        }

        /// <summary>
        /// Gets or sets items.
        /// </summary>
        [JsonProperty(Order = 1)]
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets continuation Token.
        /// </summary>
        [JsonProperty(Order = 2)]
        public string ContinuationToken { get; set; }
    }
}
