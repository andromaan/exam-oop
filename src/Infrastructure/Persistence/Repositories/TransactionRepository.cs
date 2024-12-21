using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository, ITransactionQueries
{
    public async Task<IReadOnlyList<Transaction>> GetAll()
    {
        return await context.Transactions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetAllForEmployee(Guid employeeId)
    {
        return await context.Transactions
            .Where(x=> x.EmployeeId == employeeId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Transaction> Get(Guid id)
    {
        return await context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Transaction> Delete(Guid id)
    {
        var transaction = await context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();
        }
        return transaction;
    }

    public async Task<Transaction> Update(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> Add(Transaction transaction)
    {
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }
}