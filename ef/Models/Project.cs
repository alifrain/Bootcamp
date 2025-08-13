namespace ef.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public List<Employee> Employees { get; set; } = new();
}

