using Microsoft.EntityFrameworkCore;
using CrudSample.Api.Data;
using CrudSample.Api.Models;
using CrudSample.Api.Repositories.Interfaces;

namespace CrudSample.Api.Repositories.Implementations;

public class EmployeeRepository : EfRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext db) : base(db) { }

    public Task<Employee?> GetWithDepartmentAsync(int id) =>
        _set.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id);

    public Task<bool> ExistsByNameInDepartmentAsync(string name, int departmentId, int? excludeId = null) =>
        _set.AnyAsync(e => e.DepartmentId == departmentId
                         && e.Name == name
                         && (excludeId == null || e.Id != excludeId));

    public IQueryable<Employee> Query() => _set.AsQueryable();
}
