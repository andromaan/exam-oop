using Domain.Models;

namespace Api.DTOs;

public record EmployeeDTO(Guid Id, string Name, string Position, decimal Salary)
{
    public static EmployeeDTO FromDomainModel(Employee employee)
        => new(employee.Id, employee.Name, employee.Position, employee.Salary);
}