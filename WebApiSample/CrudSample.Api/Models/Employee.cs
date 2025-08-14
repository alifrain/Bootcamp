namespace CrudSample.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int DepartmentId { get; set; }
    public bool IsDeleted = false;
    public Department Department { get; set; } = null!;
}
