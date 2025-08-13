namespace CrudSample.Api.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Navigation many-to-many
    public List<Employee> Members { get; set; } = new();
}
