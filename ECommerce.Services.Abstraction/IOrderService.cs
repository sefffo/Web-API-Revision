using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Abstraction
{
    public interface IOrderService
    {
        public Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string Email);
        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethodsAsync();
        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email);
        Task<Result<OrderToReturnDTO>> GetOrderById(Guid orderId);

        // payment related methods
        Task<bool> MarkOrderAsPaidAsync(string invoiceId);
        Task<bool> SetOrderInvoiceIdAsync(Guid orderId, string invoiceId);
    }
}
