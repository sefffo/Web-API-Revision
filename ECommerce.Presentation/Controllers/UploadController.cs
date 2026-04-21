using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.CommonResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    public class UploadController(IUploadService uploadService) : ApiBaseController
    {
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UploadFile([FromForm] UploadImageDto uploadImageDto)
        {
            // 1. Validate DTO + file
            if (uploadImageDto is null || uploadImageDto.Image is null)
                return BadRequest(Error.BadRequest("Upload.Failed", "No file uploaded."));

            // 2. Delegate to the service. UploadService now stores the file in Azure Blob
            //    Storage and returns a fully-qualified https URL (e.g.
            //    https://{account}.blob.core.windows.net/product-images/products/{guid}.jpg),
            //    so the controller no longer needs to build an absolute URL from Request.Host.
            try
            {
                var url = await uploadService.UploadFileAsync(
                    uploadImageDto.Image.OpenReadStream(),
                    uploadImageDto.Image.FileName
                );

                return Ok(Result<string>.Ok(url));
            }
            catch (InvalidOperationException ex)
            {
                // Catches invalid extension, oversized file, and missing-config errors.
                return BadRequest(Error.BadRequest("Upload.Failed", ex.Message));
            }
        }
    }
}
