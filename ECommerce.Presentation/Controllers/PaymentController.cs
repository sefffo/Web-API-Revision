using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.PaymentDTOs;
using ECommerce.SharedLibirary.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ECommerce.Presentation.Controllers;

public class PaymentController : ApiBaseController
{
    private readonly IFawaterakService _fawaterakService;
    private readonly IOrderService _orderService;
    private readonly FawaterakSettings _settings;

    public PaymentController(
        IFawaterakService fawaterakService,
        IOrderService orderService,
        IOptions<FawaterakSettings> settings)
    {
        _fawaterakService = fawaterakService;
        _orderService = orderService;
        _settings = settings.Value;
    }

    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDTO model)
    {
        var orderResult = await _orderService.GetOrderById(model.OrderId);
        if (!orderResult.isSuccess)
            return NotFound(orderResult.Errors);

        var order = orderResult.value;

        var dto = new CreateInvoiceDTO
        {
            CartTotal = order.OrderItems.Sum(i => i.Price * i.Quantity).ToString("F2"),
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            Items = order.OrderItems.Select(i => new CreateInvoiceItemDTO
            {
                Name = i.ProductName,
                Price = i.Price.ToString("F2"),
                Quantity = i.Quantity.ToString()
            }).ToList()
        };

        // Call Fawaterak
        var (paymentUrl, invoiceId) = await _fawaterakService.CreateInvoiceAsync(dto);

        // Save invoiceId to the order in DB so callback can find it later
        await _orderService.SetOrderInvoiceIdAsync(model.OrderId, invoiceId);

        return Ok(new { paymentUrl, invoiceId });
    }

    [HttpGet("success")]
    public IActionResult PaymentSuccess([FromQuery] string invoiceId)
        => Ok(new { message = "Payment successful! Your order is confirmed.", invoiceId });

    [HttpGet("fail")]
    public IActionResult PaymentFail([FromQuery] string invoiceId)
        => BadRequest(new { message = "Payment failed. Please try again.", invoiceId });

    [HttpGet("pending")]
    public IActionResult PaymentPending([FromQuery] string invoiceId)
        => Ok(new { message = "Payment is being processed.", invoiceId });

    [HttpPost("callback")]
    public async Task<IActionResult> PaymentCallback([FromBody] FawaterakCallbackPayload payload)
    {
        var providerKey = Request.Headers["provider-key"].ToString();
        if (providerKey != _settings.ProviderKey)
            return Unauthorized();

        if (payload.PaymentStatus == "paid")
            await _orderService.MarkOrderAsPaidAsync(payload.InvoiceId);

        return Ok();
    }
}
