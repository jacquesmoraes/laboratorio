using Applications.Dtos.Schedule;
using Applications.Records.Schedule;
using Core.Models.Schedule;

namespace Applications.Contracts
{
    public interface IScheduleService : IGenericService<ServiceOrderSchedule>
    {
        /// <summary>
        /// Agenda uma entrega para uma OS
        /// </summary>
        Task<ScheduleItemRecord> ScheduleDeliveryAsync(ScheduleDeliveryDto dto);
        
        /// <summary>
        /// Atualiza um agendamento existente
        /// </summary>
        Task<ServiceOrderSchedule?> UpdateScheduleAsync(int scheduleId, ScheduleDeliveryDto dto);
        
        /// <summary>
        /// Remove um agendamento
        /// </summary>
        Task<bool> RemoveScheduleAsync(int scheduleId);
        
        /// <summary>
        /// Obtém a agenda completa por setor para uma data específica
        /// </summary>
        Task<List<SectorScheduleRecord>> GetScheduleByDateAsync(DateTime date);
        
        /// <summary>
        /// Obtém a agenda de hoje destacando as OS em atraso
        /// </summary>
        Task<List<SectorScheduleRecord>> GetTodayScheduleAsync();
        
        Task<SectorScheduleRecord?> GetScheduleByCurrentSectorAsync(int sectorId, DateTime date);
        
        
        /// <summary>
        /// Marca uma entrega como realizada
        /// </summary>
        //Task<bool> MarkAsDeliveredAsync(int scheduleId);
        
        /// <summary>
        /// Atualiza automaticamente o status de atraso das OS
        /// </summary>
        Task UpdateOverdueStatusAsync();
        
        /// <summary>
        /// Obtém OS que podem ser agendadas (não finalizadas e não já agendadas)
        /// </summary>
        //Task<List<ServiceOrderSchedule>> GetAvailableForSchedulingAsync();
    }
}