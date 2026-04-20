using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Abstraction
{
    public interface IOrderService
    {
        Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string Email);
        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethodsAsync();
        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email);
        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersForAdminAsync();
        Task<Result<OrderToReturnDTO>> GetOrderById(Guid orderId);

        // payment related
        Task<bool> SaveInvoiceIdAsync(Guid orderId, string invoiceId);
        Task<bool> MarkOrderAsPaidAsync(string invoiceId);
    }
}
