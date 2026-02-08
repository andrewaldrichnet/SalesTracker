using SalesTracker.Shared.Services;

namespace SalesTracker.Web.Services;

/// <summary>
/// Web implementation for image file storage
/// </summary>
public class WebImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _environment;

    public WebImageStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string GetImagesDirectory()
    {
        var imagesDir = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "items");
        Directory.CreateDirectory(imagesDir);
        return imagesDir;
    }

    public bool ImageExists(string imagePath)
    {
        return !string.IsNullOrEmpty(imagePath) && File.Exists(imagePath);
    }

    public string GetImageUri(string imagePath)
    {
        // On web, convert file path to relative URL
        if (File.Exists(imagePath))
        {
            var wwwrootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            var relativePath = Path.GetRelativePath(wwwrootPath, imagePath);
            return "/" + relativePath.Replace("\\", "/");
        }
        return string.Empty;
    }
}
