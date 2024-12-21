using Domain.Models;

namespace Application.Abstraction.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAll();
    Task<Employee> Get(Guid id);
    Task<Employee> Add(Employee employee);
    Task<Employee> Delete(Guid id);
    Task<Employee> Update(Employee employee);
}