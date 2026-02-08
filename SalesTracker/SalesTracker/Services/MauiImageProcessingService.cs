using SalesTracker.Shared.Services;

namespace SalesTracker.Services;

/// <summary>
/// MAUI implementation for image processing
/// </summary>
public class MauiImageProcessingService : IImageProcessingService
{
    public async Task<string> ProcessImageToBase64Async(Stream imageStream, string fileName, int maxWidth = 400, int maxHeight = 400)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // For MAUI on Windows, we'll save to disk and use file:// URI instead of base64
            // But for consistency with interface, return base64
            var resizedBytes = await ResizeImageAsync(memoryStream, maxWidth, maxHeight);
            var base64String = Convert.ToBase64String(resizedBytes);
            
            // Determine MIME type from file extension
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            var mimeType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            return $"data:{mimeType};base64,{base64String}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error processing image to base64: {ex}");
            throw;
        }
    }

    public async Task<string> ResizeImageAndSaveAsync(string sourceFilePath, string destinationDirectory, int maxWidth = 400, int maxHeight = 400)
    {
        try
        {
            Directory.CreateDirectory(destinationDirectory);

            using (var sourceStream = File.OpenRead(sourceFilePath))
            {
                var resizedBytes = await ResizeImageAsync(sourceStream, maxWidth, maxHeight);
                
                var fileName = Path.GetFileName(sourceFilePath);
                var destinationPath = Path.Combine(destinationDirectory, fileName);
                
                await File.WriteAllBytesAsync(destinationPath, resizedBytes);
                return destinationPath;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error resizing and saving image: {ex}");
            throw;
        }
    }

    private async Task<byte[]> ResizeImageAsync(Stream imageStream, int maxWidth, int maxHeight)
    {
        try
        {
            imageStream.Position = 0;
            
            // Read original image into memory
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            var originalBytes = memoryStream.ToArray();

            System.Diagnostics.Debug.WriteLine($"Image size: {originalBytes.Length} bytes");

            // For actual image resizing on MAUI, you would need to:
            // 1. Add NuGet package: SixLabors.ImageSharp
            // 2. Use ImageSharp to decode, resize, and encode the image
            // 
            // Example with ImageSharp:
            // using (var image = Image.Load(imageStream))
            // {
            //     image.Mutate(x => x.Resize(new ResizeOptions
            //     {
            //         Size = new Size(maxWidth, maxHeight),
            //         Mode = ResizeMode.Max
            //     }));
            //     using (var output = new MemoryStream())
            //     {
            //         image.SaveAsJpeg(output);
            //         return output.ToArray();
            //     }
            // }

            // For now, return original bytes
            // Images will be stored unresized (but typically won't be huge from camera)
            System.Diagnostics.Debug.WriteLine("Returning original image (install SixLabors.ImageSharp for resizing support)");
            return originalBytes;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ResizeImageAsync: {ex}");
            imageStream.Position = 0;
            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }

    private async Task<byte[]> ResizeWithMauiAsync(byte[] imageBytes, int maxWidth, int maxHeight)
    {
        // This method is no longer used
        // Image resizing requires SixLabors.ImageSharp package
        return imageBytes;
    }

    private (int width, int height) CalculateDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
    {
        // If image is smaller than max, don't resize
        if (originalWidth <= maxWidth && originalHeight <= maxHeight)
            return (originalWidth, originalHeight);

        // Calculate aspect ratio
        var aspectRatio = (double)originalWidth / originalHeight;

        // Calculate new dimensions
        if (originalWidth > originalHeight)
        {
            // Landscape orientation
            var newWidth = Math.Min(originalWidth, maxWidth);
            var newHeight = (int)(newWidth / aspectRatio);
            return (newWidth, newHeight);
        }
        else
        {
            // Portrait orientation
            var newHeight = Math.Min(originalHeight, maxHeight);
            var newWidth = (int)(newHeight * aspectRatio);
            return (newWidth, newHeight);
        }
    }
}

