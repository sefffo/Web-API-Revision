using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.PaymentDTOs;
using ECommerce.SharedLibirary.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ECommerce.Presentation.Controllers;

//[ApiController]
//[Route("api/[controller]")]


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

    // 1️⃣ Frontend calls this after creating an order
    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDTO model)
    {
        // Get the existing order from DB
        var orderResult = await _orderService.GetOrderById(model.OrderId);
        if (!orderResult.isSuccess)
            return NotFound(orderResult.Errors);

        var order = orderResult.value;

        // Build CreateInvoiceDTO using your actual OrderToReturnDTO properties
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
                Name = i.ProductName,                 // ← ProductName from OrderItemDTO
                Price = i.Price.ToString("F2"),        // ← Price from OrderItemDTO
                Quantity = i.Quantity.ToString()          // ← Quantity from OrderItemDTO
            }).ToList()
        };

        // Call Fawaterak → returns (paymentUrl, invoiceId)
        var (paymentUrl, invoiceId) = await _fawaterakService.CreateInvoiceAsync(dto);

        // Return payment URL to frontend — they redirect the user there
        return Ok(new { paymentUrl, invoiceId });
    }

    // 2️⃣ User's browser lands here after payment (UI feedback only)
    [HttpGet("success")]
    public IActionResult PaymentSuccess([FromQuery] string invoiceId)
        => Ok(new { message = "Payment successful! Your order is confirmed.", invoiceId });

    [HttpGet("fail")]
    public IActionResult PaymentFail([FromQuery] string invoiceId)
        => BadRequest(new { message = "Payment failed. Please try again.", invoiceId });

    [HttpGet("pending")]
    public IActionResult PaymentPending([FromQuery] string invoiceId)
        => Ok(new { message = "Payment is being processed.", invoiceId });

    // 3️⃣ Fawaterak's SERVER posts here — this is where DB gets updated
    [HttpPost("callback")]
    public async Task<IActionResult> PaymentCallback([FromBody] FawaterakCallbackPayload payload)
    {
        // Security: verify it's really Fawaterak calling
        var providerKey = Request.Headers["provider-key"].ToString();
        if (providerKey != _settings.ProviderKey)
            return Unauthorized();

        if (payload.PaymentStatus == "paid")
            await _orderService.MarkOrderAsPaidAsync(payload.InvoiceId);

        // Always return 200 fast — Fawaterak retries if you're slow
        return Ok();
    }
}