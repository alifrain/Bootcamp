using CrudSample.Api.DTOs;
using FluentValidation;

namespace CrudSample.Api.Validators;

public class EmployeeCreateValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 chars.")
            .MaximumLength(80);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be > 0.");
    }
}

public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(3).MaximumLength(80);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0);
    }
}
