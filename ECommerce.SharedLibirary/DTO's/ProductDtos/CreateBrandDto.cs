using System.ComponentModel.DataAnnotations;

namespace ECommerce.SharedLibirary.DTO_s.ProductDtos
{
    public class CreateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required.")]
        [MaxLength(100, ErrorMessage = "Brand name cannot exceed 100 characters.")]
        public string Name { get; set; }=null!;
    }
}
