using System.Linq.Expressions;

namespace CrudSample.Api.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();  // <- THIS must exist on the interface

    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
