using Applications.Dtos.Clients;
using Core.Enums;

namespace Applications.Dtos.Billing
{
    public class BillingInvoiceResponseDto
    {
        public int BillingInvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }

        public ClientInvoiceDto Client { get; set; } = new();

        public List<InvoiceServiceOrderDto> ServiceOrders { get; set; } = [];
        public InvoiceStatus Status { get; set; }

        public decimal TotalServiceOrdersAmount { get; set; }

        // Valores herdados da fatura anterior (de saldo global do cliente)
        public decimal PreviousCredit { get; set; }
        public decimal PreviousDebit { get; set; }

        // Soma total das OS - PreviousCredit + PreviousDebit
        public decimal TotalInvoiceAmount { get; set; }

        // Soma de todos os pagamentos vinculados a essa fatura
        public decimal TotalPaid { get; set; }

        // Calculado: quanto ainda falta pagar (ou crédito nesta fatura)
        public decimal OutstandingBalance => TotalInvoiceAmount - TotalPaid;
    }
}
