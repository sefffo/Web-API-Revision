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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadFile([FromForm]UploadImageDto uploadImageDto)
        {
            // 1. Check DTO + file
            if (uploadImageDto is null || uploadImageDto.Image is null)
                return BadRequest(Error.BadRequest("Upload.Failed", "No file uploaded."));

            // 2. Extract Stream + FileName from IFormFile → pass to service
            // This is the Choice C pattern — service stays HTTP-agnostic
            try
            {
                var url = await uploadService.UploadFileAsync(
                    uploadImageDto.Image.OpenReadStream(),
                    uploadImageDto.Image.FileName
                );

                // 3. Return the URL so client can use it in CreateProductDto.PictureUrl
                return Ok(Result<string>.Ok(url));
            }
            catch (InvalidOperationException ex)
            {
                // Catches invalid extension + wwwroot not configured errors
                return BadRequest(Error.BadRequest("Upload.Failed", ex.Message));
            }




        }
    }
}
