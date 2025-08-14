using CrudSample.Api.Models;

namespace CrudSample.Api.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetWithDepartmentAsync(int id);
    Task<bool> ExistsByNameInDepartmentAsync(string name, int departmentId, int? excludeId = null);
}
