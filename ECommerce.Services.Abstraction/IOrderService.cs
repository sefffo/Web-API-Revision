using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Abstraction
{
    public interface IOrderService
    {
        // we will get the Email address from the JWT token and then we will get the user id from the database and then we will create the order for that user id
        //OrderDto CreateOrder(OrderCreateDto orderCreateDto);
        //OrderToReturnDto GetOrderById(int orderId);
        public Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string Email);

        //public Task<Result<IReadOnlyList<OrderToReturnDTO>>> GetOrdersForUserAsync(string userEmail);
        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethodsAsync();
        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email);
        Task<Result<OrderToReturnDTO>> GetOrderById(Guid orderId);

        //payment related methods
        Task<bool> MarkOrderAsPaidAsync(string invoiceId);


    }
}
