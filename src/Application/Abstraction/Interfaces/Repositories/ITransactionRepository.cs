using Domain.Models;

namespace Application.Abstraction.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> GetAll();
    Task<IReadOnlyList<Transaction>> GetAllForEmployee(Guid employeeId);
    Task<Transaction> Get(Guid id);
    Task<Transaction> Delete(Guid id);
    Task<Transaction> Update(Transaction employee);
    Task<Transaction> Add(Transaction transaction);
}