namespace Applications.Contracts
{
    public interface IWorkTypeService : IGenericService<WorkType>
    {
        Task<IReadOnlyList<WorkType>> GetAllWithSectionsAsync ( );
        Task<WorkType?> GetByIdWithSectionAsync ( int id );
        Task<IReadOnlyList<WorkType>> GetAllForFormAsync();
    }
}
