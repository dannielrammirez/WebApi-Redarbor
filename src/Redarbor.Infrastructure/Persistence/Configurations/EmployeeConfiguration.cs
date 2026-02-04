using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Redarbor.Domain.Entities;

namespace Redarbor.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CompanyId)
            .IsRequired();

        builder.Property(e => e.CreatedOn)
            .IsRequired();

        builder.Property(e => e.DeletedOn)
            .IsRequired(false);

        builder.Property(e => e.UpdatedOn)
            .IsRequired(false);

        builder.Property(e => e.LastLogin)
            .IsRequired(false);

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.Fax)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(e => e.Telephone)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(e => e.PortalId)
            .IsRequired();

        builder.Property(e => e.RoleId)
            .IsRequired();

        builder.Property(e => e.StatusId)
            .IsRequired();

        builder.OwnsOne(e => e.Email, email =>
        {
            email.Property(v => v.Value)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();

            email.HasIndex(v => v.Value)
                .IsUnique()
                .HasDatabaseName("IX_Employees_Email");
        });

        builder.OwnsOne(e => e.Username, username =>
        {
            username.Property(v => v.Value)
                .HasColumnName("Username")
                .HasMaxLength(50)
                .IsRequired();

            username.HasIndex(v => v.Value)
                .IsUnique()
                .HasDatabaseName("IX_Employees_Username");
        });

        builder.OwnsOne(e => e.Password, password =>
        {
            password.Property(v => v.Value)
                .HasColumnName("Password")
                .HasMaxLength(500)
                .IsRequired();
        });

        builder.HasIndex(e => e.CompanyId)
            .HasDatabaseName("IX_Employees_CompanyId");

        builder.HasIndex(e => e.StatusId)
            .HasDatabaseName("IX_Employees_StatusId");

        builder.HasIndex(e => e.DeletedOn)
            .HasDatabaseName("IX_Employees_DeletedOn");
    }
}
