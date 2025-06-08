using Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Data
{
    public class UnitOfWork ( ApplicationContext context ) : IUnitOfWork
    {
        private readonly ApplicationContext _context = context;

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
