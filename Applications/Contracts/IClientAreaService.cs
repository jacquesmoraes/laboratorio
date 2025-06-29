namespace Applications.Contracts
{
    public interface IClientAreaService
    {
          /// <summary>
        /// Returns consolidated data for the client area:
        /// personal information, financial summary, and invoices.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        Task<ClientDashboardRecord> GetClientBasicDashboardAsync ( int clientId );
    }
}
