using Domain.Models;

namespace Api.DTOs;

public record TransactionDTO(Guid Id, Guid EmployeeId, decimal Amount, DateTime Date, string Type)
{
    public static TransactionDTO FromDomainModel(Transaction transaction)
        => new(transaction.Id, transaction.EmployeeId, transaction.Amount, transaction.Date, transaction.TypeId);
}