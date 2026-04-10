namespace ECommerce.SharedLibirary.DTO_s.BasketDTOs
{
    public record BasketDTO(string id, ICollection<BasketItemDTO> Items);
    //we are using record and its immutable  ==> VI 

    //best practice for the dtos 
    //also its value comparison 

}
