using System.Net.Http.Json;
using Application.Abstraction.ViewModels;
using Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class EmployeesControllerTest(IntegrationTestWebFactory factory)
    : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Employee _mainEmployee = EmployeeData.MainEmployee();

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
        var response = await Client.PostAsJsonAsync("employee/add", request);

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

    public async Task InitializeAsync()
    {
        await Context.Employees.AddAsync(_mainEmployee);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Employees.RemoveRange(Context.Employees);

        await SaveChangesAsync();
    }
}