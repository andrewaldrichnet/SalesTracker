using Microsoft.JSInterop;
using SalesTracker.Shared.Services;

namespace SalesTracker.Web.Client.Services;

/// <summary>
/// WebAssembly implementation for image processing using Canvas API
/// </summary>
public class WebAssemblyImageProcessingService : IImageProcessingService
{
    private readonly IJSRuntime _jsRuntime;

    public WebAssemblyImageProcessingService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> ProcessImageToBase64Async(Stream imageStream, string fileName, int maxWidth = 400, int maxHeight = 400)
    {
        // Read the image into a byte array
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();

        // Convert to base64
        var base64String = Convert.ToBase64String(imageBytes);

        // Determine MIME type
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var mimeType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };

        var dataUri = $"data:{mimeType};base64,{base64String}";

        // Use JavaScript to resize the image via Canvas API
        try
        {
            var resizedDataUri = await _jsRuntime.InvokeAsync<string>(
                "window.imageResizer.resizeImage",
                dataUri,
                maxWidth,
                maxHeight
            );
            return resizedDataUri;
        }
        catch
        {
            // If resizing fails, return original data URI
            return dataUri;
        }
    }

    public Task<string> ResizeImageAndSaveAsync(string sourceFilePath, string destinationDirectory, int maxWidth = 400, int maxHeight = 400)
    {
        // WebAssembly can't save files to disk
        // This method shouldn't be called on the web client
        throw new NotSupportedException("File saving is not supported on WebAssembly. Use ProcessImageToBase64Async instead.");
    }
}
