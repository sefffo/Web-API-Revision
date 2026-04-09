using System.ComponentModel.DataAnnotations;

namespace ECommerce.SharedLibirary.DTO_s.ProductDtos
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(500)]
        [MinLength(20)]
        public string Description { get; set; } = null!;

        [Required]
        [Url]
        [MaxLength(300)]
        public string PictureUrl { get; set; } = null!;
        [Required]
        [DataType(DataType.Currency)]
        [Range(10,900000)]
        public decimal Price { get; set; }


        [Required]
        public string ProductType { get; set; } = null!;
        [Required]
        public string ProductBrand { get; set; } = null!;
    }
}
