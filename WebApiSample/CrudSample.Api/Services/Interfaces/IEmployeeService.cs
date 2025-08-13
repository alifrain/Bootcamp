using CrudSample.Api.Models;

namespace CrudSample.Api.Services.Interfaces;

public interface IEmployeeService
{
    Task<IReadOnlyList<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> CreateAsync(Employee employee);
    Task UpdateAsync(int id, Employee updated);
    Task DeleteAsync(int id);
    Task TransferDepartmentAsync(int employeeId, int newDepartmentId);
}
