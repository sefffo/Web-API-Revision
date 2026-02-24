using Microsoft.AspNetCore.Identity;

namespace ECommerce.Domain.Entities.IdentityModule
{
    public class AppUser : IdentityUser
    {

        public string DisplayName { get; set; } = default!;

        public Address? Address { get; set; } 
        //Navigation property for the user's address  
        //(the address during the During the Registration  is not needed at the moment but when placing order is needed)


        
    }
}
 