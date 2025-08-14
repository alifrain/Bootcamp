using CrudSample.Api.DTOs;
using FluentValidation;

namespace CrudSample.Api.Validators;

public class DepartmentCreateValidator : AbstractValidator<DepartmentCreateDto>
{
    public DepartmentCreateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
    }
}
public class DepartmentUpdateValidator : AbstractValidator<DepartmentUpdateDto>
{
    public DepartmentUpdateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
    }
}
