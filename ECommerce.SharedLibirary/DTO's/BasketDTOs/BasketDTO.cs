using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.SharedLibirary.DTO_s.BasketDTOs
{
    public record BasketDTO(string id, ICollection<BasketItemDTO> Items); //we are using record and its immutable 

    //best practice for the dtos 
    //also its value comparison 
   
}
