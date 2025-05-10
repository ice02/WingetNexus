using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using WingetNexus.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WingetNexus.Client.Services
{
    public class ContentFileService : IContentFileService
    {
        private HttpClient _httpClient;
        private readonly IAntiforgeryHttpClientFactory _httpClientFactory;

        private readonly string _apiversion = "v2";

        public ContentFileService(IAntiforgeryHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task InitializeHttpClientAsync(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ContentFileDto> GetContentFileAsync(int id)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var result = await _httpClient.GetFromJsonAsync<ContentFileDto>($"api/{_apiversion}/contentfiles/" + id);
            if (result == null)
            {
                throw new NullReferenceException("The content file data could not be retrieved.");
            }
            return result;
        }

        public async Task SaveContentFileAsync(int id, string content)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var response = await _httpClient.PutAsJsonAsync($"api/{_apiversion}/contentfiles/" + id, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error saving content file.");
            }
        }

        public async Task DeleteContentFileAsync(int id)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var res = await _httpClient.DeleteAsync($"api/{_apiversion}/contentfiles/" + id);

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception(res.ReasonPhrase);
            }
        }
    }
}