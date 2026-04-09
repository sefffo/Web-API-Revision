

namespace ECommerce.Services.Abstraction
{
    public interface IUploadService
    {
        Task<string> UploadFileAsync(Stream ImageStream, string fileName);
    }
}
