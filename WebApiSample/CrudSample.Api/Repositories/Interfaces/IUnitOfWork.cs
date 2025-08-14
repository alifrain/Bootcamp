using CrudSample.Api.Data;

namespace CrudSample.Api.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}