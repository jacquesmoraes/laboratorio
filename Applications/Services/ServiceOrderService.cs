using Applications.Contracts;
using Applications.Dtos.ServiceOrder;
using AutoMapper;
using Core.Enums;
using Core.FactorySpecifications.ClientsFactorySpecifications;
using Core.FactorySpecifications.ServiceOrderFactorySpecifications;
using Core.Interfaces;
using Core.Models.Clients;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using static Core.FactorySpecifications.SectorSpecifications.SectorSpecification;

namespace Applications.Services
{
    public class ServiceOrderService (
        IMapper mapper,
        IGenericRepository<ServiceOrder> orderRepo,
        IGenericRepository<Client> clientRepo,
        IGenericRepository<Sector> sectorRepo,
        IUnitOfWork uow )
        : GenericService<ServiceOrder> ( orderRepo ), IServiceOrderService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGenericRepository<ServiceOrder> _orderRepo = orderRepo;
        private readonly IGenericRepository<Client>       _clientRepo = clientRepo;
        private readonly IGenericRepository<Sector>       _sectorRepo = sectorRepo;
        private readonly IUnitOfWork _uow = uow;


        public async Task<ServiceOrder> CreateOrderAsync ( CreateServiceOrderDto dto )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var client = await _clientRepo.GetEntityWithSpec(ClientSpecification.ClientSpecs.ById(dto.ClientId))
        ?? throw new Exception("Cliente não encontrado.");

            var order = _mapper.Map<ServiceOrder>(dto);
            order.ClientId = client.ClientId;
            order.Client = client;
            order.OrderNumber = GenerateOrderNumber ( );
            order.Status = OrderStatus.Production;
            order.OrderTotal = order.Works.Sum ( w => w.PriceTotal );

            var firstSector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.FirstSectorId))
        ?? throw new Exception("Setor inicial não encontrado.");

            order.Stages.Add ( new ProductionStage
            {
                Sector = firstSector,
                SectorId = firstSector.SectorId,
                DateIn = DateTime.SpecifyKind ( dto.DateIn, DateTimeKind.Utc ),
                ServiceOrder = order
            } );

            await _orderRepo.CreateAsync ( order );
            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return order;
        }



        public async Task<ServiceOrder?> MoveToStageAsync ( MoveToStageDto dto )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var order  = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(dto.ServiceOrderId));
            var sector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.SectorId));
            if ( order == null || sector == null ) return null;
            var dateInUtc = DateTime.SpecifyKind(dto.DateIn, DateTimeKind.Utc);
            order.MoveTo ( sector, dateInUtc );
            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );
            return order;
        }

        public async Task<IReadOnlyList<ServiceOrder>> GetAllFilteredAsync ( ServiceOrderFilterDto filter )
        {
            var spec = ServiceOrderSpecification.ServiceOrderSpecs.Filtered(
                status: filter.Status,
                clientId: filter.ClientId,
                start: filter.Start,
                end: filter.End
                );
            return await _orderRepo.GetAllAsync ( spec );
        }


        public async Task<ServiceOrder?> SendToTryInAsync ( SendToTryInDto dto )
        {
            var order = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(dto.ServiceOrderId));
            if ( order == null ) return null;

            var dateOutUtc = DateTime.SpecifyKind(dto.DateOut, DateTimeKind.Utc);
            order.SendToTryIn ( dateOutUtc );
            await _uow.SaveChangesAsync ( );

            var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(order.ServiceOrderId);
            return await _orderRepo.GetEntityWithSpec ( spec );
        }


        public async Task<List<ServiceOrder>> FinishOrdersAsync ( FinishOrderDto dto )
        {
            using var tx = await _uow.BeginTransactionAsync();

            var serviceOrders = new List<ServiceOrder>();

            foreach ( var id in dto.ServiceOrderIds )
            {
                var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(id);
                var order = await _orderRepo.GetEntityWithSpec(spec);

                if ( order == null || order.Status == OrderStatus.Finished )
                    continue;

                serviceOrders.Add ( order );
            }

            if ( !serviceOrders.Any ( ) )
                throw new InvalidOperationException ( "Nenhuma ordem válida encontrada." );

            // Validação de cliente único
            var clientId = serviceOrders.First().ClientId;
            if ( serviceOrders.Any ( o => o.ClientId != clientId ) )
                throw new InvalidOperationException ( "Todas as ordens devem ser do mesmo cliente." );

            var dateOutUtc = DateTime.SpecifyKind(dto.DateOut, DateTimeKind.Utc);

            foreach ( var order in serviceOrders )
            {
                order.Finish ( dateOutUtc );
            }

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return serviceOrders;
        }




        public async Task<IReadOnlyList<ServiceOrder>> GetOutForTryInAsync ( int days )
        {
            var baseSpec = ServiceOrderSpecification.ServiceOrderSpecs.OutForTryInMoreThanXDays(days);
            var candidatos = await _orderRepo.GetAllAsync(baseSpec);
            return candidatos.Where ( o => o.OutForTryInMoreThan ( days ) ).ToList ( );
        }

        public async Task<ServiceOrder?> UpdateOrderAsync ( int id, CreateServiceOrderDto dto )
        {
            var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(id);
            var order = await _orderRepo.GetEntityWithSpec(spec);

            if ( order == null )
                return null;

            if ( order.Status == OrderStatus.Finished )
                throw new InvalidOperationException ( "Não é permitido editar uma OS finalizada." );

            // Troca de cliente
            if ( order.ClientId != dto.ClientId )
            {
                var newClient = await _clientRepo.GetEntityWithSpec(ClientSpecification.ClientSpecs.ById(dto.ClientId));
                if ( newClient == null )
                    throw new InvalidOperationException ( "Cliente informado não existe." );

                order.ClientId = newClient.ClientId;
                order.Client = newClient;
            }


            order.PatientName = dto.PatientName;
            order.DateIn = DateTime.SpecifyKind ( dto.DateIn, DateTimeKind.Utc );

            // Atualiza os trabalhos
            order.Works.Clear ( );
            order.Works = _mapper.Map<List<Work>> ( dto.Works );
            order.OrderTotal = order.Works.Sum ( w => w.PriceTotal );

            await _uow.SaveChangesAsync ( );
            return order;
        }


        public async Task<ServiceOrder?> DeleteOrderAsync ( int serviceOrderId )
        {
            var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(serviceOrderId);
            var order = await _orderRepo.GetEntityWithSpec(spec);

            if ( order == null )
                return null;

            // Validação adicional (opcional)
            if ( order.Status == OrderStatus.Finished )
                throw new InvalidOperationException ( "Não é permitido excluir ordens finalizadas." );

            await _orderRepo.DeleteAsync ( order.ServiceOrderId );
            await _uow.SaveChangesAsync ( );

            return order;
        }

        public async Task<ServiceOrder?> ReopenOrderAsync ( int id )
        {
            var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(id);
            var order = await _orderRepo.GetEntityWithSpec(spec);

            if ( order == null || order.Status != OrderStatus.Finished )
                return null;

            order.Status = OrderStatus.Production;
            order.DateOutFinal = null;

            var lastStage = order.Stages.LastOrDefault();
            if ( lastStage != null && lastStage.DateOut != null )
            {
                lastStage.DateOut = null;
            }

            await _uow.SaveChangesAsync ( );
            return order;
        }


        private static int GenerateOrderNumber ( ) => new Random ( ).Next ( 100000, 999999 );


    }
}