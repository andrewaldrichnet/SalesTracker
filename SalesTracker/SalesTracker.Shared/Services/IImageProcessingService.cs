namespace SalesTracker.Shared.Services;

/// <summary>
/// Service for processing images (resize, compress, etc.)
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Resizes an image stream to specified dimensions and returns base64 string
    /// </summary>
    /// <param name="imageStream">The image stream to process</param>
    /// <param name="fileName">Original file name (for extension)</param>
    /// <param name="maxWidth">Maximum width in pixels</param>
    /// <param name="maxHeight">Maximum height in pixels</param>
    /// <returns>Base64 data URI (data:image/jpeg;base64,...)</returns>
    Task<string> ProcessImageToBase64Async(Stream imageStream, string fileName, int maxWidth = 400, int maxHeight = 400);

    /// <summary>
    /// Resizes an image file and saves it to disk
    /// </summary>
    /// <param name="sourceFilePath">Path to source image file</param>
    /// <param name="destinationDirectory">Directory to save resized image</param>
    /// <param name="maxWidth">Maximum width in pixels</param>
    /// <param name="maxHeight">Maximum height in pixels</param>
    /// <returns>Path to the resized image file</returns>
    Task<string> ResizeImageAndSaveAsync(string sourceFilePath, string destinationDirectory, int maxWidth = 400, int maxHeight = 400);
}
