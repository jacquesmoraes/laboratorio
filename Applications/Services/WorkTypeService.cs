namespace Applications.Services
{
    public class WorkTypeService ( IGenericRepository<WorkType> repository )
        : GenericService<WorkType> ( repository ), IWorkTypeService
    {



        public Task<IReadOnlyList<WorkType>> GetAllWithSectionsAsync ( )
        {
            var spec = WorkTypeSpecs.All();
            return GetAllWithSpecAsync ( spec );
        }

        public Task<WorkType?> GetByIdWithSectionAsync ( int id )
        {
            var spec = WorkTypeSpecs.ByIdWithRelations(id);
            return GetEntityWithSpecAsync ( spec );
        }

        public Task<IReadOnlyList<WorkType>> GetAllForFormAsync ( )
        {
            var spec = WorkTypeSpecs.AllForForm();
            return GetAllWithSpecAsync ( spec );
        }
    }
}
