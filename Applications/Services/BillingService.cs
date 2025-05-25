using Applications.Contracts;
using Applications.Dtos.Billing;
using Core.Enums;
using Core.FactorySpecifications.Billing;
using Core.FactorySpecifications.ServiceOrderFactorySpecifications;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.ServiceOrders;

namespace Applications.Services
{
    public class BillingService (
    IGenericRepository<BillingInvoice> invoiceRepo,
    IGenericRepository<ServiceOrder> orderRepo,
    IUnitOfWork uow
) : GenericService<BillingInvoice> ( invoiceRepo ), IBillingService
    {
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo = invoiceRepo;
        private readonly IGenericRepository<ServiceOrder> _orderRepo = orderRepo;
        private readonly IUnitOfWork _uow = uow;

        public async Task<BillingInvoice> GenerateInvoiceAsync ( CreateBillingInvoiceDto dto )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIds(dto.ServiceOrderIds);
            var orders = await _orderRepo.GetAllAsync(spec);

            if ( orders.Count != dto.ServiceOrderIds.Count )
                throw new Exception ( "Uma ou mais OS não foram encontradas." );

            if ( orders.Any ( o => o.ClientId != dto.ClientId ) )
                throw new Exception ( "As OS não pertencem ao cliente informado." );

            if ( orders.Any ( o => o.Status != OrderStatus.Finished ) )
                throw new Exception ( "Todas as OS devem estar finalizadas." );

            if ( orders.Any ( o => o.BillingInvoiceId != null ) )
                throw new Exception ( "Uma ou mais OS já estão vinculadas a uma fatura." );

            var total = orders.Sum(o => o.OrderTotal);

            var invoice = new BillingInvoice
            {
                ClientId = dto.ClientId,
                IssuedAt = dto.IssuedAt ?? DateTime.UtcNow,
                TotalAmount = total,
                Status = InvoiceStatus.Open,
                ServiceOrders = orders.ToList()
            };

            await _invoiceRepo.CreateAsync ( invoice );
            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return invoice; // ✅ retorna a entidade
        }

        public async Task<BillingInvoice> MarkAsPaidAsync ( int invoiceId )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var spec = BillingInvoiceSpecification.BillingInvoiceSpecs.ByIdFull(invoiceId);
            var invoice = await _invoiceRepo.GetEntityWithSpec(spec);

            if ( invoice == null )
                throw new Exception ( "Fatura não encontrada." );

            if ( invoice.Status == InvoiceStatus.Paid )
                throw new Exception ( "Fatura já está marcada como paga." );

            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt = DateTime.UtcNow;

            // Marca todas as OS como pagas também
            foreach ( var os in invoice.ServiceOrders )
            {
                os.Status = OrderStatus.Paid;
            }

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return invoice;
        }

        public async Task<BillingInvoice> CancelInvoiceAsync ( int invoiceId )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var spec = BillingInvoiceSpecification.BillingInvoiceSpecs.ByIdFull(invoiceId);
            var invoice = await _invoiceRepo.GetEntityWithSpec(spec);

            if ( invoice == null )
                throw new Exception ( "Fatura não encontrada." );

            if ( invoice.Status == InvoiceStatus.Cancelled )
                throw new Exception ( "Fatura já está cancelada." );

            invoice.Status = InvoiceStatus.Cancelled;
            invoice.PaidAt = null;

            // Desvincula as OS
            foreach ( var os in invoice.ServiceOrders )
            {
                os.BillingInvoiceId = null;
            }

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return invoice;
        }

        public async Task<IReadOnlyList<BillingInvoice>> GetAllByClientAsync ( int clientId )
        {
            var spec = BillingInvoiceSpecification.BillingInvoiceSpecs.AllByClient(clientId);
            return await _invoiceRepo.GetAllAsync ( spec );
        }


    }
}
