using Domain.Models;

namespace Application.Abstraction.ViewModels;

public record TransactionVM(Guid Id, Guid EmployeeId, decimal Amount, DateTime Date, string Type)
{
    public TransactionVM FromDomainModel(Transaction transaction)
        => new(transaction.Id, transaction.EmployeeId, transaction.Amount, transaction.Date, transaction.Type);

    public Transaction ToDomainModel()
        => new(Id, EmployeeId, Amount, Type);
}