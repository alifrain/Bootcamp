using Microsoft.AspNetCore.Mvc;
using CrudSample.Api.Data;
using CrudSample.Api.Models;

namespace CrudSample.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public DepartmentsController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create(Department d)
    {
        await _db.Departments.AddAsync(d);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = d.Id }, d);
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_db.Departments.Select(x => new { x.Id, x.Name }));
}
