using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.SharedLibirary.DTO_s.IdentityDTOs
{
    public record UserDTO(string Email, string DisplayName ,string Token);
}
