namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for capturing images from various sources (camera, gallery, etc.)
/// </summary>
public interface IImageCaptureService
{
    /// <summary>
    /// Indicates whether the device supports camera capture
    /// </summary>
    bool SupportsCamera { get; }

    /// <summary>
    /// Captures a photo using the device camera and returns the file path
    /// </summary>
    /// <returns>The full file path to the captured photo, or null if cancelled/failed</returns>
    Task<string?> CapturePhotoAsync();
}
