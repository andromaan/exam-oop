using Application.Abstraction.Interfaces;
using Domain.Models;

namespace Application.Implementation.Observers;

public class TransactionLogger : IObserver
{
    public Task UpdateAsync(Transaction transaction, string action)
    {
        Console.WriteLine($"Transaction {action}: {transaction.TypeId} - {transaction.Amount} on {transaction.Date}");
        return Task.CompletedTask;
    }
}
