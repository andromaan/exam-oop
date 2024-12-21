using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Repositories;
using Application.Abstraction.ViewModels;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Implementation;

public class PayrollManager(IServiceProvider serviceProvider, TransactionNotifier notifier)
{
    private async Task<T> ExecuteInScopeEmployeeAsync<T>(Func<IEmployeeRepository, Task<T>> operation)
    {
        using var scope = serviceProvider.CreateScope();
        var employeeRepository = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
        return await operation(employeeRepository);
    }

    private async Task<T> ExecuteInScopeTransactionAsync<T>(Func<ITransactionRepository, Task<T>> operation)
    {
        using var scope = serviceProvider.CreateScope();
        var employeeRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        return await operation(employeeRepository);
    }

    public Task<Employee> AddEmployeeAsync(EmployeeVM request) =>
        ExecuteInScopeEmployeeAsync(async repo =>
        {
            var employee = new Employee(new Guid(), request.Name, request.Position, request.Salary);

            if (request == null) throw new ArgumentNullException(nameof(request));
            return await repo.Add(employee);
        });

    public Task<Employee> DeleteEmployeeAsync(Guid employeeId) =>
        ExecuteInScopeEmployeeAsync(async repo =>
        {
            var employee = await repo.Get(employeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
            }

            return await repo.Delete(employeeId);
        });

    public Task<Transaction> CreateTransactionAsync(TransactionVM request) =>
        ExecuteInScopeTransactionAsync(async repo =>
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var employee = await ExecuteInScopeEmployeeAsync( repository 
                => repository.Get(request.EmployeeId));

            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with {request.EmployeeId} not found.");
            }

            var transaction = new Transaction(new Guid(), employee.Id, request.Amount, request.Type);
            
            await notifier.NotifyAsync(transaction, "add");
            
            return await repo.Add(transaction);
        });

    public Task<IReadOnlyList<Transaction>> GetTransactionsByEmployeeAsync(Guid employeeId) =>
        ExecuteInScopeTransactionAsync(async repo =>
        {
            var employee = await ExecuteInScopeEmployeeAsync( repository 
                => repository.Get(employeeId));
            
            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with {employeeId} not found.");
            }

            return await repo.GetAllForEmployee(employeeId);
        });

    public Task<decimal> GetTotalPayoutsAsync(DateTime startDate, DateTime endDate) =>
        ExecuteInScopeTransactionAsync(async repo =>
        {
            var transactions = await repo.GetAll();

            return transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .Sum(t => t.Amount);
        });
    
    public Task<Transaction> DeleteTransactionAsync(Guid transactionId) =>
        ExecuteInScopeTransactionAsync(async repo =>
        {
            var transaction = await repo.Get(transactionId);
            if (transaction == null)
                throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");

            var deletedTransaction = await repo.Delete(transactionId);

            await notifier.NotifyAsync(transaction, "delete");

            return deletedTransaction;
        });
}