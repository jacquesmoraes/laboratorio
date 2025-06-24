using Applications.Contracts;
using Applications.Dtos.Clients;
using Applications.Projections.Billing;
using Applications.Projections.ServiceOrder;
using Applications.Records.Clients;
using Applications.Records.Payments;
using Applications.Responses;
using AutoMapper;
using Core.Exceptions;
using Core.FactorySpecifications.BillingSpecifications;
using Core.FactorySpecifications.ClientsSpecifications;
using Core.FactorySpecifications.PaymentSpecifications;
using Core.FactorySpecifications.ServiceOrderSpecifications;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Models.ServiceOrders;
using Core.Params;

namespace Applications.Services;

public class ClientAreaService : GenericService<Client>, IClientAreaService
{
    private readonly IGenericRepository<BillingInvoice> _invoiceRepo;
    private readonly IGenericRepository<Payment> _paymentRepo;
    private readonly IGenericRepository<ServiceOrder> _orderRepo;
    private readonly IGenericRepository<Client> _clientRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ClientAreaService (
        IGenericRepository<Client> clientRepo,
        IGenericRepository<BillingInvoice> invoiceRepo,
        IGenericRepository<Payment> paymentRepo,
        IGenericRepository<ServiceOrder> orderRepo,
        IUnitOfWork uow,
        IMapper mapper
    ) : base ( clientRepo )
    {
        _clientRepo = clientRepo;
        _invoiceRepo = invoiceRepo;
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ClientDashboardRecord> GetClientBasicDashboardAsync ( int clientId )
    {
        var client = await _clientRepo.GetEntityWithSpec(
        ClientSpecification.ClientSpecs.ById(clientId))
        ?? throw new NotFoundException("Cliente não encontrado.");

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
