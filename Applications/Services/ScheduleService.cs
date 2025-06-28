using Applications.Contracts;
using Applications.Dtos.Schedule;
using Applications.Records.Schedule;
using AutoMapper;
using Core.Enums;
using Core.Exceptions;
using Core.FactorySpecifications.SectorSpecifications;
using Core.Interfaces;
using Core.Models.Schedule;
using Core.Models.ServiceOrders;
using static Core.FactorySpecifications.ScheduleSpecification;
using static Core.FactorySpecifications.ScheduleSpecification.ScheduleSpecs;
using static Core.FactorySpecifications.SectorSpecifications.SectorSpecification;
using static Core.FactorySpecifications.ServiceOrderSpecifications.ServiceOrderSpecification.ServiceOrderSpecs;

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
            var order = await _orderRepo.GetEntityWithSpec(ByIdFull(dto.ServiceOrderId))
                ?? throw new NotFoundException($"OS {dto.ServiceOrderId} não encontrada.");

            if ( order.Status == OrderStatus.Finished )
                throw new BusinessRuleException ( "Não é possível agendar uma OS finalizada." );

            var existing = await _scheduleRepo.GetEntityWithSpec(ActiveByServiceOrderId(dto.ServiceOrderId));
            if ( existing != null )
                throw new ConflictException ( "Já existe um agendamento ativo para esta OS." );

            if ( dto.SectorId.HasValue )
            {
                var sector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.SectorId.Value))

                    ?? throw new NotFoundException($"Setor {dto.SectorId.Value} não encontrado.");
            }

            var entity = new ServiceOrderSchedule
            {
                ServiceOrderId = dto.ServiceOrderId,
                ScheduledDate = dto.ScheduledDate?.ToLocalTime() ?? DateTime.Today, // Converter para local time
                DeliveryType = dto.DeliveryType ?? ScheduledDeliveryType.FinalDelivery,
                SectorId = dto.SectorId,
                IsDelivered = false,
                IsOverdue = false,
                CreatedAt = DateTime.Now // Converter para local time
            };

            var created = await base.CreateAsync(entity);
            return _mapper.Map<ScheduleItemRecord> ( created );

        }

        public async Task<ServiceOrderSchedule?> UpdateScheduleAsync ( int scheduleId, ScheduleDeliveryDto dto )
        {
            var schedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ById(scheduleId))
                ?? throw new NotFoundException($"Agendamento {scheduleId} não encontrado.");

            if ( schedule.IsDelivered )
                throw new BusinessRuleException ( "Não é possível alterar um agendamento já entregue." );

            if ( dto.SectorId.HasValue )
            {
                var sector = await _sectorRepo.GetEntityWithSpec(SectorSpecs.ById(dto.SectorId.Value))
                    ?? throw new NotFoundException($"Setor {dto.SectorId.Value} não encontrado.");
            }

            schedule.ScheduledDate = dto.ScheduledDate ?? schedule.ScheduledDate;
            schedule.DeliveryType = dto.DeliveryType ?? schedule.DeliveryType;
            schedule.SectorId = dto.SectorId;

            return await _scheduleRepo.UpdateAsync ( scheduleId, schedule );
        }

        public async Task<bool> RemoveScheduleAsync ( int scheduleId )
        {
            var schedule = await _scheduleRepo.GetEntityWithSpec(ById(scheduleId))

                ?? throw new NotFoundException($"Agendamento {scheduleId} não encontrado.");

            if ( schedule.IsDelivered )
                throw new BusinessRuleException ( "Não é possível remover um agendamento já entregue." );

            await _scheduleRepo.DeleteAsync ( scheduleId );
            await _uow.SaveChangesAsync ( );
            return true;
        }

        public async Task<List<SectorScheduleRecord>> GetScheduleByDateAsync ( DateTime date )
        {
            var spec = ForDate(date);
            var schedules = await _scheduleRepo.GetAllAsync(spec);

            return schedules
                .GroupBy ( s => s.SectorId )
                .Select ( g => new SectorScheduleRecord
                {
                    SectorId = g.Key ?? 0,
                    SectorName = g.First ( ).Sector?.Name ?? "Sem Setor",
                    Deliveries = g.Select ( _mapper.Map<ScheduleItemRecord> ).ToList ( )
                } ).ToList ( );
        }

        public async Task<List<SectorScheduleRecord>> GetTodayScheduleAsync ( )
        {
            return await GetScheduleByDateAsync ( DateTime.Today );
        }



        //public async Task<bool> MarkAsDeliveredAsync ( int scheduleId )
        //{
        //    var schedule = await _scheduleRepo.GetEntityWithSpec(ScheduleSpecs.ById(scheduleId))
        //        ?? throw new NotFoundException($"Agendamento {scheduleId} não encontrado.");

        //    schedule.IsDelivered = true;
        //    await _uow.SaveChangesAsync ( );
        //    return true;
        //}

        public async Task UpdateOverdueStatusAsync ( )
        {
            var spec = OverdueDeliveries();
            var overdue = await _scheduleRepo.GetAllAsync(spec);

            foreach ( var s in overdue )
                s.IsOverdue = true;

            if ( overdue.Any ( ) )
                await _uow.SaveChangesAsync ( );
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
                SectorName = items.First().Sector?.Name ?? "Sem Nome",
                Deliveries = _mapper.Map<List<ScheduleItemRecord>>(items)
            };

            return record;
        }




        //public async Task<List<ServiceOrderSchedule>> GetAvailableForSchedulingAsync ( )
        //{
        //    var spec = AvailableForScheduling();
        //    var orders = await _orderRepo.GetAllAsync(spec);

        //    return orders.Select ( o => new ServiceOrderSchedule
        //    {
        //        ServiceOrderId = o.ServiceOrderId,
        //        ServiceOrder = o,
        //        IsDelivered = false,
        //        IsOverdue = false
        //    } ).ToList ( );
        //}
    }
}
