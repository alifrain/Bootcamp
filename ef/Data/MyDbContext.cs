using Microsoft.EntityFrameworkCore;
using ef.Models;   // <— keep this

namespace ef.Data;

public class MyDbContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();   // <— use simple name

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "MyDatabase.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>()
            .HasMany(d => d.Employees)
            .WithOne(e => e.Department!)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Projects)
            .WithMany(p => p.Employees)
            .UsingEntity(j => j.ToTable("EmployeeProjects"));
            
        // --- SEEDING ---
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Sales" },
            new Department { Id = 2, Name = "IT" }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, Name = "Jotaro Kujo", DepartmentId = 1 },
            new Employee { Id = 2, Name = "Polnareff", DepartmentId = 1 }
        );

        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Title = "CRM Revamp" }
        );    
    }
}
