namespace CrudSample.Api.DTOs;

public record EmployeeDto(int Id, string Name, int DepartmentId, string DepartmentName);

public record EmployeeCreateDto(string Name, int DepartmentId);

public record EmployeeUpdateDto(string Name, int DepartmentId);