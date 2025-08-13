using CrudSample.Api.DTOs;

namespace CrudSample.Api.Services.Interfaces;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
    Task UpdateAsync(int id, EmployeeUpdateDto dto);
    Task DeleteAsync(int id);
}
