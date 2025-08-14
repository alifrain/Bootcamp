using AutoMapper;
using CrudSample.Api.DTOs;
using CrudSample.Api.Models;

namespace CrudSample.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeDto>()
        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.Name));

        CreateMap<EmployeeCreateDto, Employee>();
        CreateMap<EmployeeUpdateDto, Employee>();

        CreateMap<Department, DepartmentDto>()
        .ForMember(d => d.EmployeesCount, o => o.MapFrom(s => s.Employees.Count));

        CreateMap<DepartmentCreateDto, Department>();
        CreateMap<DepartmentUpdateDto, Department>();
    }
}
