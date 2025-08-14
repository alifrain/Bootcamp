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

        builder
            .HasMany(e => e.Projects)
            .WithMany(p => p.Members)
            .UsingEntity<Dictionary<string, object>>(
                "EmployeeProjects",
                j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("EmployeeId", "ProjectId");
                    j.ToTable("EmployeeProjects");
                });
    }
}
