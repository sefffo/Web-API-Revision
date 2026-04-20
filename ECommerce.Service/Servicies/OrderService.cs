using AutoMapper;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Domain.Interfaces;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;

namespace ECommerce.Services.Servicies
{
    public class OrderService(IMapper mapper, IBasketRepository basketRepository, IUnitOfWork unitOfWork) : IOrderService
    {
        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string Email)
        {
            var orderAddress = mapper.Map<OrderShippingAddress>(orderDTO.Address);

            var Basket = await basketRepository.GetBasketAsync(orderDTO.BasketId);
            if (Basket == null)
                return Error.NotFound("Basket not found", $"Basket With Id : {orderDTO.BasketId} is Not Found");

            List<OrderItem> OrderItems = new List<OrderItem>();
            foreach (var item in Basket.Items)
            {
                var Product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (Product is null)
                    return Error.NotFound("Product not found", $"Product With Id : {item.Id} is Not Found");

                OrderItems.Add(CreateOrderItem(item, Product));
            }

            var DeliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDTO.DeliveryMethoId);
            if (DeliveryMethod is null)
                return Error.NotFound("Delivery method not found", $"Delivery method With Id : {orderDTO.DeliveryMethoId} is Not Found");

            var existingOrdersSpec = new OrderSpecifications(Email);
            var existingOrders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(existingOrdersSpec);

            var duplicate = existingOrders.FirstOrDefault(o =>
                o.DeliveryMethodId == orderDTO.DeliveryMethoId &&
                o.Items.Count == OrderItems.Count &&
                o.Items.All(existingItem =>
                    OrderItems.Any(newItem =>
                        newItem.Product.ProductId == existingItem.Product.ProductId &&
                        newItem.Quantity == existingItem.Quantity &&
                        newItem.Price == existingItem.Price)));

            if (duplicate is not null)
                return Result<OrderToReturnDTO>.Ok(mapper.Map<OrderToReturnDTO>(duplicate));

            var Subtotal = OrderItems.Sum(item => item.Price * item.Quantity);

            var Order = new Order
            {
                UserEmail = Email,
                Items = OrderItems,
                ShippingAddress = orderAddress,
                DeliveryMethod = DeliveryMethod,
                Subtotal = Subtotal
            };

            await unitOfWork.GetRepository<Order, Guid>().AddAsync(Order);

            var res = await unitOfWork.SaveChangesAsync();
            if (res == 0)
                return Error.InternalServerError("Order creation failed", "An error occurred while creating the order");

            return mapper.Map<OrderToReturnDTO>(Order);
        }

        private static OrderItem CreateOrderItem(Domain.Entities.BasketModule.BasketItem item, Product Product)
        {
            return new OrderItem
            {
                Product = new ProductItemOrdered
                {
                    ProductId = Product.Id,
                    ProductName = Product.Name,
                    PictureUrl = Product.PictureUrl
                },
                Price = Product.Price,
                Quantity = item.Quantity
            };
        }

        public async Task<Result<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();
            if (!deliveryMethods.Any())
                return Error.NotFound("No delivery methods found", "There are no delivery methods available");

            return Result<IEnumerable<DeliveryMethodDTO>>.Ok(mapper.Map<IEnumerable<DeliveryMethodDTO>>(deliveryMethods));
        }

        public async Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email)
        {
            var spec = new OrderSpecifications(email);
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);

            if (!orders.Any())
                return Error.NotFound("No orders found", $"No orders found for user with email: {email}");

            return Result<IEnumerable<OrderToReturnDTO>>.Ok(mapper.Map<IEnumerable<OrderToReturnDTO>>(orders));
        }

        public async Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersForAdminAsync()
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync();

            if (!orders.Any())
                return Error.NotFound("No orders found", "There are no orders in the system");

            return Result<IEnumerable<OrderToReturnDTO>>.Ok(mapper.Map<IEnumerable<OrderToReturnDTO>>(orders));
        }

        public async Task<Result<OrderToReturnDTO>> GetOrderById(Guid orderId)
        {
            var spec = new OrderSpecifications(orderId);
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(spec);

            if (order is null)
                return Error.NotFound("Order not found", $"Order with Id: {orderId} was not found");

            return Result<OrderToReturnDTO>.Ok(mapper.Map<OrderToReturnDTO>(order));
        }

        public async Task<bool> SaveInvoiceIdAsync(Guid orderId, string invoiceId)
        {
            var spec = new OrderSpecifications(orderId);
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(spec);
            if (order is null) return false;

            order.PaymentInvoiceId = invoiceId;
            unitOfWork.GetRepository<Order, Guid>().Update(order);

            var res = await unitOfWork.SaveChangesAsync();
            return res > 0;
        }

        public async Task<bool> MarkOrderAsPaidAsync(string invoiceId)
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync();
            var order = orders.FirstOrDefault(o => o.PaymentInvoiceId == invoiceId);

            if (order is null) return false;
            if (order.Status == OrderStatus.Paid) return true;

            order.Status = OrderStatus.Paid;
            unitOfWork.GetRepository<Order, Guid>().Update(order);

            var res = await unitOfWork.SaveChangesAsync();
            return res > 0;
        }
    }
}
