using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class UploadImageDto
{
    [Required]
    public IFormFile Image { get; set; } = null!;
}