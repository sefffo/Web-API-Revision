using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.SharedLibirary.DTO_s.IdentityDTOs
{
    public record RegisterDTO([EmailAddress] string Email , string DisplayName , string UserName  , string Password ,[Phone] string PhoneNumber );
}
