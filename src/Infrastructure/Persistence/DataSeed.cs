using Domain.Constants;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DataSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        _seedTypesForTransaction(modelBuilder);
    }

    private static void _seedTypesForTransaction(ModelBuilder modelBuilder)
    {
        var statuses = new List<TypeForTransaction>();

        foreach (var status in TypesForTransaction.All)
        {
            statuses.Add(new TypeForTransaction(status));
        }

        modelBuilder.Entity<TypeForTransaction>()
            .HasData(statuses);
    }
}