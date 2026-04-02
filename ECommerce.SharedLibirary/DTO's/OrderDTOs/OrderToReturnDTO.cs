using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.SharedLibirary.DTO_s.OrderDTOs
{
    public class OrderToReturnDTO
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public ICollection<OrderItemDTO> OrderItems { get; set; }
        public AddressDTO Address { get; set; }
        public string DeliveryMethod { get; set; }
        public string OrderStatus { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string? PaymentInvoiceId { get; set; }//for the payment callback to find the order and update its status
    }
}
