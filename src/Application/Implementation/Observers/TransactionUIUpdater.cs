using Application.Abstraction.Interfaces;
using Domain.Models;

namespace Application.Implementation.Observers;

public class TransactionUIUpdater : IObserver
{
    public Task UpdateAsync(Transaction transaction, string action)
    {
        Console.WriteLine($"UI updated: Transaction {action} for {transaction.Type}.");
        return Task.CompletedTask;
    }
}
