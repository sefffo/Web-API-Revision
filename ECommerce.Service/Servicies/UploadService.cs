using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Services.Servicies
{
    /// <summary>
    /// Uploads product images to Azure Blob Storage. Replaces the old wwwroot-based
    /// implementation because App Service's container filesystem is ephemeral — files
    /// written under wwwroot vanish on redeploys, scale events, and restarts.
    ///
    /// Blob storage gives us:
    ///  • durable storage independent of the app server lifecycle
    ///  • a public, CDN-cacheable URL we can store directly in Product.PictureUrl
    ///  • horizontal-scale safe (any replica can serve the same URL)
    /// </summary>
    public class UploadService : IUploadService
    {
        // HashSet for O(1) extension lookup — safer than a List<string>.Contains().
        private static readonly HashSet<string> _allowedExtensions
            = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        // Maps the extensions above to the correct HTTP content-type so the browser
        // renders the blob inline (as an image) instead of downloading it.
        private static readonly Dictionary<string, string> _contentTypes
            = new(StringComparer.OrdinalIgnoreCase)
            {
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".png"] = "image/png",
                [".webp"] = "image/webp",
                [".gif"] = "image/gif",
            };

        private readonly BlobContainerClient _container;

        public UploadService(IOptions<AzureBlobStorageSettings> settings)
        {
            var cfg = settings.Value
                ?? throw new InvalidOperationException("AzureBlobStorage settings are missing.");

            if (string.IsNullOrWhiteSpace(cfg.ConnectionString))
                throw new InvalidOperationException(
                    "AzureBlobStorage:ConnectionString is not configured. Set it in appsettings.json or the AzureBlobStorage__ConnectionString environment variable.");

            if (string.IsNullOrWhiteSpace(cfg.ContainerName))
                throw new InvalidOperationException("AzureBlobStorage:ContainerName is not configured.");

            var service = new BlobServiceClient(cfg.ConnectionString);
            _container = service.GetBlobContainerClient(cfg.ContainerName);

            // CreateIfNotExists + PublicAccessType.Blob lets anyone read individual blobs
            // (the <img> tag in the dashboard can hit the URL without auth) but still
            // prevents container-level listing. This is the standard pattern for public assets.
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadFileAsync(Stream imageStream, string fileName)
        {
            // ---- 1. Validate extension ----
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}.");

            // ---- 2. Validate size (10 MB) ----
            // Only trust Length when the stream supports it; multipart IFormFile streams do.
            if (imageStream.CanSeek && imageStream.Length > 10 * 1024 * 1024)
                throw new InvalidOperationException("File size cannot exceed 10MB.");

            // ---- 3. Pick a unique blob name inside a "products/" virtual folder ----
            // Prefixing with "products/" keeps the container tidy if we ever add other
            // asset types (e.g. user avatars, category thumbnails).
            var blobName = $"products/{Guid.NewGuid()}{extension}";

            var blob = _container.GetBlobClient(blobName);

            // ---- 4. Upload the stream ----
            // Setting the ContentType header is what makes the browser render the URL
            // as an image instead of triggering a download.
            var headers = new BlobHttpHeaders
            {
                ContentType = _contentTypes.TryGetValue(extension, out var ct) ? ct : "application/octet-stream",
                CacheControl = "public, max-age=31536000, immutable",
            };

            await blob.UploadAsync(imageStream, new BlobUploadOptions { HttpHeaders = headers });

            // ---- 5. Return the public URL ----
            // blob.Uri is the fully-qualified https URL that passes the [Url] data
            // annotation on CreateProductDto and can be rendered directly by <img>.
            return blob.Uri.ToString();
        }
    }
}
