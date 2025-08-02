namespace Applications.Contracts
{
    public interface IClientService : IGenericService<Client>
    {
        Task<Client> CreateClientAsync ( Client entity );
        
        Task<Client?> UpdateFromDtoAsync ( UpdateClientDto dto );
        Task<ClientResponseDetailsProjection> GetClientDetailsProjectionAsync ( int id );
        Task<Pagination<ClientResponseRecord>> GetPaginatedAsync(QueryParams parameters);

    }


}
