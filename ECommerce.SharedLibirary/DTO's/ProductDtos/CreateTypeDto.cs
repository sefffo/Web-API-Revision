using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.SharedLibirary.DTO_s.ProductDtos
{
    public class CreateTypeDto
    {
        [Required(ErrorMessage = "Type name is required.")]
        [MaxLength(100, ErrorMessage = "Type name cannot exceed 100 characters.")]
        public string Name { get; set; }=null!;
    }
}
