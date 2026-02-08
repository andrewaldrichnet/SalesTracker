using SalesTracker.Shared.Services;

namespace SalesTracker.Services;

/// <summary>
/// MAUI implementation for image file storage
/// </summary>
public class MauiImageStorageService : IImageStorageService
{
    public string GetImagesDirectory()
    {
        var imagesDir = Path.Combine(FileSystem.AppDataDirectory, "images", "items");
        Directory.CreateDirectory(imagesDir);
        return imagesDir;
    }

    public bool ImageExists(string imagePath)
    {
        return !string.IsNullOrEmpty(imagePath) && File.Exists(imagePath);
    }

    public string GetImageUri(string imagePath)
    {
        // If it's a base64 data URI, return it as-is
        if (imagePath.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return imagePath;
        }

        // On MAUI, use the file path directly for display
        if (File.Exists(imagePath))
        {
            return $"file://{imagePath}";
        }
        return string.Empty;
    }
}
