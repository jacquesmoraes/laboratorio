namespace Applications.Contracts
{
    public interface IScheduleService : IGenericService<ServiceOrderSchedule>
    {
        /// <summary>
        /// Schedules a delivery for a service order.
        /// </summary>
        Task<ScheduleItemRecord> ScheduleDeliveryAsync(ScheduleDeliveryDto dto);

        /// <summary>
        /// Updates an existing schedule.
        /// </summary>
        Task<ServiceOrderSchedule?> UpdateScheduleAsync(int scheduleId, ScheduleDeliveryDto dto);

        /// <summary>
        /// Removes a schedule.
        /// </summary>
        Task<bool> RemoveScheduleAsync(int scheduleId);

        /// <summary>
        /// Gets the full schedule per sector for a specific date.
        /// </summary>
        Task<List<SectorScheduleRecord>> GetScheduleByDateAsync(DateTime date);

        /// <summary>
        /// Gets today's schedule, highlighting overdue service orders.
        /// </summary>
        Task<List<SectorScheduleRecord>> GetTodayScheduleAsync();

        Task<SectorScheduleRecord?> GetScheduleByCurrentSectorAsync(int sectorId, DateTime date);

        /// <summary>
        /// Automatically updates the overdue status of service orders.
        /// </summary>
        Task UpdateOverdueStatusAsync();

        Task<List<SectorScheduleRecord>> GetScheduleByDateRangeAsync(DateTime start, DateTime end);


       
    }
}
