using Domain.Models;

namespace Application.Abstraction.ViewModels;

public record TransactionVM
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
}