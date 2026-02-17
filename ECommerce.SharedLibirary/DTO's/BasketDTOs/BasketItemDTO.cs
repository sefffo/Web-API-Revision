using System.ComponentModel.DataAnnotations;

namespace ECommerce.SharedLibirary.DTO_s.BasketDTOs
{
    public record BasketItemDTO(int id , string ProductName , string PictureUrl ,
        [Range(1,50)]int Quantity ,
        [Range(1,900000)]decimal price);
}