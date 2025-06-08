using Applications.Contracts;
using Applications.Dtos.Payments;
using Core.Exceptions;
using AutoMapper;
using Core.Enums;
using Core.FactorySpecifications.Billing;
using Core.FactorySpecifications.ClientsFactorySpecifications;
using Core.FactorySpecifications.PayementSpecifications;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;

namespace Applications.Services
{
    public class PaymentService (
        IGenericRepository<Payment> paymentRepo,
        IGenericRepository<BillingInvoice> invoiceRepo,
        IGenericRepository<Client> clientRepo,
        IUnitOfWork uow,
        IMapper mapper
    ) : GenericService<Payment> ( paymentRepo ), IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepo = paymentRepo;
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo = invoiceRepo;
        private readonly IGenericRepository<Client> _clientRepo = clientRepo;
        private readonly IUnitOfWork _uow = uow;
        private readonly IMapper _mapper = mapper;

        public async Task<Payment> RegisterClientPaymentAsync ( CreatePaymentDto dto )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var client = await _clientRepo.GetEntityWithSpec(
                ClientSpecification.ClientSpecs.ByIdWithInvoices(dto.ClientId)
                ) ?? throw new NotFoundException("Cliente não encontrado.");

            // Localiza a fatura aberta mais recente
            var invoice = (await _invoiceRepo.GetAllAsync( BillingInvoiceSpecification.BillingInvoiceSpecs.
                OpenOrPartiallyPaidByClient(dto.ClientId)))
                .OrderByDescending(i => i.CreatedAt)
                .FirstOrDefault();


            if ( invoice == null )
                throw new UnprocessableEntityException ( "Nenhuma fatura aberta encontrada para este cliente." );

            // Cria o pagamento e associa à fatura aberta
            var payment = new Payment
            {
                ClientId = dto.ClientId,
                AmountPaid = dto.AmountPaid,
                PaymentDate = dto.PaymentDate.ToUniversalTime(),
                Description = dto.Description,
                BillingInvoiceId = invoice.BillingInvoiceId
            };

            await _paymentRepo.CreateAsync ( payment );

            // Recalcula o total pago incluindo este novo pagamento
            var payments = await _paymentRepo.GetAllAsync(
        PaymentSpecification.PaymentSpecs.ByInvoiceId(invoice.BillingInvoiceId));

            var totalPaid = payments.Sum(p => p.AmountPaid);

            invoice.Status = totalPaid switch
            {
                var total when total >= invoice.TotalInvoiceAmount => InvoiceStatus.Paid,
                var total when total > 0 => InvoiceStatus.PartiallyPaid,
                _ => InvoiceStatus.Open
            };

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return payment;
        }

    }
}
