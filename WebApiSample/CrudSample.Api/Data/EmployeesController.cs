using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrudSample.Api.DTOs;
using CrudSample.Api.Services.Interfaces;

namespace CrudSample.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _svc;
    public EmployeesController(IEmployeeService svc) => _svc = svc;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeCreateDto dto)
    {
        var created = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, EmployeeUpdateDto dto)
    {
        await _svc.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
