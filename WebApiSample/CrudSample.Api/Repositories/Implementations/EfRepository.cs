using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CrudSample.Api.Data;
using CrudSample.Api.Repositories.Interfaces;

namespace CrudSample.Api.Repositories.Implementations;

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public EfRepository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(int id) => _set.FindAsync(id).AsTask();

    public virtual async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> q = _set.AsNoTracking();
        if (predicate != null) q = q.Where(predicate);
        return await q.ToListAsync();
    }

    public virtual Task AddAsync(T entity) => _set.AddAsync(entity).AsTask();

    public virtual void Update(T entity) => _set.Update(entity);

    public virtual void Remove(T entity) => _set.Remove(entity);
}
