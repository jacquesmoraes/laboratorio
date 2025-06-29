namespace Applications.Contracts
{
    public interface ISectorService : IGenericService<Sector>
    {
        Task<Sector?> UpdateFromDtoAsync ( UpdateSectorDto dto );
    }
}
