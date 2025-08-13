namespace ef.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public List<Project> Projects { get; set; } = new();  // <— EXACTLY this
}
