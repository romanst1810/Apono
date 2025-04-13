using Apono.Data;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using FluentAssertions;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace DataServiceTests
{
    public sealed class DataServiceTests
    {
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "CacheMinutes", "5" },
                { "FileName", "apalula.json" }
            })
            .Build();
        private readonly DataService dataService;

        public DataServiceTests() => dataService = new DataService(_cache, _configuration);

        [Fact]
        public async Task CanVisit_succeeds_with_real_json_file()
        {
            string fileName = _configuration["FileName"] ?? string.Empty;
            string jsonPath = Path.Combine(AppContext.BaseDirectory, fileName);

            // Sanity‑check the file exists so the test fails clearly if someone deletes it
            File.Exists(jsonPath).Should().BeTrue($"test data file {jsonPath} must be present");

            // Load once (populates 5‑minute cache)
            var dataset = await dataService.GetDataSetAsync(jsonPath, CancellationToken.None);

            var results = new[]
            {
                await dataService.CanVisitAsync("Aco", "Armory",dataset, CancellationToken.None),
                await dataService.CanVisitAsync("Aro", "City Wall",dataset, CancellationToken.None),
                await dataService.CanVisitAsync("Baco", "Storage",dataset, CancellationToken.None)
            };

            results.Should().Equal(false, true, true);
        }
    }
}