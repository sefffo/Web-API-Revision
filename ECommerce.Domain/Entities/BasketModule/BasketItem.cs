using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities.BasketModule
{
    public class BasketItem
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = default!;

        public string pictureUrl { get; set; }=default!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

    }
}
