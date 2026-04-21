using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.CommonResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ECommerce.Presentation.Controllers
{
    public class UploadController(IUploadService uploadService) : ApiBaseController
    {
        

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UploadFile([FromForm]UploadImageDto uploadImageDto)
        {
            // 1. Check DTO + file
            if (uploadImageDto is null || uploadImageDto.Image is null)
                return BadRequest(Error.BadRequest("Upload.Failed", "No file uploaded."));

            // 2. Extract Stream + FileName from IFormFile → pass to service
            // This is the Choice C pattern — service stays HTTP-agnostic
            try
            {
                var relativeUrl = await uploadService.UploadFileAsync(
                    uploadImageDto.Image.OpenReadStream(),
                    uploadImageDto.Image.FileName
                );

                // 3. Return an ABSOLUTE URL so the product-creation DTO's [Url] validator accepts it,
                //    and the browser can fetch the image directly without prepending the API origin.
                //    Example: "https://web-api-revesion-xxx.azurewebsites.net/images/products/{guid}.jpg"
                var absoluteUrl = BuildAbsoluteUrl(relativeUrl);

                return Ok(Result<string>.Ok(absoluteUrl));
            }
            catch (InvalidOperationException ex)
            {
                // Catches invalid extension + wwwroot not configured errors
                return BadRequest(Error.BadRequest("Upload.Failed", ex.Message));
            }
        }

        /// <summary>
        /// Combines the incoming request's scheme + host with the relative path from the upload
        /// service. Using Request.Scheme/Host (instead of hard-coding the Azure URL) keeps the
        /// controller portable across local dev, Docker, and production.
        /// </summary>
        private string BuildAbsoluteUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return relativePath;

            // Already absolute? pass through unchanged.
            if (Uri.TryCreate(relativePath, UriKind.Absolute, out _))
                return relativePath;

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host.Value}";
            return relativePath.StartsWith('/')
                ? $"{baseUrl}{relativePath}"
                : $"{baseUrl}/{relativePath}";
        }
    }
}
