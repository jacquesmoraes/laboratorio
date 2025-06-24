using Core.Enums;

namespace Core.Params
{
    public class InvoiceParams : QueryParams
    {
        public int? ClientId { get; set; }
        public InvoiceStatus? Status { get; set; } 
    }
}
