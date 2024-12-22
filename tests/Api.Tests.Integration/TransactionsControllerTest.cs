using System.Net.Http.Json;
using Application.Abstraction.ViewModels;
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
        var response = await Client.PostAsJsonAsync("transaction/add", request);
        
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