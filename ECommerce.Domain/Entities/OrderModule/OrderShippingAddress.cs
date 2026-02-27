using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities.OrderModule
{
    public class OrderShippingAddress
    {
        public string City { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string FirstName { get; set; } = default!; //meen el hystlm el order 
        public string LastName { get; set; } = default!;

    }
}
