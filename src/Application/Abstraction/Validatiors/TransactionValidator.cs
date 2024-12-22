using System.Data;
using Application.Abstraction.ViewModels;
using FluentValidation;

namespace Application.Abstraction.Validatiors;

public class TransactionValidator : AbstractValidator<TransactionVM>
{
    public TransactionValidator()
    {
        RuleFor(x=>x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .NotNull().WithMessage("Amount cannot be null");
        RuleFor(x=>x.EmployeeId).NotNull().WithMessage("EmployeeId cannot be null");
        RuleFor(x=>x.Type).NotNull().WithMessage("Type cannot be null");
    }
}