using CrudSample.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrudSample.Api.Data.Configurations;

public class AuthUserConfig : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> b)
    {
        b.ToTable("AuthUsers");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserName).IsRequired().HasMaxLength(100);
        b.Property(x => x.PasswordHash).IsRequired();
        b.Property(x => x.Role).IsRequired().HasMaxLength(30);
        b.HasIndex(x => x.UserName).IsUnique();
    }
}
