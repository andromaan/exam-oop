﻿using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.ViewModels;
using Application.Implementation.PayrollManager;
using Domain.Constants;
using Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class DemonstrateWork
{
    public static async Task WorkDemo(this IApplicationBuilder builder)
    {
        using (var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            PayrollManager payrollManager = scope.ServiceProvider.GetRequiredService<PayrollManager>();
            var employeeQuery = scope.ServiceProvider.GetRequiredService<IEmployeeQueries>();
            
            if (payrollManager == null) throw new Exception("PayrollManager is not registered.");

            if ((await employeeQuery.GetAll())?.Count() == 0)
            {
                Random random = new Random();
                
                List<Employee> employees = new()
                {
                    await payrollManager.AddEmployeeAsync(new EmployeeVM
                    {
                        Name = "Alice Green",
                        Position = "Software Engineer",
                        Salary = 1200,
                    }),

                    await payrollManager.AddEmployeeAsync(new EmployeeVM
                    {
                        Name = "Bob Smith",
                        Position = "Project Manager",
                        Salary = 1500,
                    }),

                    await payrollManager.AddEmployeeAsync(new EmployeeVM
                    {
                        Name = "Charlie Brown",
                        Position = "Data Analyst",
                        Salary = 1100,
                    }),

                    await payrollManager.AddEmployeeAsync(new EmployeeVM
                    {
                        Name = "Diana White",
                        Position = "HR Specialist",
                        Salary = 950,
                    })
                };


                List<Transaction> transactions = new()
                {
                    await payrollManager.CreateTransactionAsync(new TransactionVM
                    {
                        EmployeeId = employees[random.Next(employees.Count())].Id,
                        Type = TypesForTransaction.Bonus,
                        Amount = random.Next(1000, 10000),
                    }),  
                    await payrollManager.CreateTransactionAsync(new TransactionVM
                    {
                        EmployeeId = employees[random.Next(employees.Count())].Id,
                        Type = TypesForTransaction.Salary,
                        Amount = random.Next(1000, 10000),

                    }),  
                    await payrollManager.CreateTransactionAsync(new TransactionVM
                    {
                        EmployeeId = employees[random.Next(employees.Count())].Id,
                        Type = TypesForTransaction.Fine,
                        Amount = random.Next(1000, 10000)
                    })
                };

                var employeesWithTransactions = employees
                    .Where(x => transactions.Any(y => y.EmployeeId == x.Id))
                    .ToList();
                
                var randomEmployee = employeesWithTransactions[random.Next(employeesWithTransactions.Count())];
                
                var transactionForEmployee = await payrollManager.GetTransactionsByEmployeeAsync(randomEmployee.Id);

                Console.WriteLine($"All transaction for user: {randomEmployee.Name}");
                foreach (var transaction in transactionForEmployee)
                {
                    Console.WriteLine($"{transaction.Date}: {transaction.TypeId} - {transaction.Amount} USD");
                }

                await payrollManager.DeleteTransactionAsync(transactions[random.Next(transactions.Count())].Id);
            }
        }
    }
}