using Applications.Contracts;
using Core.Interfaces;
using Core.Specifications;

namespace Applications.Services
{
    public class GenericService<T> ( IGenericRepository<T> repository ) : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository = repository;



        public Task<T?> GetEntityWithSpecAsync ( ISpecification<T> spec ) =>
          _repository.GetEntityWithSpec ( spec );

        public Task<IReadOnlyList<T>> GetAllWithSpecAsync ( ISpecification<T> spec ) =>
            _repository.GetAllAsync ( spec );

        public Task<int> CountAsync ( ISpecification<T> spec ) =>
            _repository.CountAsync ( spec );

        public Task<T> CreateAsync ( T entity ) =>
            _repository.CreateAsync ( entity );
        public Task DeleteAsync ( T entity ) =>
            _repository.DeleteAsync ( entity );


        public Task<T?> UpdateAsync ( int id, T entity ) =>
            _repository.UpdateAsync ( id, entity );

        public Task<T?> DeleteAsync ( int id ) =>
            _repository.DeleteAsync ( id );
    }
}
