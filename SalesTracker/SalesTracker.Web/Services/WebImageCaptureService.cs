using SalesTracker.Shared.Services;

namespace SalesTracker.Web.Services;

/// <summary>
/// Web implementation for image capture (camera not supported on web)
/// </summary>
public class WebImageCaptureService : IImageCaptureService
{
    public bool SupportsCamera => false;

    public Task<string?> CapturePhotoAsync()
    {
        return Task.FromResult<string?>(null);
    }
}
