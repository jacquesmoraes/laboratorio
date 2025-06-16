namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<ITransaction> BeginTransactionAsync ( );
        Task<int> SaveChangesAsync ( );
        IGenericRepository<T> Repository<T>() where T : class;
    }
}
