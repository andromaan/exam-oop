using Application.Abstraction.ViewModels;
using FluentValidation;

namespace Application.Abstraction.Validatiors;

public class EmployeeValidator : AbstractValidator<EmployeeVM>
{
    public EmployeeValidator()
    {
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Name cannot be empty");
        RuleFor(x=> x.Position).NotEmpty().WithMessage("Position cannot be empty");
        RuleFor(x=>x.Salary)
            .GreaterThan(0).WithMessage("Salary must be greater than 0")
            .NotEmpty().WithMessage("Salary cannot be empty");
    }
}