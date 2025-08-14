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
        => await _deps.ListAsync() // fetch entities then map
           .ContinueWith(t => t.Result.AsQueryable()
           .Select(d => new DepartmentDto(d.Id, d.Name, d.Employees == null ? 0 : d.Employees.Count))
           .ToList() as IReadOnlyList<DepartmentDto>);

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        // need counts -> simple load + map
        var d = (await _deps.ListAsync(x => x.Id == id)).FirstOrDefault();
        return d is null ? null : new DepartmentDto(d.Id, d.Name, d.Employees?.Count ?? 0);
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto)
    {
        var entity = _mapper.Map<Department>(dto);
        await _deps.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return new DepartmentDto(entity.Id, entity.Name, 0);
    }

    public async Task UpdateAsync(int id, DepartmentUpdateDto dto)
    {
        var d = await _deps.GetByIdAsync(id) ?? throw new KeyNotFoundException("Department not found.");
        _mapper.Map(dto, d);
        _deps.Update(d);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var d = await _deps.GetByIdAsync(id) ?? throw new KeyNotFoundException("Department not found.");
        _deps.Remove(d);
        await _uow.SaveChangesAsync();
    }
}
