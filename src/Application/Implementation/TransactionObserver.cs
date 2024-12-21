using Application.Abstraction.Interfaces;

namespace Application.Implementation;

public class TransactionObserver : IObserver
{
    private readonly string _name;

    public TransactionObserver(string name)
    {
        _name = name;
    }

    public void Update(string message)
    {
        Console.WriteLine($"[{_name}] Отримано сповіщення: {message}");
    }
}