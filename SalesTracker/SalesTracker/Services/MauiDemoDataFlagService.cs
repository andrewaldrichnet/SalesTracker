using SalesTracker.Shared.Services;

namespace SalesTracker.Services;

/// <summary>
/// MAUI implementation of DemoDataFlagService using Preferences
/// </summary>
public class MauiDemoDataFlagService : DemoDataFlagService
{
    private const string DemoDataFlagKey = "salestracker_demo_data_loaded";

    protected override async Task<bool> GetFlagAsync()
    {
        try
        {
            var value = Preferences.Get(DemoDataFlagKey, false);
            return await Task.FromResult(value);
        }
        catch
        {
            return false;
        }
    }

    protected override async Task SetFlagAsync(bool value)
    {
        try
        {
            Preferences.Set(DemoDataFlagKey, value);
            await Task.CompletedTask;
        }
        catch
        {
            // Silently fail if Preferences is not available
        }
    }
}
