using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync ( );
        Task<int> SaveChangesAsync ( );
    }
}
