using Domain.Models;

namespace Application.Abstraction.ViewModels;

public class EmployeeVM
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public decimal Salary { get; set; }
}