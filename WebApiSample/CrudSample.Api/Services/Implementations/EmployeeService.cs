using Microsoft.EntityFrameworkCore;
using CrudSample.Api.Models;
using CrudSample.Api.Repositories.Interfaces;
using CrudSample.Api.Repositories.Implementations;
using CrudSample.Api.Services.Interfaces;

namespace CrudSample.Api.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;
    private readonly IRepository<Department> _departments;
    private readonly IUnitOfWork _uow;

    public EmployeeService(IEmployeeRepository employees,
                           IRepository<Department> departments,
                           IUnitOfWork uow)
    {
        _employees = employees;
        _departments = departments;
        _uow = uow;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync()
    {
        // include Department to avoid N+1 when controller needs department name later
        return await _employees.Query().Include(e => e.Department).AsNoTracking().ToListAsync();
    }

    public Task<Employee?> GetByIdAsync(int id) => _employees.GetWithDepartmentAsync(id);

    public async Task<Employee> CreateAsync(Employee employee)
    {
        // simple business rule example: department must exist
        var dept = await _departments.GetByIdAsync(employee.DepartmentId);
        if (dept is null) throw new KeyNotFoundException("Department not found.");

        if (await _employees.ExistsByNameInDepartmentAsync(employee.Name, employee.DepartmentId))
            throw new InvalidOperationException("Employee name already exists in department.");

        await _employees.AddAsync(employee);
        await _uow.SaveChangesAsync();

        // reload with department
        return await _employees.GetWithDepartmentAsync(employee.Id) ?? employee;
    }

    public async Task UpdateAsync(int id, Employee updated)
    {
        var e = await _employees.GetByIdAsync(id) ?? throw new KeyNotFoundException("Employee not found.");
        if (await _employees.ExistsByNameInDepartmentAsync(updated.Name, updated.DepartmentId, id))
            throw new InvalidOperationException("Duplicate name in department.");

        // map allowed fields (simple example)
        e.Name = updated.Name;
        e.DepartmentId = updated.DepartmentId;

        _employees.Update(e);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _employees.GetByIdAsync(id) ?? throw new KeyNotFoundException("Employee not found.");
        _employees.Remove(e);
        await _uow.SaveChangesAsync();
    }

    public async Task TransferDepartmentAsync(int employeeId, int newDepartmentId)
    {
        var e = await _employees.GetByIdAsync(employeeId) ?? throw new KeyNotFoundException("Employee not found.");
        if (e.DepartmentId == newDepartmentId) return;

        if (await _departments.GetByIdAsync(newDepartmentId) is null)
            throw new KeyNotFoundException("Target department not found.");

        if (await _employees.ExistsByNameInDepartmentAsync(e.Name, newDepartmentId, e.Id))
            throw new InvalidOperationException("Duplicate name in target department.");

        e.DepartmentId = newDepartmentId;
        _employees.Update(e);
        await _uow.SaveChangesAsync();
    }
}
