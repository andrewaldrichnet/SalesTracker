namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for managing demo data flag in localStorage
/// </summary>
public class DemoDataFlagService
{
    private const string DemoDataFlagKey = "salestracker_demo_data_loaded";

    /// <summary>
    /// Checks if demo data has been loaded
    /// </summary>
    public async Task<bool> IsDemoDataLoadedAsync()
    {
        // This will be implemented in platform-specific services
        return await GetFlagAsync();
    }

    /// <summary>
    /// Sets the demo data flag to indicate demo data is loaded
    /// </summary>
    public async Task SetDemoDataLoadedAsync()
    {
        await SetFlagAsync(true);
    }

    /// <summary>
    /// Clears the demo data flag
    /// </summary>
    public async Task ClearDemoDataFlagAsync()
    {
        await SetFlagAsync(false);
    }

    // These methods will be overridden by platform-specific implementations
    protected virtual async Task<bool> GetFlagAsync()
    {
        return await Task.FromResult(false);
    }

    protected virtual async Task SetFlagAsync(bool value)
    {
        await Task.CompletedTask;
    }
}
