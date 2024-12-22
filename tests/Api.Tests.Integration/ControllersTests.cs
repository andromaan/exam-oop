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

public class ControllersTests
    : BaseIntegrationTest, IAsyncLifetime
{
        private readonly Transaction _mainTransaction;
    private readonly Employee _mainEmployee = EmployeeData.MainEmployee();

    public ControllersTests(IntegrationTestWebFactory factory) : base(factory)
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
    
    [Fact]
    public async Task ShouldCreateEmployeeClass()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employeeName = "From Test Employee";
        var employeePosition = "From Test Position";
        var employeeSalary = 5000;

        // Act
        var employee = new Employee(employeeId, employeeName, employeePosition, employeeSalary);

        // Assert
        Assert.NotNull(employee);
        Assert.Equal(employeeId, employee.Id);
        Assert.Equal(employeeName, employee.Name);
        Assert.Equal(employeePosition, employee.Position);
        Assert.Equal(employeeSalary, employee.Salary);
    }


    [Fact]
    public async Task ShouldCreateEmployee()
    {
        // Arrange
        var employeeName = "From Test Employee";
        var employeePosition = "From Test Position";
        var employeeSalary = 5000;
        var request = new EmployeeVM
        {
            Name = employeeName,
            Position = employeePosition,
            Salary = employeeSalary
        };

        // Act
        var response = await Client.PostAsJsonAsync("employees/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var employeeFromResponse = await response.ToResponseModel<EmployeeVM>();
        var employeeId = employeeFromResponse.Id;

        var employeeFromDataBase = await Context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId);
        employeeFromDataBase.Should().NotBeNull();

        employeeFromDataBase!.Name.Should().Be(employeeName);
        employeeFromDataBase!.Position.Should().Be(employeePosition);
        employeeFromDataBase!.Salary.Should().Be(employeeSalary);
    }

    [Fact]
    public async Task ShouldDeleteEmployee()
    {
        // Arrange
        var mainEmployeeId = _mainEmployee.Id;

        // Act
        var response = await Client.DeleteAsync($"employees/delete/{mainEmployeeId}");
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
    
    [Fact]
    public async Task ShouldNotDeleteEmployeeBecauseEmployeeIsNotFound()
    {
        // Arrange
        var randomEmployeeId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"employees/delete/{randomEmployeeId}");
        
        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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