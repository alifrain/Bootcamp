namespace CrudSample.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;
    public List<Project> Projects { get; set; } = new();
}
