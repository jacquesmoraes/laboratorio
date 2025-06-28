using Applications.Contracts;
using Applications.Dtos.ServiceOrder;
using Applications.Projections.ServiceOrder;
using Applications.Responses;
using Applications.Services.Validators;
using AutoMapper;
using Core.Enums;
using Core.Exceptions;
using Core.FactorySpecifications.ServiceOrderSpecifications;
using Core.Interfaces;
using Core.Models.Clients;
using Core.Models.Schedule;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Core.Params;
using static Core.FactorySpecifications.ScheduleSpecification.ScheduleSpecs;
using static Core.FactorySpecifications.ClientsSpecifications.ClientSpecification;
using static Core.FactorySpecifications.SectorSpecifications.SectorSpecification;
using static Core.FactorySpecifications.ServiceOrderSpecifications.ServiceOrderSpecification;

namespace Applications.Services
{
    public class ServiceOrderService (
        IMapper mapper,
        IGenericRepository<ServiceOrder> orderRepo,
        IGenericRepository<Client> clientRepo,
        IGenericRepository<Sector> sectorRepo,
        IGenericRepository<ServiceOrderSchedule> scheduleRepo,
        IUnitOfWork uow )
        : GenericService<ServiceOrder> ( orderRepo ), IServiceOrderService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGenericRepository<ServiceOrder> _orderRepo = orderRepo;
        private readonly IGenericRepository<Client>       _clientRepo = clientRepo;
        private readonly IGenericRepository<Sector>       _sectorRepo = sectorRepo;
        private readonly IGenericRepository<ServiceOrderSchedule> _scheduleRepo = scheduleRepo;
        private readonly IUnitOfWork _uow = uow;


        public async Task<ServiceOrder> CreateOrderAsync ( CreateServiceOrderDto dto )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var client = await _clientRepo.GetEntityWithSpec(ClientSpecs.ById(dto.ClientId))
        ?? throw new NotFoundException("Cliente não encontrado.");

            var order = _mapper.Map<ServiceOrder>(dto);
            order.ClientId = client.ClientId;
            order.Client = client;
            order.OrderNumber = await GenerateOrderNumberAsync ( client.ClientId );
            order.Status = OrderStatus.Production;
            order.OrderTotal = order.Works.Sum ( w => w.PriceTotal );

            var firstSector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.FirstSectorId))
        ?? throw new NotFoundException("Setor inicial não encontrado.");

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



            var fullOrder = await _orderRepo.GetEntityWithSpec(
                ServiceOrderSpecs.ByIdFull(order.ServiceOrderId)
                );
            return fullOrder!;

        }



        public async Task<ServiceOrder?> MoveToStageAsync ( MoveToStageDto dto )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var order = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecs.ByIdFull(dto.ServiceOrderId))
        ?? throw new NotFoundException($"Ordem de serviço {dto.ServiceOrderId} não encontrada.");

            var sector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.SectorId))
        ?? throw new NotFoundException($"Setor {dto.SectorId} não encontrado.");

            //  Limpa agendamento ativo, se houver
            var existingSchedule = await _scheduleRepo.GetEntityWithSpec(ActiveByServiceOrderId(dto.ServiceOrderId));
            if ( existingSchedule is not null )
            {
                await _scheduleRepo.DeleteAsync ( existingSchedule );
                // Se quiser manter histórico em vez de deletar:
                // existingSchedule.IsDelivered = true;
            }

            //  Valida e aplica movimentação
            var dateInUtc = DateTime.SpecifyKind(dto.DateIn, DateTimeKind.Utc);
            ServiceOrderDateValidator.ValidateNewStageDate ( order.Stages, dateInUtc );
            order.MoveTo ( sector, dateInUtc );

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );
            return order;
        }





        public async Task<ServiceOrder?> SendToTryInAsync ( SendToTryInDto dto )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var order = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecs.ByIdFull(dto.ServiceOrderId))
        ?? throw new NotFoundException($"Ordem de serviço {dto.ServiceOrderId} não encontrada.");

            var dateOutUtc = DateTime.SpecifyKind(dto.DateOut, DateTimeKind.Utc);
            ServiceOrderDateValidator.ValidateTryInDate ( order.Stages, dateOutUtc );
            order.SendToTryIn ( dateOutUtc );

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return order;
        }


        public async Task<List<ServiceOrder>> FinishOrdersAsync ( FinishOrderDto dto )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var serviceOrders = new List<ServiceOrder>();

            foreach ( var id in dto.ServiceOrderIds )
            {
                var spec = ServiceOrderSpecs.ByIdFull(id);
                var order = await _orderRepo.GetEntityWithSpec(spec);

                if ( order == null || order.Status == OrderStatus.Finished )
                    continue;

                serviceOrders.Add ( order );
            }

            if ( !serviceOrders.Any ( ) )
                throw new BusinessRuleException ( "Nenhuma ordem válida encontrada." );

            // Validação de cliente único
            var clientId = serviceOrders.First().ClientId;
            if ( serviceOrders.Any ( o => o.ClientId != clientId ) )
                throw new BusinessRuleException ( "Todas as ordens devem ser do mesmo cliente." );

            var dateOutUtc = DateTime.SpecifyKind(dto.DateOut, DateTimeKind.Utc);

            foreach ( var order in serviceOrders )
            {
                ServiceOrderDateValidator.ValidateFinishDate ( serviceOrders.First ( ), dateOutUtc );
            }
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
            var baseSpec = ServiceOrderSpecs.OutForTryInMoreThanXDays(days);
            var candidatos = await _orderRepo.GetAllAsync(baseSpec);
            return candidatos.Where ( o => o.OutForTryInMoreThan ( days ) ).ToList ( );
        }

        public async Task<ServiceOrder?> UpdateOrderAsync ( int id, CreateServiceOrderDto dto )
        {
            if ( id <= 0 )
                throw new CustomValidationException ( "ID da ordem de serviço inválido." );
            var spec = ServiceOrderSpecs.ByIdFull(id);
            var order = await _orderRepo.GetEntityWithSpec(spec)
        ?? throw new NotFoundException($"Ordem de serviço {id} não encontrada.");


            if ( order.Status == OrderStatus.Finished )
                throw new BusinessRuleException ( "Não é permitido editar uma OS finalizada." );

            // Troca de cliente
            if ( order.ClientId != dto.ClientId )
            {
                var newClient = await _clientRepo.GetEntityWithSpec(ClientSpecs.ById(dto.ClientId));
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
            var spec = ServiceOrderSpecs.ByIdFull(serviceOrderId);
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

        public async Task<Pagination<ServiceOrderListProjection>> GetPaginatedAsync ( ServiceOrderParams p )
        {
            var spec = ServiceOrderSpecification.ServiceOrderSpecs.Paged(p);

            var countSpec = new ServiceOrderSpecification(o =>
        (!p.ClientId.HasValue || o.ClientId == p.ClientId) &&
        (!p.Status.HasValue || o.Status == p.Status) &&
        (string.IsNullOrEmpty(p.PatientName) || o.PatientName.ToLower().Contains(p.PatientName.ToLower())) &&
        (!p.StartDate.HasValue || o.DateIn >= p.StartDate.Value) &&
        (!p.EndDate.HasValue || o.DateIn <= p.EndDate.Value)
    );

            var totalItems = await _orderRepo.CountAsync(countSpec);
            var orders = await _orderRepo.GetAllAsync(spec);

            var projections = _mapper.Map<IReadOnlyList<ServiceOrderListProjection>>(orders);

            return new Pagination<ServiceOrderListProjection> ( p.PageNumber, p.PageSize, totalItems, projections );
        }


        public async Task<ServiceOrder?> ReopenOrderAsync ( int id )
        {
            var spec = ServiceOrderSpecs.ByIdFull(id);
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


        private async Task<string> GenerateOrderNumberAsync ( int clientId )
        {
            var spec = ServiceOrderSpecs.AllByClient(clientId);

            var allOrders = await _orderRepo.GetAllAsync(spec);
            var maxSequence = allOrders
        .Select(o =>
        {
            var parts = o.OrderNumber?.Split('-');
            return parts != null && int.TryParse(parts[0], out var n) ? n : 0;
        })
        .DefaultIfEmpty(0)
        .Max();

            var nextSequence = maxSequence + 1;

            return $"{nextSequence}-{clientId}";
        }




    }
}