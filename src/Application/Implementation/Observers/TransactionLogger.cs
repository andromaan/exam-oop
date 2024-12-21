using Application.Abstraction.Interfaces;
using Domain.Models;

namespace Application.Implementation.Observers;

public class TransactionLogger : IObserver
{
    public Task UpdateAsync(Transaction transaction, string action)
    {
        Console.WriteLine($"Transaction {action}: {transaction.Type} - {transaction.Amount} on {transaction.Date}");
        return Task.CompletedTask;
    }
}
