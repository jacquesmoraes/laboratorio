﻿using Core.Models.Works;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync ( ISpecification<T> spec );
        Task<T?> GetByIdAsync ( int id,ISpecification<T> spec );
        Task<T> CreateAsync ( T entity );
        Task<T?> UpdateAsync ( int id, T entity );
        Task<T?> DeleteAsync ( int id );
        Task<T?> GetEntityWithSpec ( ISpecification<T> spec );
        Task<int> CountAsync ( ISpecification<T> spec );
        

    }


}
