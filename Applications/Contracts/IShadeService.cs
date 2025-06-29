namespace Applications.Contracts
{
    public interface IShadeService : IGenericService<Shade>
    {
        Task<Shade> CreateShade ( CreateShadeDto dto );
        Task<Shade?> UpdateWithValidationAsync ( int id, CreateShadeDto dto );
    }
}
