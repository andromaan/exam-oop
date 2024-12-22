using Domain.Models;

namespace Tests.Data;

public static class EmployeeData
{
    public static Employee MainEmployee()
        => new(Guid.NewGuid(), "Some name", "Some position", 5000m);
}