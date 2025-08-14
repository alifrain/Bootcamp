using CrudSample.Api.Data;
using CrudSample.Api.Models;
using CrudSample.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudSample.Api.Repositories.Implementations;

public class EmployeeRepository : EfRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext db) : base(db) { }

    public override IQueryable<Employee> Query()              // <- override, not hide
        => _set.AsNoTracking().Include(e => e.Department);

    public Task<Employee?> GetWithDepartmentAsync(int id)
        => _set.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id);

    public Task<bool> ExistsByNameInDepartmentAsync(string name, int deptId, int? excludeId = null)
        => _set.AnyAsync(e => e.DepartmentId == deptId && e.Name == name && (excludeId == null || e.Id != excludeId));
}
