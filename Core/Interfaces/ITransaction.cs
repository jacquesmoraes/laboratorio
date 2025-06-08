﻿namespace Core.Interfaces
{

    public interface ITransaction : IAsyncDisposable
    {
        Task CommitAsync ( );
        Task RollbackAsync ( );
    }
}

