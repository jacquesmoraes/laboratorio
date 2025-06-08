using Core.Models.Clients;

namespace Applications.Contracts
{
    public interface IClientBalanceService
    {
        /// <summary>
        /// Calculates the balance for a given client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>A task that represents the asynchronous operation, containing the client's balance.</returns>
        Task<ClientBalance> GetClientBalanceAsync ( int clientId );
    
       
    }
}
