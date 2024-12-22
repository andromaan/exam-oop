﻿using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Repositories;
using Application.Abstraction.ViewModels;
using Application.Implementation.Observers;
using Domain.Constants;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Implementation;

public class PayrollManager
{
    private readonly IServiceProvider serviceProvider;
    private readonly TransactionNotifier notifier;
    private readonly ILogger logger;

    public PayrollManager(IServiceProvider serviceProvider, TransactionNotifier notifier, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.notifier = notifier;
        this.logger = logger;
        
        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        
        notifier.Subscribe(scopedProvider.GetRequiredService<TransactionLogger>());
        notifier.Subscribe(scopedProvider.GetRequiredService<TransactionUIUpdater>());
        notifier.Subscribe(scopedProvider.GetRequiredService<TransactionReportGenerator>());
    }

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
        ExecuteInScopeEmployeeAsync(async repo =>
        {
            var employee = await repo.Get(employeeId);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(employeeId);
            }

            return await repo.Delete(employeeId);
        });

    public Task<Transaction> CreateTransactionAsync(TransactionVM request) =>
        ExecuteInScopeTransactionAsync(async repo =>
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var employee = await ExecuteInScopeEmployeeAsync(repository
                => repository.Get(request.EmployeeId));

            if (employee == null)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            var transaction = new Transaction(Guid.NewGuid(), employee.Id, request.Amount, request.Type);

            await notifier.NotifyAsync(transaction, ActionsConstants.Add);

            logger.Log(
                $"{ActionsConstants.Add} Transaction {transaction.Date}:" +
                $" {transaction.TypeId} - {transaction.Amount} USD, id: {transaction.Id}");

            return await repo.Add(transaction);
        });

    public Task<IReadOnlyList<Transaction>> GetTransactionsByEmployeeAsync(Guid employeeId) =>
        ExecuteInScopeTransactionAsync(async repo =>
        {
            var employee = await ExecuteInScopeEmployeeAsync(repository
                => repository.Get(employeeId));

            if (employee == null)
            {
                throw new EmployeeNotFoundException(employeeId);
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
            {
                throw new TransactionNotFoundException(transactionId);
            }

            var deletedTransaction = await repo.Delete(transactionId);

            await notifier.NotifyAsync(transaction, ActionsConstants.Delete);

            logger.Log(
                $"{ActionsConstants.Delete} Transaction {transaction.Date}: " +
                $"{transaction.TypeId} - {transaction.Amount} USD, id: {transaction.Id}");

            return deletedTransaction;
        });
}