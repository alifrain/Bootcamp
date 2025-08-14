namespace CrudSample.Api.DTOs;

public record DepartmentCreateDto(string Name);
public record DepartmentUpdateDto(string Name);
public record DepartmentDto(int Id, string Name, int EmployeesCount);
