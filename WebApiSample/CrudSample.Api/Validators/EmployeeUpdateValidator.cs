using CrudSample.Api.DTOs;
using FluentValidation;

namespace CrudSample.Api.Validators;

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
