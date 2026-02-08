using SalesTracker.Shared.Services;

namespace SalesTracker.Services;

/// <summary>
/// MAUI implementation for capturing images using the device camera
/// </summary>
public class MauiImageCaptureService : IImageCaptureService
{
    public bool SupportsCamera => MediaPicker.Default.IsCaptureSupported;

    public async Task<string?> CapturePhotoAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Starting camera capture...");
            
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null)
            {
                System.Diagnostics.Debug.WriteLine("Camera returned null");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"Photo captured: {photo.FileName}");

            // Copy the file to app data directory for permanent storage
            var appImagesDir = Path.Combine(FileSystem.AppDataDirectory, "images", "items");
            System.Diagnostics.Debug.WriteLine($"Creating directory: {appImagesDir}");
            Directory.CreateDirectory(appImagesDir);
            
            var newFileName = $"{Guid.NewGuid()}_{photo.FileName}";
            var newFilePath = Path.Combine(appImagesDir, newFileName);
            
            System.Diagnostics.Debug.WriteLine($"Saving photo to: {newFilePath}");
            
            using (var sourceStream = await photo.OpenReadAsync())
            using (var destStream = File.Create(newFilePath))
            {
                await sourceStream.CopyToAsync(destStream);
            }
            
            // Verify file was created
            if (File.Exists(newFilePath))
            {
                var fileSize = new FileInfo(newFilePath).Length;
                System.Diagnostics.Debug.WriteLine($"Photo saved successfully. Size: {fileSize} bytes");
                return newFilePath;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: File was not created at {newFilePath}");
                return null;
            }
        }
        catch (FeatureNotSupportedException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Camera not supported: {ex.Message}");
            return null;
        }
        catch (PermissionException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Permission denied: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error capturing photo: {ex}");
            return null;
        }
    }
}

