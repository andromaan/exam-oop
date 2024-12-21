namespace Domain.Models;

public class Employee
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Position { get; private set; }
    public decimal Salary { get; private set; }
    public List<Transaction> Transactions { get; private set;  } = new();

    public Employee(Guid id, string name, string position, decimal salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}
