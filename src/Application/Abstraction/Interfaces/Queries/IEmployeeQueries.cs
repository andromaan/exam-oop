using Domain.Models;

namespace Application.Abstraction.Interfaces.Queries;

public interface IEmployeeQueries
{
    Task<IReadOnlyList<Employee>> GetAll();
    Task<Employee> Get(Guid id);
}