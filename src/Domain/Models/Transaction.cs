namespace Domain.Models;

public class Transaction
{
    public Guid Id { get; }
    public Guid EmployeeId { get; private set; }
    public Employee? Employee { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TypeForTransaction Type { get; set; }
    public string TypeId { get; set; }

    public Transaction(Guid id, Guid employeeId, decimal amount, string typeId)
    {
        Id = id;
        EmployeeId = employeeId;
        Amount = amount;
        Date = DateTime.UtcNow;
        TypeId = typeId;
    }
}