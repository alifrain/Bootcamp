using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using CrudSample.Api.DTOs;
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
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employees,
                           IRepository<Department> departments,
                           IUnitOfWork uow,
                           IMapper mapper)
    {
        _employees = employees;
        _departments = departments;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync()
    {
        // ProjectTo bikin mapping di level DB, hemat data dan tanpa loop
        return await _employees.Query()
        .Include(e => e.Department)
        .AsNoTracking()
        .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var e = await _employees.GetWithDepartmentAsync(id);
        return e is null ? null : _mapper.Map<EmployeeDto>(e);
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
    {
        if (await _departments.GetByIdAsync(dto.DepartmentId) is null)
            throw new KeyNotFoundException("Department not found.");

        if (await _employees.ExistsByNameInDepartmentAsync(dto.Name, dto.DepartmentId))
            throw new InvalidOperationException("Employee name already exists in department.");

        var entity = _mapper.Map<Employee>(dto);
        await _employees.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var withDept = await _employees.GetWithDepartmentAsync(entity.Id) ?? entity;
        return _mapper.Map<EmployeeDto>(withDept);
    }

    public async Task UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        var e = await _employees.GetByIdAsync(id) ?? throw new KeyNotFoundException("Employee not found.");

        if (await _departments.GetByIdAsync(dto.DepartmentId) is null)
            throw new KeyNotFoundException("Department not found.");

        if (await _employees.ExistsByNameInDepartmentAsync(dto.Name, dto.DepartmentId, id))
            throw new InvalidOperationException("Duplicate name in department.");

        _mapper.Map(dto, e);
        _employees.Update(e);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _employees.GetByIdAsync(id) ?? throw new KeyNotFoundException("Employee not found.");
        e.IsDeleted = true;
        _employees.Update(e);
        await _uow.SaveChangesAsync();
    }
}
