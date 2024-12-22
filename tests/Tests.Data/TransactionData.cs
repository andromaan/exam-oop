using Domain.Constants;
using Domain.Models;

namespace Tests.Data;

public static class TransactionData
{
    public static Transaction MeinTransaction(Guid employeeId)
        => new(Guid.NewGuid(), employeeId, 10000, TypesForTransaction.Bonus);
}