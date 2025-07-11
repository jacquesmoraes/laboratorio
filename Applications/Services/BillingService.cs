namespace Applications.Services
{
    public class BillingService (
        IGenericRepository<BillingInvoice> invoiceRepo,
        IGenericRepository<Client> clientRepo,
        IGenericRepository<ServiceOrder> orderRepo,
        IGenericRepository<Payment> paymentRepo,
        IUnitOfWork uow,
        IMapper mapper
    ) : GenericService<BillingInvoice> ( invoiceRepo ), IBillingService
    {
        private readonly IGenericRepository<Client> _clientRepo = clientRepo;
        private readonly IGenericRepository<ServiceOrder> _orderRepo = orderRepo;
        private readonly IGenericRepository<Payment> _paymentRepo = paymentRepo;
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo = invoiceRepo;
        private readonly IUnitOfWork _uow = uow;
        private readonly IMapper _mapper = mapper;

        public async Task<BillingInvoice> GenerateInvoiceAsync ( CreateBillingInvoiceDto dto )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var client = await _clientRepo.GetEntityWithSpec(ClientSpecs.ById(dto.ClientId))
                ?? throw new NotFoundException("Client not found.");

            // Validate orders
            var orders = await _orderRepo.GetAllAsync(ServiceOrderSpecs.ByIds(dto.ServiceOrderIds));

            if ( orders.Any ( o => o.BillingInvoiceId != null ) )
                throw new ConflictException ( "One or more service orders are already linked to an invoice." );

            if ( orders.Any ( o => o.Status != OrderStatus.Finished ) )
                throw new UnprocessableEntityException ( "Only finished service orders can be invoiced." );

            var invoice = new BillingInvoice
            {
                InvoiceNumber = GenerateInvoiceNumber(client.ClientId),
                CreatedAt = DateTime.UtcNow,
                Description = dto.Description,
                ClientId = client.ClientId,
                Client = client,
                Status = InvoiceStatus.Open,
                ServiceOrders = [..orders],
                TotalServiceOrdersAmount = orders.Sum(o => o.OrderTotal)
            };

            // Close previous invoice if it exists
            var previousInvoices = (await _invoiceRepo.GetAllAsync(
                BillingInvoiceSpecs.AllByClient(client.ClientId)))
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            var lastInvoice = previousInvoices.FirstOrDefault();

            if ( lastInvoice != null )
            {
                lastInvoice.Status = InvoiceStatus.Closed;

                // Calculate balance from previous invoice
                var payments = await _paymentRepo.GetAllAsync(
                    PaymentSpecs.ByInvoiceId(lastInvoice.BillingInvoiceId));

                var totalPaid = payments.Sum(p => p.AmountPaid);
                var balanceDifference = totalPaid - lastInvoice.TotalInvoiceAmount;

                if ( balanceDifference > 0 )
                    invoice.PreviousCredit = balanceDifference;
                else if ( balanceDifference < 0 )
                    invoice.PreviousDebit = Math.Abs ( balanceDifference );
            }

            // Link orders to invoice
            foreach ( var order in invoice.ServiceOrders )
                order.BillingInvoice = invoice;

            await _invoiceRepo.CreateAsync ( invoice );
            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return invoice;
        }

        public async Task<Pagination<BillingInvoiceResponseProjection>> GetPaginatedInvoicesAsync ( InvoiceParams p )
        {
            var spec = BillingInvoiceSpecs.Paged(p);
            var countSpec = BillingInvoiceSpecs.PagedForCount(p);

            var totalItems = await _invoiceRepo.CountAsync(countSpec);
            var invoices = await _invoiceRepo.GetAllAsync(spec);
            var projections = _mapper.Map<IReadOnlyList<BillingInvoiceResponseProjection>>(invoices);

            foreach ( var invoice in projections )
            {
                invoice.PdfDownloadUrl = $"/api/client-area/invoices/{invoice.BillingInvoiceId}/download";
            }

            return new Pagination<BillingInvoiceResponseProjection> ( p.PageNumber, p.PageSize, totalItems, projections );
        }

        public async Task<BillingInvoice> CancelInvoiceAsync ( int id )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var spec = BillingInvoiceSpecs.ByIdFull(id);
            var invoice = await GetEntityWithSpecAsync(spec);

            if ( invoice == null )
                throw new NotFoundException ( "Invoice not found." );

            if ( invoice.Status == InvoiceStatus.Cancelled )
                throw new ConflictException ( "Invoice is already cancelled." );

            if ( invoice.Status == InvoiceStatus.Paid )
                throw new UnprocessableEntityException ( "Paid invoices cannot be cancelled." );

            foreach ( var order in invoice.ServiceOrders )
            { 
                order.BillingInvoiceId = null;
                order.BillingInvoice = null;
            }
            invoice.Status = InvoiceStatus.Cancelled;

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return invoice;
        }

        public async Task<BillingInvoiceRecord?> GetInvoiceRecordByIdAsync ( int id )
        {
            var spec = BillingInvoiceSpecs.ByIdFull(id);
            var invoice = await _invoiceRepo.GetEntityWithSpec(spec);

            return invoice == null
                ? null
                : _mapper.Map<BillingInvoiceRecord> ( invoice );
        }

        private static string GenerateInvoiceNumber ( int clientId )
        {
            return $"INV-{DateTime.UtcNow:yyyyMM}-C{clientId:D4}-{Guid.NewGuid ( ).ToString ( "N" ) [..6].ToUpper ( )}";
        }
    }
}
