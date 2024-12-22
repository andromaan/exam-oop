using Application.Abstraction.Interfaces;
using Domain.Models;

namespace Application.Implementation;

public class TransactionNotifier
{
    private readonly List<IObserver> _observers = new();

    public void Subscribe(IObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Unsubscribe(IObserver observer)
    {
        if (_observers.Contains(observer))
            _observers.Remove(observer);
    }

    public async Task NotifyAsync(Transaction transaction, string action)
    {
        foreach (var observer in _observers)
        {
            try
            {
                await observer.UpdateAsync(transaction, action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
