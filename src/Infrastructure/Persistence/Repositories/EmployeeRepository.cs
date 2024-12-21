using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EmployeeRepository(ApplicationDbContext context) : IEmployeeRepository, IEmployeeQueries
{

    public async Task<IReadOnlyList<Employee>> GetAll()
    {
        return await context.Employees
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Employee> Get(Guid id)
    {
        return await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> Add(Employee employee)
    {
        await context.Employees.AddAsync(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> Delete(Guid id)
    {
        var employee = await context.Employees.FindAsync(id);
        if (employee != null)
        {
            context.Employees.Remove(employee);
            await context.SaveChangesAsync();
        }
        return employee;
    }

    public async Task<Employee> Update(Employee employee)
    {
        context.Employees.Update(employee);
        await context.SaveChangesAsync();
        return employee;
    }
}