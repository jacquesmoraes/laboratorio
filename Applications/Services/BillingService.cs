using Applications.Contracts;
using Applications.Dtos.Billing;
using Core.Enums;
using Core.FactorySpecifications.Billing;
using Core.FactorySpecifications.ClientsFactorySpecifications;
using Core.FactorySpecifications.PayementSpecifications;
using Core.FactorySpecifications.ServiceOrderFactorySpecifications;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Models.ServiceOrders;

namespace Applications.Services
{
    public class BillingService (
     IGenericRepository<BillingInvoice> invoiceRepo,
     IGenericRepository<Client> clientRepo,
     IGenericRepository<ServiceOrder> orderRepo,
     IGenericRepository<Payment> paymentRepo,
     IUnitOfWork uow
 ) : GenericService<BillingInvoice> ( invoiceRepo ), IBillingService
    {
        private readonly IGenericRepository<Client> _clientRepo = clientRepo;
        private readonly IGenericRepository<ServiceOrder> _orderRepo = orderRepo;
        private readonly IGenericRepository<Payment> _paymentRepo = paymentRepo;
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo = invoiceRepo;
        private readonly IUnitOfWork _uow = uow;

        public async Task<BillingInvoice> GenerateInvoiceAsync ( CreateBillingInvoiceDto dto )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var client = await _clientRepo.GetEntityWithSpec(ClientSpecification.ClientSpecs.ById(dto.ClientId))
        ?? throw new Exception("Cliente não encontrado.");

            // Valida ordens
            var orders = await _orderRepo.GetAllAsync(ServiceOrderSpecification.ServiceOrderSpecs.ByIds(dto.ServiceOrderIds));

            if ( orders.Any ( o => o.BillingInvoiceId != null ) )
                throw new Exception ( "Uma ou mais ordens já estão associadas a uma fatura." );

            if ( orders.Any ( o => o.Status != OrderStatus.Finished ) )
                throw new Exception ( "Só é possível faturar ordens já finalizadas." );

            var invoice = new BillingInvoice
            {
                InvoiceNumber = GenerateInvoiceNumber(client.ClientId),
                CreatedAt = DateTime.UtcNow,
                Description = dto.Description,
                ClientId = client.ClientId,
                Client = client,
                Status = InvoiceStatus.Open,
                ServiceOrders = orders.ToList(),
                TotalServiceOrdersAmount = orders.Sum(o => o.OrderTotal)
            };

            // Fecha fatura anterior se existir
            var faturasAntigas = (await _invoiceRepo.GetAllAsync(
        BillingInvoiceSpecification.BillingInvoiceSpecs.AllByClient(client.ClientId)))
        .Where(i => i.Status != InvoiceStatus.Cancelled)
        .OrderByDescending(i => i.CreatedAt)
        .ToList();

            var faturaAnterior = faturasAntigas.FirstOrDefault();

            if ( faturaAnterior != null )
            {
                faturaAnterior.Status = InvoiceStatus.Closed;

                // Calcula saldo anterior
                var pagamentos = await _paymentRepo.GetAllAsync(
            PaymentSpecification.PaymentSpecs.ByInvoiceId(faturaAnterior.BillingInvoiceId));

                var totalPagoAnterior = pagamentos.Sum(p => p.AmountPaid);
                var diff = totalPagoAnterior - faturaAnterior.TotalInvoiceAmount;

                if ( diff > 0 )
                    invoice.PreviousCredit = diff;
                else if ( diff < 0 )
                    invoice.PreviousDebit = Math.Abs ( diff );
            }

            // Atualiza relacionamento com OS
            foreach ( var order in invoice.ServiceOrders )
                order.BillingInvoice = invoice;



            await _invoiceRepo.CreateAsync ( invoice );
            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return invoice;
        }


        public async Task<BillingInvoice> CancelInvoiceAsync ( int id )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var spec = BillingInvoiceSpecification.BillingInvoiceSpecs.ByIdFull(id);
            var invoice = await GetEntityWithSpecAsync(spec);

            if ( invoice == null )
                throw new Exception ( "Fatura não encontrada." );

            if ( invoice.Status == InvoiceStatus.Cancelled )
                throw new Exception ( "Fatura já está cancelada." );

            if ( invoice.Status == InvoiceStatus.Paid )
                throw new Exception ( "Faturas pagas não podem ser canceladas." );

            foreach ( var order in invoice.ServiceOrders )
                order.BillingInvoiceId = null;

            invoice.Status = InvoiceStatus.Cancelled;

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return invoice;
        }

        private static string GenerateInvoiceNumber ( int clientId )
        {
            return $"FAT-{DateTime.UtcNow:yyyyMM}-C{clientId:D4}-{Guid.NewGuid ( ).ToString ( "N" ) [..6].ToUpper ( )}";
        }

    }
}
