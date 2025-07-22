namespace StoreManagement.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
    Task RollbackSync();
}
