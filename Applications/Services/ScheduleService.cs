namespace Applications.Services
{
    public class ScheduleService (
        IGenericRepository<ServiceOrderSchedule> scheduleRepo,
        IGenericRepository<ServiceOrder> orderRepo,
        IGenericRepository<Sector> sectorRepo,
        IUnitOfWork uow,
        IMapper mapper
    ) : GenericService<ServiceOrderSchedule> ( scheduleRepo ), IScheduleService
    {
        private readonly IGenericRepository<ServiceOrderSchedule> _scheduleRepo = scheduleRepo;
        private readonly IGenericRepository<ServiceOrder> _orderRepo = orderRepo;
        private readonly IGenericRepository<Sector> _sectorRepo = sectorRepo;
        private readonly IUnitOfWork _uow = uow;
        private readonly IMapper _mapper = mapper;

        public async Task<ScheduleItemRecord> ScheduleDeliveryAsync ( ScheduleDeliveryDto dto )
        {
            var order = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecs.ByIdFull(dto.ServiceOrderId))
                ?? throw new NotFoundException($"Service order {dto.ServiceOrderId} not found.");

            if ( order.Status == OrderStatus.Finished )
                throw new BusinessRuleException ( "Cannot schedule a finished service order." );

            if ( order.Status == OrderStatus.TryIn )
                throw new BusinessRuleException ( "Cannot schedule a service order that is in try-in stage." );

            var existing = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ActiveByServiceOrderId(dto.ServiceOrderId));
            if ( existing != null )
                throw new ConflictException ( "An active schedule already exists for this service order." );

            // Validate sector (if provided)
            if ( dto.SectorId.HasValue )
            {
                var sector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.SectorId.Value));
                if ( sector == null )
                    throw new NotFoundException ( $"Sector with ID {dto.SectorId.Value} not found." );
            }

            // Specific rules for sector transfer
            if ( dto.DeliveryType == ScheduledDeliveryType.SectorTransfer )
            {
                if ( !dto.SectorId.HasValue )
                    throw new BusinessRuleException ( "For 'Sector Transfer' scheduling, the destination sector must be provided." );

                var currentSectorId = order.Stages?
                    .Where(s => s.DateOut == null)
                    .OrderByDescending(s => s.DateIn)
                    .FirstOrDefault()?.SectorId;

                if ( currentSectorId.HasValue && dto.SectorId.Value == currentSectorId.Value )
                    throw new BusinessRuleException ( "Destination sector must be different from the current sector." );
            }

            // Validate scheduled date
            var lastDateIn = order.Stages?.Max(s => s.DateIn);
            if ( lastDateIn.HasValue && dto.ScheduledDate < lastDateIn.Value )
                throw new BusinessRuleException (
                    $"Scheduled date ({dto.ScheduledDate:yyyy-MM-dd}) must be after the last stage entry date ({lastDateIn:yyyy-MM-dd})." );

            var entity = new ServiceOrderSchedule
            {
                ServiceOrderId = dto.ServiceOrderId,
                ScheduledDate = dto.ScheduledDate  ?? DateTime.Now,
                DeliveryType = dto.DeliveryType ?? ScheduledDeliveryType.FinalDelivery,
                SectorId = dto.SectorId,
                IsDelivered = false,
                IsOverdue = false,
                CreatedAt = DateTime.Now
            };

            var created = await base.CreateAsync(entity);
            return _mapper.Map<ScheduleItemRecord> ( created );
        }



        public async Task<ScheduleItemRecord?> UpdateScheduleAsync ( int scheduleId, ScheduleDeliveryDto dto )
        {
            var schedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ById(scheduleId))
        ?? throw new NotFoundException($"Schedule {scheduleId} not found.");

            if ( schedule.IsDelivered )
                throw new BusinessRuleException ( "Cannot update a schedule that has already been delivered." );

            var order = await _orderRepo.GetEntityWithSpec(ServiceOrderSpecs.ByIdFull(schedule.ServiceOrderId))
        ?? throw new NotFoundException($"Service order {schedule.ServiceOrderId} not found.");

            // Validate sector (if provided)
            if ( dto.SectorId.HasValue )
            {
                var sector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.SectorId.Value));
                if ( sector == null )
                    throw new NotFoundException ( $"Sector with ID {dto.SectorId.Value} not found." );
            }

            // Specific rules for sector transfer
            if ( dto.DeliveryType == ScheduledDeliveryType.SectorTransfer )
            {
                if ( !dto.SectorId.HasValue )
                    throw new BusinessRuleException ( "For 'Sector Transfer' scheduling, the destination sector must be provided." );

                var currentSectorId = order.Stages?
            .Where(s => s.DateOut == null)
            .OrderByDescending(s => s.DateIn)
            .FirstOrDefault()?.SectorId;

                if ( currentSectorId.HasValue && dto.SectorId.Value == currentSectorId.Value )
                    throw new BusinessRuleException ( "Destination sector must be different from the current sector." );
            }

            // Validate new date
            var newDate = dto.ScheduledDate ?? schedule.ScheduledDate;
            var lastDateIn = order.Stages?.Max(s => s.DateIn);

            if ( lastDateIn.HasValue && newDate < lastDateIn.Value )
                throw new BusinessRuleException (
                    $"New schedule date ({newDate:yyyy-MM-dd}) must be after the last stage entry date ({lastDateIn:yyyy-MM-dd})." );

            schedule.ScheduledDate = newDate;
            schedule.DeliveryType = dto.DeliveryType ?? schedule.DeliveryType;
            schedule.SectorId = dto.SectorId;

            var updated = await _scheduleRepo.UpdateAsync(scheduleId, schedule);

            // Retornar o record mapeado em vez da entidade
            return updated != null ? _mapper.Map<ScheduleItemRecord> ( updated ) : null;
        }




        public async Task<bool> RemoveScheduleAsync ( int scheduleId )
        {
            var schedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ById(scheduleId))
                ?? throw new NotFoundException($"Schedule {scheduleId} not found.");

            if ( schedule.IsDelivered )
                throw new BusinessRuleException ( "Cannot remove a schedule that has already been delivered." );

            await _scheduleRepo.DeleteAsync ( scheduleId );
            await _uow.SaveChangesAsync ( );
            return true;
        }

        public async Task<List<SectorScheduleRecord>> GetScheduleByDateAsync ( DateTime date )
        {
            var spec = ScheduleSpecs.ForDate(date);
            var schedules = await _scheduleRepo.GetAllAsync(spec);

            return schedules
                .GroupBy ( s => s.SectorId )
                .Select ( g => new SectorScheduleRecord
                {
                    SectorId = g.Key ?? 0,
                    SectorName = g.First ( ).Sector?.Name ?? "No Sector",
                    Deliveries = g.Select ( _mapper.Map<ScheduleItemRecord> ).ToList ( )
                } ).ToList ( );
        }

        public async Task<List<SectorScheduleRecord>> GetTodayScheduleAsync ( )
        {
            return await GetScheduleByDateAsync ( DateTime.Today );
        }

        public async Task UpdateOverdueStatusAsync ( )
        {
            var spec = ScheduleSpecs.OverdueDeliveries();
            var overdue = await _scheduleRepo.GetAllAsync(spec);

            foreach ( var s in overdue )
                s.IsOverdue = true;

            if ( overdue.Any ( ) )
                await _uow.SaveChangesAsync ( );
        }

        public async Task<List<SectorScheduleRecord>> GetScheduleByDateRangeAsync ( DateTime start, DateTime end )
        {
            var spec = ScheduleSpecs.ForDateRange(start, end);
            var schedules = await _scheduleRepo.GetAllAsync(spec);

            return schedules
                .GroupBy ( s => s.SectorId )
                .Select ( g => new SectorScheduleRecord
                {
                    SectorId = g.Key ?? 0,
                    SectorName = g.First ( ).Sector?.Name ?? "No Sector",
                    Deliveries = g.Select ( _mapper.Map<ScheduleItemRecord> ).ToList ( )
                } )
                .ToList ( );
        }

        public async Task MarkAsDeliveredAsync ( int serviceOrderId )
        {
            var schedule = await _scheduleRepo
        .GetEntityWithSpec(ScheduleSpecs.ActiveByServiceOrderId(serviceOrderId));

            if ( schedule is not null )
            {
                schedule.IsDelivered = true;
                await _scheduleRepo.UpdateAsync ( schedule.Id, schedule );
                await _uow.SaveChangesAsync ( );
            }
        }


        public async Task<ScheduleItemRecord?> GetActiveScheduleByServiceOrderIdAsync ( int serviceOrderId )
        {
            var schedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ActiveByServiceOrderId(serviceOrderId));
            return schedule != null ? _mapper.Map<ScheduleItemRecord> ( schedule ) : null;
        }

        public async Task<SectorScheduleRecord?> GetScheduleByCurrentSectorAsync ( int sectorId, DateTime date )
        {
            var spec = ScheduleSpecs.ByCurrentSectorAndDate(sectorId, date);
            var items = await _scheduleRepo.GetAllAsync(spec);

            if ( items.Count == 0 )
                return null;

            var record = new SectorScheduleRecord
            {
                SectorId = sectorId,
                SectorName = items.First().Sector?.Name ?? "Unnamed Sector",
                Deliveries = _mapper.Map<List<ScheduleItemRecord>>(items)
            };

            return record;
        }
    }
}
