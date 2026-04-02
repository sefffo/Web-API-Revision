using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.SharedLibirary.DTO_s.PaymentDTOs
{
    public class CreateInvoiceDTO
    {
        public string CartTotal { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
        public List<CreateInvoiceItemDTO> Items { get; set; } = [];
    }
    public class CreateInvoiceItemDTO
    {
        public string Name { get; set; } = default!;
        public string Price { get; set; } = default!;
        public string Quantity { get; set; } = default!;
    }
}
