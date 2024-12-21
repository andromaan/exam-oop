using Domain.Models;

namespace Application.Abstraction.Interfaces;

public interface IObserver
{
    Task UpdateAsync(Transaction transaction, string action);
}