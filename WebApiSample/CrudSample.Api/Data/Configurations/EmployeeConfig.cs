using CrudSample.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrudSample.Infrastructure.Data.Configurations;

internal class EmployeeConfig : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(80);

        builder.HasIndex(e => new { e.DepartmentId, e.Name }).IsUnique(false);
    }
}
