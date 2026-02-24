using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities.IdentityModule
{
    public class Address
    {
        public int id { get; set; } //pk

        public string City { get; set; } = default!;

        public string Street { get; set; } = default!;

        public string Country { get; set; } = default!;
        public string FirstName { get; set; } = default!; //meen el hystlm el order 
        public string LastName { get; set; } = default!;



        public AppUser user { get; set; } = default!; //Navigation property to the user who owns this address

        public string UserId { get; set; }= default!; //pk and fk to the AppUser table, linking the address to a specific user

    }
}
