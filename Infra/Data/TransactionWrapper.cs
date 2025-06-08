using Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Data
{
    public class TransactionWrapper ( IDbContextTransaction inner ) : ITransaction
    {
        private readonly IDbContextTransaction _inner = inner;

        public Task CommitAsync ( ) => _inner.CommitAsync ( );

        public Task RollbackAsync ( ) => _inner.RollbackAsync ( );

        public ValueTask DisposeAsync ( ) => _inner.DisposeAsync ( );
    }
}
