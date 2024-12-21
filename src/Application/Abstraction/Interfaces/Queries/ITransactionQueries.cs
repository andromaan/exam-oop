using Domain.Models;

namespace Application.Abstraction.Interfaces.Queries;

public interface ITransactionQueries
{
    Task<IReadOnlyList<Transaction>> GetAll();
    Task<IReadOnlyList<Transaction>> GetAllForEmployee(Guid employeeId);
    Task<Transaction> Get(Guid id);
}