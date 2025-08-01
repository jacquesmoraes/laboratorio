﻿using static Core.FactorySpecifications.ClientAreaSpecifications.ClientAreaPaymentSpecification;

namespace Applications.Services
{
    public class PaymentService (
        IGenericRepository<Payment> paymentRepo,
        IGenericRepository<BillingInvoice> invoiceRepo,
        IGenericRepository<Client> clientRepo,
        IUnitOfWork uow
    ) : GenericService<Payment> ( paymentRepo ), IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepo = paymentRepo;
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo = invoiceRepo;
        private readonly IGenericRepository<Client> _clientRepo = clientRepo;
        private readonly IUnitOfWork _uow = uow;

        public async Task<Payment> RegisterClientPaymentAsync ( CreatePaymentDto dto )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            try
            {
                if ( dto.AmountPaid <= 0 )
                    throw new UnprocessableEntityException ( "Payment amount must be greater than zero." );

                var client = await _clientRepo.GetEntityWithSpec(ClientSpecs.ByIdWithInvoices(dto.ClientId)
                ) ?? throw new NotFoundException("Client not found.");

                var invoice = (await _invoiceRepo.GetAllAsync(
                    BillingInvoiceSpecification.BillingInvoiceSpecs.OpenOrPartiallyPaidByClient(dto.ClientId)))
                    .OrderByDescending(i => i.CreatedAt)
                    .FirstOrDefault();

                if ( invoice == null )
                    throw new UnprocessableEntityException ( "No open invoice found for this client." );
                if ( invoice.Status == InvoiceStatus.Closed )
                    throw new UnprocessableEntityException ( "This invoice is closed and cannot receive payments." );

                var payment = new Payment
                {
                    ClientId = dto.ClientId,
                    AmountPaid = dto.AmountPaid,
                    PaymentDate = dto.PaymentDate.ToUniversalTime(),
                    Description = dto.Description,
                    BillingInvoiceId = invoice.BillingInvoiceId
                };

                await _paymentRepo.CreateAsync ( payment );

                var payments = await _paymentRepo.GetAllAsync(PaymentSpecification.PaymentSpecs.ByInvoiceId(invoice.BillingInvoiceId));

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
            catch
            {
                await tx.RollbackAsync ( );
                throw;
            }
        }

        public async Task<Pagination<ClientPaymentRecord>> GetPaginatedAsync ( PaymentParams p )
        {
            // 1. Main spec with filters + pagination + sorting
            var spec = PaymentSpecification.PaymentSpecs.Paged(p);

            // 2. Count spec with criteria only (no pagination)
            Expression<Func<Payment, bool>> countCriteria = x =>
                (!p.ClientId.HasValue || x.ClientId == p.ClientId) &&
                (string.IsNullOrEmpty(p.Search) || x.Description!.ToLower().Contains(p.Search.ToLower())) &&
                (!p.StartDate.HasValue || x.PaymentDate >= p.StartDate.Value) &&
                (!p.EndDate.HasValue || x.PaymentDate <= p.EndDate.Value);

            var countSpec = new PaymentSpecification(countCriteria);

            // 3. Query
            var totalItems = await _paymentRepo.CountAsync(countSpec);
            var payments = await _paymentRepo.GetAllAsync(spec);

            // 4. Map to response record
            var records = payments.Select(p => new ClientPaymentRecord
            {
                Id = p.Id,
                PaymentDate = p.PaymentDate,
                AmountPaid = p.AmountPaid,
                Description = p.Description,
                ClientId = p.ClientId,
                ClientName = p.Client?.ClientName ?? string.Empty,
                BillingInvoiceId = p.BillingInvoiceId,
                InvoiceNumber = p.BillingInvoice?.InvoiceNumber
            }).ToList();

            return new Pagination<ClientPaymentRecord> ( p.PageNumber, p.PageSize, totalItems, records );
        }

        public async Task<Pagination<ClientPaymentRecord>> GetPaginatedForClientAreaAsync ( PaymentParams p )
        {
            // 1. Main spec otimizado para client area
            var spec = ClientAreaPaymentSpecs.Paged(p);

            // 2. Count spec otimizado (sem includes)
            var countSpec = ClientAreaPaymentSpecs.PagedForCount(p);

            // 3. Executar queries sequencialmente (não em paralelo)
            var totalItems = await _paymentRepo.CountWithoutTrackingAsync(countSpec);
            var payments = await _paymentRepo.GetAllWithoutTrackingAsync(spec);

            // 4. Mapeamento otimizado
            var records = payments.Select(p => new ClientPaymentRecord
            {
                Id = p.Id,
                PaymentDate = p.PaymentDate,
                AmountPaid = p.AmountPaid,
                Description = p.Description,
                ClientId = p.ClientId,
                BillingInvoiceId = p.BillingInvoiceId,
                InvoiceNumber = p.BillingInvoice?.InvoiceNumber
            }).ToList();

            return new Pagination<ClientPaymentRecord> ( p.PageNumber, p.PageSize, totalItems, records );
        }
    }
}
