using Core.Interfaces;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Data
{
    public class UnitOfWork ( ApplicationContext context ) : IUnitOfWork
    {
        private readonly ApplicationContext _context = context;
        private readonly Dictionary<Type, object> _repositories = new();

        public IGenericRepository<T> Repository<T> ( ) where T : class
        {
            if ( _repositories.TryGetValue ( typeof ( T ), out var repo ) )
                return ( IGenericRepository<T> ) repo;

            var newRepo = new GenericRepository<T>(_context);
            _repositories [typeof ( T )] = newRepo;
            return newRepo;
        }
        public async Task<ITransaction> BeginTransactionAsync ( )
        {
            IDbContextTransaction efTransaction = await _context.Database.BeginTransactionAsync();
            return new TransactionWrapper ( efTransaction );
        }

        public async Task<int> SaveChangesAsync ( )
        {
            return await _context.SaveChangesAsync ( );
        }

        public void Dispose ( )
        {
            _context.Dispose ( );
        }
    }
}
