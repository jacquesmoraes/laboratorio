using Applications.Projections.ClientArea;
using AutoMapper.QueryableExtensions;
using static Core.FactorySpecifications.ClientAreaSpecifications.ClientAreaServiceOrderSpecification;

namespace Applications.Services
{
    public class ServiceOrderService (
        IMapper mapper,
        IGenericRepository<ServiceOrder> orderRepo,
        IGenericRepository<Client> clientRepo,
        IGenericRepository<Sector> sectorRepo,
        IGenericRepository<ServiceOrderSchedule> scheduleRepo,
        IUnitOfWork uow,
        IPerformanceLoggingService perfLogger )
        : GenericService<ServiceOrder> ( orderRepo ), IServiceOrderService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGenericRepository<ServiceOrder> _orderRepo = orderRepo;
        private readonly IGenericRepository<Client>       _clientRepo = clientRepo;
        private readonly IGenericRepository<Sector>       _sectorRepo = sectorRepo;
        private readonly IGenericRepository<ServiceOrderSchedule> _scheduleRepo = scheduleRepo;
        private readonly IUnitOfWork _uow = uow;
        private readonly IPerformanceLoggingService _perfLogger = perfLogger;


        public async Task<ServiceOrder> CreateOrderAsync ( CreateServiceOrderDto dto )
        {
            using ( _perfLogger.MeasureOperation ( "CreateServiceOrder", new Dictionary<string, object>
            {
                ["ClientId"] = dto.ClientId
            } ) )
            {
                await using var tx = await _uow.BeginTransactionAsync();

                var client = await _clientRepo.GetEntityWithSpec(ClientSpecs.ById(dto.ClientId))
            ?? throw new NotFoundException("Client not found.");

                var order = _mapper.Map<ServiceOrder>(dto);
                order.ClientId = client.ClientId;
                order.Client = client;
                order.OrderNumber = await GenerateOrderNumberAsync ( client.ClientId );
                order.Status = OrderStatus.Production;
                order.OrderTotal = order.Works.Sum ( w => w.PriceTotal );

                if ( order.IsRepeat && order.RepeatResponsible == RepeatResponsible.Laboratory )
                {
                    order.OrderTotal = 0;
                }

                var firstSector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.FirstSectorId))
            ?? throw new NotFoundException("Initial sector not found.");

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

                var fullOrder = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecs.ByIdFull(order.ServiceOrderId));
                return fullOrder!;
            }
        }

        public async Task<ServiceOrder?> MoveToStageAsync ( MoveToStageDto dto )
        {
            await using var tx = await _uow.BeginTransactionAsync();

            var order = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecs.ByIdFull(dto.ServiceOrderId))
        ?? throw new NotFoundException($"Service Order {dto.ServiceOrderId} not found.");

            var sector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.SectorId))
        ?? throw new NotFoundException($"Sector {dto.SectorId} not found.");

            // Clear active schedule, if any
            var existingSchedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ActiveByServiceOrderId(dto.ServiceOrderId));
            if ( existingSchedule is not null )
            {
                await _scheduleRepo.DeleteAsync ( existingSchedule );
                // If you prefer to keep history instead of deleting:
                // existingSchedule.IsDelivered = true;
            }

            // Validate and apply stage move
            var localTime = DateTime.SpecifyKind(dto.DateIn, DateTimeKind.Local);
            var dateInUtc = localTime.ToUniversalTime();

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
        ?? throw new NotFoundException($"Service Order {dto.ServiceOrderId} not found.");

            var dateOutUtc = dto.DateOut.Kind switch
            {
                DateTimeKind.Utc => dto.DateOut,
                DateTimeKind.Local => dto.DateOut.ToUniversalTime(),
                _ => throw new UnprocessableEntityException("DateOut must have a defined DateTimeKind.")
            };


            var existingSchedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ActiveByServiceOrderId(dto.ServiceOrderId));
            if ( existingSchedule is not null )
            {
                await _scheduleRepo.DeleteAsync ( existingSchedule );
            }

            var localTime = DateTime.SpecifyKind(dto.DateOut, DateTimeKind.Local);

            ServiceOrderDateValidator.ValidateTryInDate ( order.Stages, dateOutUtc );
            order.SendToTryIn ( dateOutUtc );

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return order;
        }

        public async Task<Pagination<ServiceOrderListDto>> GetPaginatedLightAsync ( ServiceOrderParams p )
        {
            var spec = ServiceOrderSpecs.PagedLightForLists(p);
            var countSpec = ServiceOrderSpecs.PagedForCount(p);

            var totalItems = await _orderRepo.CountAsync(countSpec);
            var items = await _orderRepo.GetAllAsync(spec);

            var data = _mapper.Map<IReadOnlyList<ServiceOrderListDto>>(items);

            return new Pagination<ServiceOrderListDto> ( p.PageNumber, p.PageSize, totalItems, data );
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
                throw new BusinessRuleException ( "No valid service order found." );

            // Validate that all orders belong to the same client
            var clientId = serviceOrders.First().ClientId;
            if ( serviceOrders.Any ( o => o.ClientId != clientId ) )
                throw new BusinessRuleException ( "All service orders must belong to the same client." );

            var localTime = DateTime.SpecifyKind(dto.DateOut, DateTimeKind.Local);
            var dateOutUtc = localTime.ToUniversalTime();

            foreach ( var order in serviceOrders )
            {
                // validate finish date
                ServiceOrderDateValidator.ValidateFinishDate ( order, dateOutUtc );

                // finalize order
                order.Finish ( dateOutUtc );

                // Remove schedule, if any
                var existingSchedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ActiveByServiceOrderId(order.ServiceOrderId));
                if ( existingSchedule is not null )
                {
                    await _scheduleRepo.DeleteAsync ( existingSchedule );
                }
            }

            await _uow.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            return serviceOrders;
        }

        public async Task<IReadOnlyList<ServiceOrder>> GetOutForTryInAsync ( int days )
        {
            var baseSpec = ServiceOrderSpecs.OutForTryInMoreThanXDays(days);
            var candidates = await _orderRepo.GetAllAsync(baseSpec);
            return candidates.Where ( o => o.OutForTryInMoreThan ( days ) ).ToList ( );
        }

        public async Task<ServiceOrder?> UpdateOrderAsync ( int id, CreateServiceOrderDto dto )
        {
            if ( id <= 0 )
                throw new CustomValidationException ( "Invalid service order ID." );

            var spec = ServiceOrderSpecs.ByIdFull(id);
            var order = await _orderRepo.GetEntityWithSpec(spec)
        ?? throw new NotFoundException($"Service Order {id} not found.");

            if ( order.Status == OrderStatus.Finished )
                throw new BusinessRuleException ( "Editing a finished service order is not allowed." );

            // Change client if needed
            if ( order.ClientId != dto.ClientId )
            {
                var newClient = await _clientRepo.GetEntityWithSpec(ClientSpecs.ById(dto.ClientId));
                if ( newClient == null )
                    throw new InvalidOperationException ( "Specified client does not exist." );

                order.ClientId = newClient.ClientId;
                order.Client = newClient;
            }

            order.PatientName = dto.PatientName;
            order.DateIn = DateTime.SpecifyKind ( dto.DateIn, DateTimeKind.Utc );
            order.IsRepeat = dto.IsRepeat;
            order.RepeatResponsible = dto.RepeatResponsible;

            // Update works
            order.Works.Clear ( );
            order.Works = _mapper.Map<List<Work>> ( dto.Works );
            order.OrderTotal = order.Works.Sum ( w => w.PriceTotal );
            if ( order.IsRepeat && order.RepeatResponsible == RepeatResponsible.Laboratory )
            {
                order.OrderTotal = 0;
            }

            await _uow.SaveChangesAsync ( );
            return order;
        }

        public async Task<ServiceOrder?> DeleteOrderAsync ( int serviceOrderId )
        {
            var spec = ServiceOrderSpecs.ByIdFull(serviceOrderId);
            var order = await _orderRepo.GetEntityWithSpec(spec);

            if ( order == null )
                return null;

            // Optional: additional validation
            if ( order.Status == OrderStatus.Finished )
                throw new InvalidOperationException ( "Deleting finished service orders is not allowed." );

            await _orderRepo.DeleteAsync ( order.ServiceOrderId );
            await _uow.SaveChangesAsync ( );

            return order;
        }

        public async Task<Pagination<ServiceOrderListProjection>> GetPaginatedAsync ( ServiceOrderParams p )
        {
            var spec = ServiceOrderSpecs.Paged(p);

            var countSpec = new ServiceOrderSpecification(o =>
                (!p.ClientId.HasValue || o.ClientId == p.ClientId) &&
                (!p.Status.HasValue || o.Status == p.Status) &&
                (!p.ExcludeFinished || o.Status != OrderStatus.Finished) &&
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

        public async Task<Pagination<ClientAreaServiceOrderProjection>> GetPaginatedForClientAreaAsync ( ServiceOrderParams p )
        {
            // 1. Main spec otimizada para client area
            var spec = ClientAreaServiceOrderSpecs.Paged(p);

            // 2. Count spec otimizada
            var countSpec = ClientAreaServiceOrderSpecs.PagedForCount(p);

            // 3. Executar queries sequencialmente
            var totalItems = await _orderRepo.CountWithoutTrackingAsync(countSpec);
            var serviceOrders = await _orderRepo.GetAllWithoutTrackingAsync(spec);

            // 4. Mapear para projection da área do cliente
            var projections = _mapper.Map<IReadOnlyList<ClientAreaServiceOrderProjection>>(serviceOrders);

            return new Pagination<ClientAreaServiceOrderProjection> (
                p.PageNumber,
                p.PageSize,
                totalItems,
                projections
            );
        }


        public async Task<ClientAreaServiceOrderDetailsProjection?> GetDetailsForClientAreaAsync ( int serviceOrderId, int clientId )
        {
            var spec = ClientAreaServiceOrderSpecs.ByIdForDetails(serviceOrderId, clientId);

            var serviceOrders = await _orderRepo.GetAllWithoutTrackingAsync(spec);
            var projection = serviceOrders
        .AsQueryable()
        .ProjectTo<ClientAreaServiceOrderDetailsProjection>(_mapper.ConfigurationProvider)
        .FirstOrDefault();

            return projection;
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
