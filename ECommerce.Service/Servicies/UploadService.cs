using ECommerce.Services.Abstraction;
using Microsoft.AspNetCore.Hosting;

namespace ECommerce.Services.Servicies
{
    public class UploadService(IWebHostEnvironment hostEnvironment) : IUploadService
    {
        // HashSet for O(1) lookup — built once, shared across all calls
        // static readonly = created once at class level, not per request
        private static readonly HashSet<string> _allowedExtensions
            = new() { ".jpg", ".jpeg", ".png", ".webp" };

        public async Task<string> UploadFileAsync(Stream imageStream, string fileName)
        {
            // ─── STEP 1: Validate wwwroot exists ────────────────────────────
            // WebRootPath is the full disk path to wwwroot
            // It can be null if UseStaticFiles() isn't configured in Program.cs
            if (string.IsNullOrEmpty(hostEnvironment.WebRootPath))
                throw new InvalidOperationException(
                    "wwwroot is not configured. Make sure app.UseStaticFiles() is called.");

            // ─── STEP 2: Validate file extension ────────────────────────────
            // Path.GetExtension("nike.jpg") → ".jpg"
            // ToLowerInvariant() → culture-safe lowercase (safer than ToLower())
            var extension = Path.GetExtension(fileName).ToLowerInvariant();


            if (imageStream.Length > 10 * 1024 * 1024) // 10MB max
                throw new InvalidOperationException("File size cannot exceed 10MB.");

            // HashSet.Contains() is O(1) — instant lookup
            if (!_allowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    $"File type '{extension}' is not allowed. Allowed types: jpg, jpeg, png, webp.");

            // ─── STEP 3: Generate unique file name ───────────────────────────
            // Guid.NewGuid() generates a universally unique identifier
            // "nike.jpg" → "a3f7c1d2-9b4e-4f6a-8c2d-1e5f7a9b3c4d.jpg"
            // This prevents any file from ever overwriting another
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // ─── STEP 4: Build the folder path on disk ───────────────────────
            // Path.Combine joins paths safely (handles slashes for you)
            // Result: "C:\MyProject\wwwroot\images\products"
            var uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images", "products");

            // ─── STEP 5: Ensure the folder exists ───────────────────────────
            // Creates the folder (and any missing parent folders) if it doesn't exist
            // Does nothing if it already exists — safe to call every time
            Directory.CreateDirectory(uploadsFolder);

            // ─── STEP 6: Build the full disk path for the new file ───────────
            // Result: "C:\MyProject\wwwroot\images\products\{guid}.jpg"
            // This is where the actual bytes will be written on disk
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            // ─── STEP 7: Write the stream bytes to disk ──────────────────────
            // FileStream opens/creates the file at fullPath for writing
            // FileMode.Create → creates new file, overwrites if somehow exists
            // "await using" → async dispose, closes the file handle when done
            // CopyToAsync → reads from imageStream and writes to fileStream async
            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await imageStream.CopyToAsync(fileStream);

            // ─── STEP 8: Return the relative URL ─────────────────────────────
            // This is NOT the disk path — it's the web-accessible URL
            // Disk:  C:\MyProject\wwwroot\images\products\{guid}.jpg
            // URL:   /images/products/{guid}.jpg
            // The browser uses this URL to fetch the image via static files middleware
            return $"/images/products/{uniqueFileName}";
        }
    }
}