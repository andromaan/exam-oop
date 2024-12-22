using System.Net;
using System.Net.Http.Json;
using Api.DTOs;
using Application.Abstraction.ViewModels;
using Application.Implementation.Observers;
using Domain.Constants;
using Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class TransactionsControllerTest
    : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Transaction _mainTransaction;
    private readonly Employee _mainEmployee = EmployeeData.MainEmployee();

    public TransactionsControllerTest(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainTransaction = TransactionData.MeinTransaction(_mainEmployee.Id);
    }

    [Fact]
    public async Task ShouldCreateTransactionClass()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var amount = 1000.50m;
        var typeId = "Bonus";

        // Act
        var transaction = new Transaction(transactionId, employeeId, amount, typeId);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal(transactionId, transaction.Id);
        Assert.Equal(employeeId, transaction.EmployeeId);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(typeId, transaction.TypeId);
        Assert.Equal(DateTime.UtcNow.Date, transaction.Date.Date);
    }

    [Fact]
    public async Task ShouldCreateTransaction()
    {
        // Arrange
        var transactionAmount = 5000;
        var transactionType = TypesForTransaction.Salary;
        var request = new TransactionVM
        {
            EmployeeId = _mainEmployee.Id,
            Amount = transactionAmount,
            Type = transactionType
        };

        // Act
        var response = await Client.PostAsJsonAsync("transactions/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var transactionFromResponse = await response.ToResponseModel<TransactionVM>();
        var transactionId = transactionFromResponse.Id;

        var facultyFromDataBase = await Context.Transactions
            .FirstOrDefaultAsync(x => x.Id == transactionId);

        facultyFromDataBase.Should().NotBeNull();

        facultyFromDataBase!.Amount.Should().Be(transactionAmount);
        facultyFromDataBase!.EmployeeId.Should().Be(_mainEmployee.Id);
        facultyFromDataBase!.TypeId.Should().Be(transactionType);
    }

    [Fact]
    public async Task ShouldShowAllTransaction()
    {
        // Arrange & Act
        var response = await Client.GetAsync($"transactions/get-all");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var transactionsFromResponse = await response.ToResponseModel<IReadOnlyList<TransactionDTO>>();

        transactionsFromResponse.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldShowAllTransactionByEmployee()
    {
        // Arrange & Act
        var response = await Client.GetAsync($"transactions/get-all-by-employee/{_mainEmployee.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var transactionsFromResponse = await response.ToResponseModel<IReadOnlyList<TransactionDTO>>();

        transactionsFromResponse.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldDeleteTransaction()
    {
        // Arrange
        var transactionIdToDelete = _mainTransaction.Id;

        // Act
        var response = await Client.DeleteAsync($"transactions/delete/{transactionIdToDelete}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldNotCreateTransactionBecauseEmployeeIsNotFound()
    {
        // Arrange
        var transactionAmount = 5000;
        var transactionType = TypesForTransaction.Salary;
        var request = new TransactionVM
        {
            EmployeeId = Guid.NewGuid(),
            Amount = transactionAmount,
            Type = transactionType
        };

        // Act
        var response = await Client.PostAsJsonAsync("transactions/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldLogTransactionInConsole()
    {
        // Arrange
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1000, "Bonus");
        var transactionLogger = new TransactionLogger();

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await transactionLogger.UpdateAsync(transaction, ActionsConstants.Add);

        // Assert
        var output = stringWriter.ToString();
        output.Should().Contain($"Transaction Add: {transaction.TypeId} - {transaction.Amount} on {transaction.Date}");
    }
    
    [Fact]
    public async Task ShouldLogTransactionInConsoleOnCreate()
    {
        // Arrange
        var transactionAmount = 5000;
        var transactionType = TypesForTransaction.Salary;
        var request = new TransactionVM
        {
            EmployeeId = _mainEmployee.Id,
            Amount = transactionAmount,
            Type = transactionType
        };

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        // Act
        var response = await Client.PostAsJsonAsync("transactions/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var transactionFromResponse = await response.ToResponseModel<TransactionVM>();
        
        var output = stringWriter.ToString();
        output.Should().Contain($"Transaction Add: {transactionFromResponse.Type} - {transactionFromResponse.Amount} on {transactionFromResponse.Date}");
    }
    
    [Fact]
    public async Task ShouldNotShowTransactionBecauseDateInputIsIncorrect()
    {
        // Arrange
        var startDate = DateTime.Now.AddMonths(-1);
        var endDate = DateTime.Now.AddYears(999);
        // Act
        var response = await Client.GetAsync($"transactions/get-get-total-amount-by-period?from={startDate}&to={endDate}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public async Task InitializeAsync()
    {
        await Context.Employees.AddAsync(_mainEmployee);
        await Context.Transactions.AddAsync(_mainTransaction);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Transactions.RemoveRange(Context.Transactions);
        Context.Employees.RemoveRange(Context.Employees);

        await SaveChangesAsync();
    }
}