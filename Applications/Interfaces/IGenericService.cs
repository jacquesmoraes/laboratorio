using Core.Specifications;

namespace Applications.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<T?> GetEntityWithSpecAsync ( ISpecification<T> spec );
        Task<IReadOnlyList<T>> GetAllWithSpecAsync ( ISpecification<T> spec );
        Task<int> CountAsync ( ISpecification<T> spec );

        Task<T> CreateAsync ( T entity );
        Task<T?> UpdateAsync ( int id, T entity );
        Task<T?> DeleteAsync ( int id );
    }
}
