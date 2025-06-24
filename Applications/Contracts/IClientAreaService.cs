using Applications.Records.Clients;

namespace Applications.Contracts
{
    public interface IClientAreaService
    {
        /// <summary>
        /// Retorna os dados consolidados da área do cliente:
        /// informações cadastrais, resumo financeiro e faturas.
        /// </summary>
        /// <param name="clientId">ID do cliente</param>
        Task<ClientDashboardRecord> GetClientBasicDashboardAsync ( int clientId );
    }
}
