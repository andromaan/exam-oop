using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Repositories;
using Application.Abstraction.ViewModels;
using Application.Implementation.Observers;
using Domain.Constants;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Implementation.PayrollManager;

public sealed class PayrollManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TransactionNotifier _notifier;
    private readonly ILogger _logger;

    private static volatile PayrollManager _instance;
    private static readonly object _lock = new();

    private PayrollManager(IServiceProvider serviceProvider, TransactionNotifier notifier, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _notifier = notifier;
        _logger = logger;

        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        _notifier.Subscribe(scopedProvider.GetRequiredService<TransactionLogger>());
        _notifier.Subscribe(scopedProvider.GetRequiredService<TransactionUIUpdater>());
        _notifier.Subscribe(scopedProvider.GetRequiredService<TransactionReportGenerator>());
    }

    public static PayrollManager GetInstance(IServiceProvider serviceProvider, TransactionNotifier notifier, ILogger logger)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new PayrollManager(serviceProvider, notifier, logger);
                }
            }
        }
        return _instance;
    }
    
    private async Task<T> ExecuteInScopeAsync<T, TRepo>(Func<TRepo, Task<T>> operation) where TRepo : class
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<TRepo>();
        return await operation(repository);
    }

    public Task<Employee> AddEmployeeAsync(EmployeeVM request) =>
        ExecuteInScopeAsync<Employee, IEmployeeRepository>(async repo =>
        {
            var employee = new Employee(Guid.NewGuid(), request.Name, request.Position, request.Salary);

            if (request == null) throw new ArgumentNullException(nameof(request));
            try
            {
                return await repo.Add(employee);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        });

    public Task<Employee> DeleteEmployeeAsync(Guid employeeId) =>
        ExecuteInScopeAsync<Employee, IEmployeeRepository>(async repo =>
        {
            var employee = await repo.Get(employeeId);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(employeeId);
            }

            return await repo.Delete(employeeId);
        });

    public Task<Transaction> CreateTransactionAsync(TransactionVM request) =>
        ExecuteInScopeAsync<Transaction, ITransactionRepository>(async repo =>
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var employee = await ExecuteInScopeAsync<Employee, IEmployeeRepository>(repository
                => repository.Get(request.EmployeeId));

            if (employee == null)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            var transaction = new Transaction(Guid.NewGuid(), employee.Id, request.Amount, request.Type);

            await _notifier.NotifyAsync(transaction, ActionsConstants.Add);

            _logger.Log(
                $"{ActionsConstants.Add} Transaction {transaction.Date}:" +
                $" {transaction.TypeId} - {transaction.Amount} USD, id: {transaction.Id}");

            return await repo.Add(transaction);
        });

    public Task<IReadOnlyList<Transaction>> GetTransactionsByEmployeeAsync(Guid employeeId) =>
        ExecuteInScopeAsync<IReadOnlyList<Transaction>, ITransactionRepository>(async repo =>
        {
            var employee = await ExecuteInScopeAsync<Employee, IEmployeeRepository>(repository
                => repository.Get(employeeId));

            if (employee == null)
            {
                throw new EmployeeNotFoundException(employeeId);
            }

            return await repo.GetAllForEmployee(employeeId);
        });

    public Task<decimal> GetTotalPayoutsAsync(DateTime startDate, DateTime endDate) =>
        ExecuteInScopeAsync<decimal, ITransactionRepository>(async repo =>
        {
            if (startDate > endDate || startDate == endDate || endDate > DateTime.UtcNow)
            {
                throw new DatePeriodInvalidException(startDate, endDate);
            }
            
            var transactions = await repo.GetAll();

            return transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .Sum(t => t.Amount);
        });

    public Task<Transaction> DeleteTransactionAsync(Guid transactionId) =>
        ExecuteInScopeAsync<Transaction, ITransactionRepository>(async repo =>
        {
            var transaction = await repo.Get(transactionId);
            if (transaction == null)
            {
                throw new TransactionNotFoundException(transactionId);
            }

            var deletedTransaction = await repo.Delete(transactionId);

            await _notifier.NotifyAsync(transaction, ActionsConstants.Delete);

            _logger.Log(
                $"{ActionsConstants.Delete} Transaction {transaction.Date}: " +
                $"{transaction.TypeId} - {transaction.Amount} USD, id: {transaction.Id}");

            return deletedTransaction;
        });
    
    public static void ResetInstance()
    {
        lock (_lock)
        {
            _instance = null;
        }
    }

}