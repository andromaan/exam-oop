namespace Domain.Models;

public class TypeForTransaction
{
    public string Id { get; private set; }
    public string Name { get; private set; }

    public TypeForTransaction(string name)
    {
        Name = name;
        Id = name;
    }
}