using Applications.Contracts;
using Applications.Dtos.Clients;
using Applications.Projections.Billing;
using Applications.Records.Billing;
using Applications.Records.Clients;
using Applications.Records.Payments;
using Core.Exceptions;
using Core.FactorySpecifications.Billing;
using Core.FactorySpecifications.ClientsFactorySpecifications;
using Core.FactorySpecifications.PayementSpecifications;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;

namespace Applications.Services
{
    public class ClientAreaService : GenericService<Client>, IClientAreaService
    {
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo;
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IGenericRepository<Client> _clientRepo;
        private readonly IUnitOfWork _uow;

        public ClientAreaService (
            IGenericRepository<Client> clientRepo,
            IGenericRepository<BillingInvoice> invoiceRepo,
            IGenericRepository<Payment> paymentRepo,
            IUnitOfWork uow ) : base ( clientRepo )
        {
            _clientRepo = clientRepo;
            _invoiceRepo = invoiceRepo;
            _paymentRepo = paymentRepo;
            _uow = uow;
        }

        public async Task<ClientAreaDataDto> GetClientDashboardAsync ( int clientId,
            DateTime? startDate = null, DateTime? endDate = null )
        {
            var client = await _clientRepo.GetEntityWithSpec(
                ClientSpecification.ClientSpecs.ById(clientId));

            if ( client is null )
                throw new NotFoundException ( "Cliente não encontrado." );

            var invoices = await _invoiceRepo.GetAllAsync(
                BillingInvoiceSpecification.BillingInvoiceSpecs.AllByClientFull(clientId));

            var allPayments = await _paymentRepo.GetAllAsync(
                PaymentSpecification.PaymentSpecs.ByClientId(clientId));
            var filteredPayments = allPayments
                .Where(p =>
                (!startDate.HasValue || p.PaymentDate >= startDate.Value) &&
                (!endDate.HasValue || p.PaymentDate <= endDate.Value)).ToList();

            var totalPaid = filteredPayments.Sum(p => p.AmountPaid);
            var totalInvoiced = invoices.Sum(i => i.TotalServiceOrdersAmount);

            var paymentRecords = filteredPayments.Select(p => new ClientPaymentRecord
            {
                Id = p.Id,
                PaymentDate = p.PaymentDate,
                AmountPaid = p.AmountPaid,
                Description = p.Description,
                ClientId = p.ClientId,
                ClientName = p.Client.ClientName,
                BillingInvoiceId = p.BillingInvoiceId,
                InvoiceNumber = p.BillingInvoice?.InvoiceNumber
            }).ToList();


            var invoiceProjections = invoices.Select(i => new BillingInvoiceResponseProjection
            {
                BillingInvoiceId = i.BillingInvoiceId,
                InvoiceNumber = i.InvoiceNumber,
                CreatedAt = i.CreatedAt,
                Description = i.Description,
                Status = i.Status,
                TotalServiceOrdersAmount = i.TotalServiceOrdersAmount,
                PreviousCredit = i.PreviousCredit,
                PreviousDebit = i.PreviousDebit,
                TotalInvoiceAmount = i.TotalInvoiceAmount,
                TotalPaid = i.Payments?.Sum(p => p.AmountPaid) ?? 0,
                PdfDownloadUrl = $"/client-area/invoices/{i.BillingInvoiceId}/download",
                Client = new ClientInvoiceRecord
                {
                    ClientName = i.Client.ClientName,
                    PhoneNumber = i.Client.ClientPhoneNumber,
                    Address = new ClientAddressRecord
                    {
                        Street = i.Client.Address?.Street ?? "",
                        Number = i.Client.Address?.Number ?? 0,
                        Complement = i.Client.Address?.Complement ?? "",
                        Neighborhood = i.Client.Address?.Neighborhood ?? "",
                        City = i.Client.Address?.City ?? ""
                    }
                },
                ServiceOrders = i.ServiceOrders?.Select(o => new InvoiceServiceOrderRecord
                {
                    DateIn = o.DateIn,
                    OrderCode = o.OrderNumber,
                    FinishedAt = o.DateOutFinal,
                    PatientName = o.PatientName,
                    Subtotal = o.OrderTotal,
                    Works = o.Works?.Select(w => new InvoiceWorkItemRecord
                    {
                        WorkTypeName = w.WorkType?.Name ?? "",
                        Quantity = w.Quantity,
                        PriceUnit = w.PriceUnit
                    }).ToList() ?? new()
                }).ToList() ?? new()
            }).ToList();

            return new ClientAreaDataDto
            {
                Client = client,
                TotalPaid = totalPaid,
                TotalInvoiced = totalInvoiced,
                Credit = totalPaid > totalInvoiced ? totalPaid - totalInvoiced : 0,
                Debit = totalInvoiced > totalPaid ? totalInvoiced - totalPaid : 0,
                Invoices = invoiceProjections,
                Payments = paymentRecords
            };
        }
    }
}
