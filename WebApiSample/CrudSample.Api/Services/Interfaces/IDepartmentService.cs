using CrudSample.Api.DTOs;

namespace CrudSample.Api.Services.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto);
    Task UpdateAsync(int id, DepartmentUpdateDto dto);
    Task DeleteAsync(int id);
}
