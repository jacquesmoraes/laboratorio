namespace Applications.Services
{
    public class ClientAreaService : GenericService<Client>, IClientAreaService
    {
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo;
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IGenericRepository<Client> _clientRepo;

        public ClientAreaService (
            IGenericRepository<Client> clientRepo,
            IGenericRepository<BillingInvoice> invoiceRepo,
            IGenericRepository<Payment> paymentRepo
        ) : base ( clientRepo )
        {
            _clientRepo = clientRepo;
            _invoiceRepo = invoiceRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<ClientDashboardRecord> GetClientBasicDashboardAsync ( int clientId )
        {
            var client = await _clientRepo.GetEntityWithSpec(ClientSpecs.ById(clientId))
                ?? throw new NotFoundException("Client not found.");

            var totalInvoiced = await _invoiceRepo.SumAsync(
                i => i.ClientId == clientId,
                i => i.TotalServiceOrdersAmount);

            var totalPaid = await _paymentRepo.SumAsync(
                p => p.ClientId == clientId,
                p => p.AmountPaid);

            var balance = totalPaid - totalInvoiced;

            return new ClientDashboardRecord
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                Street = client.Address.Street,
                Number = client.Address.Number,
                Complement = client.Address.Complement,
                Neighborhood = client.Address.Neighborhood,
                City = client.Address.City,
                PhoneNumber = client.ClientPhoneNumber,
                TotalInvoiced = totalInvoiced,
                TotalPaid = totalPaid,
                Balance = balance
            };
        }
    }
}
