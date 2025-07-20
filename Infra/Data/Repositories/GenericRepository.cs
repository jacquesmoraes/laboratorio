
using Core.Interfaces;
using Core.Specifications;
using System.Linq.Expressions;

namespace Infra.Data.Repositories
{
    public class GenericRepository<T> ( ApplicationContext context ) : IGenericRepository<T> where T : class
    {
        private readonly ApplicationContext _context = context;
        private readonly DbSet<T> _dbSet = context.Set<T>();


        public async Task<IReadOnlyList<T>> GetAllAsync ( ISpecification<T> spec )
        {
            return await ApplySpecification ( spec ).ToListAsync ( );
        }

        public Task<T?> GetByIdAsync ( int id, ISpecification<T> spec )
        {
            return ApplySpecification ( spec ).FirstOrDefaultAsync ( x => EF.Property<int> ( x, "Id" ) == id );
        }


        public async Task<T> CreateAsync ( T entity )
        {
            _dbSet.Add ( entity );
            await _context.SaveChangesAsync ( );
            return entity;
        }


        public async Task<T?> UpdateAsync ( int id, T entity )
        {
            var existing = await _dbSet.FindAsync(id);
            if ( existing == null )
                return null;

            _context.Entry ( existing ).CurrentValues.SetValues ( entity );
            await _context.SaveChangesAsync ( );
            return existing;
        }
        public async Task DeleteAsync ( T entity )
        {
            _dbSet.Remove ( entity );
            await _context.SaveChangesAsync ( );
        }

        public async Task<T?> DeleteAsync ( int id )
        {
            var entity = await _dbSet.FindAsync ( id );
            if ( entity == null )
                return null;
            _dbSet.Remove ( entity );
            await _context.SaveChangesAsync ( );
            return entity;
        }

        public async Task<T?> GetEntityWithSpec ( ISpecification<T> spec )
        {
            return await ApplySpecification ( spec ).FirstOrDefaultAsync ( );
        }

        public async Task<decimal> SumAsync ( Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector )
        {
            return await _context.Set<T> ( )
                .Where ( predicate )
                .Select ( selector )
                .SumAsync ( );
        }

        public async Task<IReadOnlyList<T>> GetAllWithoutTrackingAsync ( ISpecification<T> spec )
        {
            var query = ApplySpecification(spec);
            return await query.AsNoTracking ( ).ToListAsync ( );
        }

        public async Task<T?> GetEntityWithSpecWithoutTrackingAsync ( ISpecification<T> spec )
        {
            var query = ApplySpecification(spec);
            return await query.AsNoTracking ( ).FirstOrDefaultAsync ( );
        }

        public async Task<int> CountWithoutTrackingAsync ( ISpecification<T> spec )
        {
            var query = ApplySpecification(spec);
            return await query.AsNoTracking ( ).CountAsync ( );
        }

        public async Task<int> CountAsync ( ISpecification<T> spec )
        {
            return await ApplySpecification ( spec ).CountAsync ( );
        }


        private IQueryable<T> ApplySpecification ( ISpecification<T> spec )
        {
            var query = _dbSet.AsQueryable();

            if ( spec.Criteria != null )
            {
                query = query.Where ( spec.Criteria );
            }

            query = spec.Includes.Aggregate ( query, ( current, include ) => current.Include ( include ) );
            query = spec.IncludeStrings.Aggregate ( query, ( current, include ) => current.Include ( include ) );

            if ( spec.OrderBy != null )
            {
                query = query.OrderBy ( spec.OrderBy );
            }
            else if ( spec.OrderByDescending != null )
            {
                query = query.OrderByDescending ( spec.OrderByDescending );
            }

            if ( spec.IsPagingEnabled )
            {
                query = query.Skip ( spec.Skip )
                            .Take ( spec.Take );
            }


            return query;
        }


    }
}
