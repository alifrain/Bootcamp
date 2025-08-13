using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrudSample.Api.Models;
using CrudSample.Api.Services.Interfaces;
using CrudSample.Api.DTOs;

namespace CrudSample.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _svc;
    public EmployeesController(IEmployeeService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _svc.GetByIdAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [AllowAnonymous] // nanti aktifkan JWT
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto)
    {
        var entity = new Employee { Name = dto.Name, DepartmentId = dto.DepartmentId };
        var created = await _svc.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new {
            created.Id, created.Name, created.DepartmentId,
            DepartmentName = created.Department?.Name
        });
    }


    [AllowAnonymous]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Employee payload)
    {
        await _svc.UpdateAsync(id, payload);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("{id:int}/transfer/{deptId:int}")]
    public async Task<IActionResult> Transfer(int id, int deptId)
    {
        await _svc.TransferDepartmentAsync(id, deptId);
        return NoContent();
    }
}
