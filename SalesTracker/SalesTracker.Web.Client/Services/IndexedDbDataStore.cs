using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.JSInterop;
using SalesTracker.Shared.Data;

namespace SalesTracker.Web.Client.Data;

/// <summary>
/// IndexedDB implementation of IDataStore for browser-based storage.
/// Uses JavaScript interop to interact with the browser's IndexedDB API.
/// </summary>
public class IndexedDbDataStore<T> : IDataStore<T> where T : class
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _storeName;
    private readonly string _databaseName = "SalesTrackerDB";
    private readonly int _databaseVersion = 1;
    private IJSObjectReference? _module;

    public IndexedDbDataStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _storeName = typeof(T).Name.ToLowerInvariant();
    }

    private async Task EnsureDbInitializedAsync()
    {
        if (_module != null) return;

        try
        {
            // Use absolute path from application root for hybrid Blazor app
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "/Data/indexeddb.js");
            
            // Initialize all required object stores upfront
            // This ensures all stores are created during database initialization
            var storeNames = new[] { "item", "order", "itemimage" };
            await _module.InvokeVoidAsync("initializeDatabase", _databaseName, _databaseVersion, storeNames);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load IndexedDB module. Ensure indexeddb.js is available. " +
                "The file should be at: wwwroot/Data/indexeddb.js", ex);
        }
    }

    public async Task<List<T>> GetAllAsync()
    {
        await EnsureDbInitializedAsync();
        var json = await _module!.InvokeAsync<string>("getAll", _databaseName, _databaseVersion, _storeName);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        await EnsureDbInitializedAsync();
        var json = await _module!.InvokeAsync<string?>("getById", _databaseName, _databaseVersion, _storeName, id);
        return json != null ? JsonSerializer.Deserialize<T>(json) : null;
    }

    public async Task<int> AddAsync(T entity)
    {
        await EnsureDbInitializedAsync();
        
        var json = JsonSerializer.Serialize(entity);
        var result = await _module!.InvokeAsync<int>("add", _databaseName, _databaseVersion, _storeName, json);
        
        return result;
    }

    public async Task UpdateAsync(T entity)
    {
        await EnsureDbInitializedAsync();
        var json = JsonSerializer.Serialize(entity);
        await _module!.InvokeAsync<object?>("update", _databaseName, _databaseVersion, _storeName, json);
    }

    public async Task DeleteAsync(int id)
    {
        await EnsureDbInitializedAsync();
        await _module!.InvokeAsync<object?>("delete", _databaseName, _databaseVersion, _storeName, id);
    }

    public async Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate)
    {
        // For IndexedDB, we fetch all records and filter in-memory using LINQ
        // This is acceptable for offline-first apps with reasonable data sizes
        var allItems = await GetAllAsync();
        return allItems.AsQueryable().Where(predicate).ToList();
    }
}
