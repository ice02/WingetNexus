using WingetNexus.Data.DataStores;
using WingetNexus.Shared.Models.Dtos;
using Microsoft.Data.Sqlite;

namespace WingetNexus.WingetApi.Services
{
    public class LocalCacheService : ILocalCacheService
    {
        public readonly ILogger<LocalCacheService> _logger;
        public readonly IConfiguration _configuration;
        public readonly IWingetAppDatastore _dataStore;

        public LocalCacheService(
            ILogger<LocalCacheService> logger,
            IConfiguration configuration,
            IWingetAppDatastore dataStore)
        {
            _logger = logger;
            _configuration = configuration;
            _dataStore = dataStore;
        }

        public async Task InitializeAsync()
        {
            var databasePath = _configuration.GetValue<string>("DatabasePath");
            if (string.IsNullOrEmpty(databasePath))
            {
                throw new ArgumentNullException("Database path is not configured.");
            }

            // Create SQLite connection
            using var connection = new SqliteConnection($"Data Source={databasePath}");
            await connection.OpenAsync();

            // Create table if it doesn't exist
            await CreateApplicationsTableAsync(connection);
        }

        // get source2.msix from cache folder if exist, if not send error
        public async Task<string> GetSource2MsixAsync()
        {
            var cachePath = _configuration.GetValue<string>("CachePath");
            if (string.IsNullOrEmpty(cachePath))
            {
                cachePath = ".\\cache"; // Default cache path
                //throw new ArgumentNullException("Cache path is not configured.");
            }

            var source2MsixPath = Path.Combine(cachePath, "source2.msix");
            if (!File.Exists(source2MsixPath))
            {
                throw new FileNotFoundException("source2.msix not found in cache folder.", source2MsixPath);
            }

            return source2MsixPath;
        }

        public async Task<List<ApplicationDto>> GetAllApplicationsAsync()
        {
            var filterDto = new FilterDto()
            {
                PageNumber = 1,
                PageSize = 1000,
                Filter = null,
                OrderBy = null,
                OrderWay = null
            };
            var result = await _dataStore.GetAllApplicationsAsync(filterDto);

            return result.ToList();
        }

        private async Task CreateApplicationsTableAsync(SqliteConnection connection)
        {
            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS Applications (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Version TEXT
                );";
            await createTableCommand.ExecuteNonQueryAsync();
        }

        private async Task InsertApplicationsAsync(SqliteConnection connection, List<ApplicationDto> applications)
        {
            // foreach (var app in applications)
            // {
            //     var insertCommand = connection.CreateCommand();
            //     insertCommand.CommandText = @"
            //         INSERT INTO Applications (Id, Name, Description, Version)
            //         VALUES (@Id, @Name, @Description, @Version);";

            //     insertCommand.Parameters.AddWithValue("@Id", app.Id);
            //     insertCommand.Parameters.AddWithValue("@Name", app.Name);
            //     insertCommand.Parameters.AddWithValue("@Description", app.Description ?? string.Empty);
            //     insertCommand.Parameters.AddWithValue("@Version", app.Version ?? string.Empty);

            //     await insertCommand.ExecuteNonQueryAsync();
            // }
        }

        public async Task GenerateSQLiteDatabaseAsync(string databasePath)
        {
            // Fetch all applications
            var applications = await GetAllApplicationsAsync();

            // Create SQLite connection
            using var connection = new SqliteConnection($"Data Source={databasePath}");
            await connection.OpenAsync();

            // Create table
            await CreateApplicationsTableAsync(connection);

            // Insert data into table
            await InsertApplicationsAsync(connection, applications);
        }
    }
}
