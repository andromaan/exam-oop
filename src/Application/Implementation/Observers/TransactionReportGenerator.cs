using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Queries;
using Domain.Models;

namespace Application.Implementation.Observers;

public class TransactionReportGenerator(ITransactionQueries transactionQueries) : IObserver
{
    private readonly List<Transaction> _transactions =
        transactionQueries.GetAll().Result.ToList();

    public Task UpdateAsync(Transaction transaction, string action)
    {
        if (action == "add")
        {
            _transactions.Add(transaction);
        }
        else if (action == "delete")
        {
            _transactions.RemoveAll(tr => tr.Id == transaction.Id);
        }

        GenerateReport();
        return Task.CompletedTask;
    }

    private void GenerateReport()
    {
        var report = string.Join(Environment.NewLine, _transactions.Select(t =>
            $"{t.Date:yyyy-MM-dd}: {t.Type} - {t.Amount} USD"));

        File.WriteAllText("TransactionReport.txt", report);
        Console.WriteLine("Transaction report updated.");
    }
}