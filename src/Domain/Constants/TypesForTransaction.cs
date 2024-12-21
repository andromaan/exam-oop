namespace Domain.Constants;

public static class TypesForTransaction
{
    public const string Salary = "Salary";
    public const string Bonus = "Bonus";
    public const string Fine = "Fine";
    
    public static readonly List<string> All = new() { Salary, Bonus, Fine };
}