using ECommerce.SharedLibirary.DTO_s.PaymentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Abstraction
{
    public interface IFawaterakService
    {
        Task<(string PaymentUrl, string InvoiceId)> CreateInvoiceAsync(CreateInvoiceDTO dto);
    }
}
