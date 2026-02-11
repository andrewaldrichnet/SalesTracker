using Microsoft.JSInterop;
using SalesTracker.Shared.Services;

namespace SalesTracker.Web.Client.Services;

/// <summary>
/// WebAssembly implementation of DemoDataFlagService using localStorage
/// </summary>
public class WebAssemblyDemoDataFlagService : DemoDataFlagService
{
    private readonly IJSRuntime _jsRuntime;
    private const string DemoDataFlagKey = "salestracker_demo_data_loaded";

    public WebAssemblyDemoDataFlagService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<bool> GetFlagAsync()
    {
        try
        {
            var value = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", DemoDataFlagKey);
            return value == "true";
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
            if (value)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", DemoDataFlagKey, "true");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", DemoDataFlagKey);
            }
        }
        catch
        {
            // Silently fail if localStorage is not available
        }
    }
}
