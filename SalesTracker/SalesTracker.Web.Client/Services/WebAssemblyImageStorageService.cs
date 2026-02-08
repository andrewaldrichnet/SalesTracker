using SalesTracker.Shared.Services;

namespace SalesTracker.Web.Client.Services;

/// <summary>
/// WebAssembly client implementation for image file storage
/// </summary>
public class WebAssemblyImageStorageService : IImageStorageService
{
    public string GetImagesDirectory()
    {
        // On WebAssembly, images are served from the server via HTTP
        return "/images/items";
    }

    public bool ImageExists(string imagePath)
    {
        // On WebAssembly, we can't directly check file existence
        // The server handles this
        return !string.IsNullOrEmpty(imagePath);
    }

    public string GetImageUri(string imagePath)
    {
        // If it's a base64 data URI, return it as-is
        if (imagePath.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return imagePath;
        }

        // If the path is already a URL, return it
        if (imagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase) || 
            imagePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        {
            return imagePath;
        }

        // If it's a file path from the server, serve it via the /images endpoint
        if (imagePath.Contains("images", StringComparison.OrdinalIgnoreCase))
        {
            var fileName = Path.GetFileName(imagePath);
            return $"/images/items/{fileName}";
        }

        return string.Empty;
    }
}
