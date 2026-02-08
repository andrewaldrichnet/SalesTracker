namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for managing image file storage across platforms
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Gets the directory path where images are stored
    /// </summary>
    string GetImagesDirectory();

    /// <summary>
    /// Checks if an image file exists at the given path
    /// </summary>
    bool ImageExists(string imagePath);

    /// <summary>
    /// Gets the full URL/URI for displaying an image
    /// </summary>
    string GetImageUri(string imagePath);
}
