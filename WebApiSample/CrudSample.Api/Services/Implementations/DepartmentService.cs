using AutoMapper;
using AutoMapper.QueryableExtensions;
using CrudSample.Api.DTOs;
using CrudSample.Api.Models;
using CrudSample.Api.Repositories.Interfaces;
using CrudSample.Api.Repositories.Implementations;
using CrudSample.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudSample.Api.Services.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly IRepository<Department> _deps;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly AutoMapper.IConfigurationProvider _cfg;

    public DepartmentService(IRepository<Department> deps, IUnitOfWork uow, IMapper mapper)
    {
        _deps = deps;
        _uow = uow;
        _mapper = mapper;
        _cfg = mapper.ConfigurationProvider;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync()
        => await _deps.Query()
                      .Include(d => d.Employees)
                      .AsNoTracking()
                      .ProjectTo<DepartmentDto>(_cfg)
                      .ToListAsync();

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var d = await _deps.Query()
                           .Include(x => x.Employees)
                           .FirstOrDefaultAsync(x => x.Id == id);
        return d is null ? null : _mapper.Map<DepartmentDto>(d);
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto)
    {
        var name = dto.Name.Trim();
        var exists = await _deps.Query().AnyAsync(x => x.Name.ToLower() == name.ToLower());
        if (exists) throw new InvalidOperationException("Department name already exists.");

        var entity = _mapper.Map<Department>(dto);
        entity.Name = name;

        await _deps.AddAsync(entity);
        await _uow.SaveChangesAsync();

        // return fresh DTO (EmployeesCount = 0)
        return new DepartmentDto(entity.Id, entity.Name, 0);
    }

    public async Task UpdateAsync(int id, DepartmentUpdateDto dto)
    {
        var d = await _deps.GetByIdAsync(id) ?? throw new KeyNotFoundException("Department not found.");

        var name = dto.Name.Trim();
        var exists = await _deps.Query()
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != id);
        if (exists) throw new InvalidOperationException("Department name already exists.");

        _mapper.Map(dto, d);
        d.Name = name;

        _deps.Update(d);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var d = await _deps.Query()
                           .Include(x => x.Employees)
                           .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException("Department not found.");

        // optional rule: block delete if it still has employees
        if ((d.Employees?.Count ?? 0) > 0)
            throw new InvalidOperationException("Cannot delete a department with employees.");

        _deps.Remove(d);
        await _uow.SaveChangesAsync();
    }
}
