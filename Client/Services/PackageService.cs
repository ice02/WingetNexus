using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WingetNexus.Client.Services
{
    public class PackageService : IPackageService
    {
        private HttpClient? _httpClient;
        private readonly string _apiversion = "v2";

        public PackageService(IAntiforgeryHttpClientFactory antiforgeryHttpClientFactory)
        {
            try
            {
                antiforgeryHttpClientFactory.CreateClientAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted || task.Result == null)
                    {
                        throw new InvalidOperationException("Failed to create HttpClient.", task.Exception);
                    }

                    _httpClient = task.Result;
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while initializing the PackageService.", ex);
            }
        }

        public async Task<ApplicationDto> GetPackageAsync(string id)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var result = await _httpClient.GetFromJsonAsync<ApplicationDto>($"api/{_apiversion}/packages/" + id);
            if (result == null)
            {
                throw new NullReferenceException("The package data could not be retrieved.");
            }
            return result;
        }

        public async Task<PackageListDto> GetPackagesFilteredAsync(GridDataRequestDto request)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var result = new PackageListDto();

            try
            {
                HttpResponseMessage? res = null;

                if (string.IsNullOrEmpty(request.SearchTerm))
                {
                    res = await _httpClient.GetAsync($"api/{_apiversion}/packages?page={request.Page}&pageSize={request.PageSize}");
                }
                else
                {
                    res = await _httpClient.GetAsync($"api/{_apiversion}/packages?page={request.Page}&pageSize={request.PageSize}&filter={request.SearchTerm}");
                }

                if (res.IsSuccessStatusCode)
                {
                    result.Items = await res.Content.ReadFromJsonAsync<IEnumerable<ApplicationDto>>() ?? new List<ApplicationDto>();
                    var totalCountHeader = res.Headers.GetValues("X-Total-Count").FirstOrDefault();
                    result.ItemTotalCount = totalCountHeader != null ? int.Parse(totalCountHeader) : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching packages", ex);
            }

            return result;
        }

        public async Task DeletePackageAsync(string id)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var res = await _httpClient.DeleteAsync($"api/{_apiversion}/packages/{id}");

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception(res.ReasonPhrase);
            }
        }

        public async Task<bool> CheckPackageIdentifierUniquenessAsync(string identifier)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var response = await _httpClient.GetAsync($"api/{_apiversion}/packages/checkUnicity?identifier=" + identifier);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            throw new Exception("Error checking package identifier uniqueness.");
        }

        public async Task<IEnumerable<string>> SearchPublishersAsync(string value)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                return new List<string>();
            }

            var response = await _httpClient.GetAsync($"api/{_apiversion}/publishers/search/" + value);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
            }

            throw new Exception("Error searching publishers.");
        }

        public async Task<ApplicationDto> CreatePackageAsync(ApplicationDto application)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            var response = await _httpClient.PostAsJsonAsync($"api/{_apiversion}/packages", application);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApplicationDto>();
                if (result == null)
                {
                    throw new NullReferenceException("The created package data could not be retrieved.");
                }
                return result;
            }

            throw new Exception("Error creating package.");
        }

        public async Task<PublisherDto> CreatePublisherAsync(PublisherDto publisher)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            // Check if publisher with the same name exists
            var response = await _httpClient.GetAsync($"api/{_apiversion}/publishers/checkExists?name=" + publisher.Name);
            if (response.IsSuccessStatusCode)
            {
                var existingPublisher = await response.Content.ReadFromJsonAsync<PublisherDto>();
                if (existingPublisher != null)
                {
                    return existingPublisher;
                }
            }

            // Create the publisher if it does not exist
            var createResponse = await _httpClient.PostAsJsonAsync($"api/{_apiversion}/publishers", publisher);
            if (createResponse.IsSuccessStatusCode)
            {
                var createdPublisher = await createResponse.Content.ReadFromJsonAsync<PublisherDto>();
                if (createdPublisher == null)
                {
                    throw new NullReferenceException("The created publisher data could not be retrieved.");
                }
                return createdPublisher;
            }

            throw new Exception("Error creating publisher.");
        }

        public async Task<ApplicationDto> CreateApplicationAsync(ApplicationDto application, string versionNumber)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Call InitializeHttpClientAsync first.");
            }

            // Check if application with the same ID and version exists
            var response = await _httpClient.GetAsync($"api/{_apiversion}/applications/checkExists?packageIdentifier={application.PackageIdentifier}&version={versionNumber}");
            if (response.IsSuccessStatusCode)
            {
                var existingApplication = await response.Content.ReadFromJsonAsync<ApplicationDto>();
                if (existingApplication != null)
                {
                    return existingApplication;
                }
            }

            // Create the application if it does not exist
            var createResponse = await _httpClient.PostAsJsonAsync($"api/{_apiversion}/applications", application);
            if (createResponse.IsSuccessStatusCode)
            {
                var createdApplication = await createResponse.Content.ReadFromJsonAsync<ApplicationDto>();
                if (createdApplication == null)
                {
                    throw new NullReferenceException("The created application data could not be retrieved.");
                }
                return createdApplication;
            }

            throw new Exception("Error creating application.");
        }
    }
}