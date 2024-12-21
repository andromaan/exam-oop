using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Position)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Salary)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.Employee)
            .HasForeignKey(t => t.EmployeeId);
    }
}