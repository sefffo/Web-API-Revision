using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities.OrderModule
{
    public enum OrderStatus : byte
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        PaymentPending = 6,   // ← ADD: order created, waiting for payment
        Paid = 7              // ← ADD: Fawaterak confirmed payment
    }
}
