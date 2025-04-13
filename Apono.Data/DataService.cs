using System.Text.Json;
using Apono.Data.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Apono.Data;

public sealed class DataService(IMemoryCache cache, IConfiguration cfg) : IDataService
{
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private static readonly StringComparer Ci = StringComparer.OrdinalIgnoreCase;

    public async Task<DatasetDictionary> GetDataSetAsync(string datasetFile, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(datasetFile))
            throw new ArgumentException("Path must not be empty.", nameof(datasetFile));
        
        if (!File.Exists(datasetFile))
            throw new FileNotFoundException("Dataset file not found.", datasetFile);
        
        int cacheMinutes = Convert.ToInt32(cfg["CacheMinutes"] ?? string.Empty);
        
        var cacheKey = Path.GetFullPath(datasetFile);

        var cached = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheMinutes);

            await using var fs = File.Open(
                datasetFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            var dataset = await JsonSerializer.DeserializeAsync<DatasetFromJson>(fs, cancellationToken: ct)
                          ?? throw new InvalidDataException("Dataset is empty or malformed.");

            return new CachedDataset(
                dataset.citizens.ToDictionary(c => c.name, Ci),
                dataset.roles.ToDictionary(r => r.title, Ci));
        }) ?? throw new InvalidOperationException();

        var datasetDictionary = new DatasetDictionary
        {
            _citizens = cached.Citizens,
            _roles = cached.Roles
        };

        return datasetDictionary;
    }

    public async Task<bool> CanVisitAsync(string citizen, string place, DatasetDictionary datasetDictionary, CancellationToken ct = default) =>
        await Task.FromResult(CanVisitInternal(citizen, place, datasetDictionary));


    private bool CanVisitInternal(string citizenName, string place, DatasetDictionary datasetDictionary)
    {
        if (datasetDictionary._citizens.Count == 0 || datasetDictionary._roles.Count == 0)
            throw new InvalidOperationException("LoadDatasetAsync must be called before CanVisitAsync.");

        if (!datasetDictionary._citizens.TryGetValue(citizenName, out var citizen))
            return false; // unknown citizen

        var allowed = new HashSet<string>(citizen.allowed_places ?? Enumerable.Empty<string>(), Ci);
        var visited = new HashSet<string>(Ci);

        foreach (var role in citizen.roles ?? Enumerable.Empty<string>())
            CollectRolePlaces(role, datasetDictionary, visited, allowed);

        return allowed.Contains(place);
    }

    private void CollectRolePlaces(string roleTitle, DatasetDictionary datasetDictionary, HashSet<string> visited, HashSet<string> allowed)
    {
        var datasetDictionaryRoles = datasetDictionary._roles;
        if (!visited.Add(roleTitle) || !datasetDictionaryRoles.TryGetValue(roleTitle, out var role))
            return; // already processed or unknown role

        foreach (var p in role.allowed_places ?? Enumerable.Empty<string>())
            allowed.Add(p);

        foreach (var sub in role.sub_roles ?? Enumerable.Empty<string>())
            CollectRolePlaces(sub, datasetDictionary, visited, allowed);
    }
    private sealed record CachedDataset(
        Dictionary<string, Citizen> Citizens,
        Dictionary<string, Role> Roles);
}


