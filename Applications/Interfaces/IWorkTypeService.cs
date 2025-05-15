using Core.Models.Works;

namespace Applications.Interfaces
{
    public interface IWorkTypeService : IGenericService<WorkType>
    {
        Task<IReadOnlyList<WorkType>> GetAllWithSectionsAsync ( );
        Task<WorkType?> GetByIdWithSectionAsync ( int id );
    }
}
