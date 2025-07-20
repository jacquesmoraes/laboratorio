using Core.Specifications;
using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync ( ISpecification<T> spec );
        Task<T?> GetByIdAsync ( int id, ISpecification<T> spec );
        Task<T> CreateAsync ( T entity );
        Task<T?> UpdateAsync ( int id, T entity );
        Task<T?> DeleteAsync ( int id );
        Task DeleteAsync ( T entity );

        Task<T?> GetEntityWithSpec ( ISpecification<T> spec );
        Task<int> CountAsync ( ISpecification<T> spec );
        Task<decimal> SumAsync ( Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector );

        Task<IReadOnlyList<T>> GetAllWithoutTrackingAsync ( ISpecification<T> spec );
        Task<T?> GetEntityWithSpecWithoutTrackingAsync ( ISpecification<T> spec );
        Task<int> CountWithoutTrackingAsync ( ISpecification<T> spec );



    }


}
