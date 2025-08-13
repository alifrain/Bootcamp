namespace CrudSample.Api.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Navigation
    public List<Employee> Employees { get; set; } = new();
}
