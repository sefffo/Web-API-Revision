using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.SharedLibirary.DTO_s.IdentityDTOs
{
    public record LoginDTO([EmailAddress]string Email , string Password);
}
