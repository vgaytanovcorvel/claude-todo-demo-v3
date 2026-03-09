using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corvel.ToDo.Repository.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(ValidationConstants.EmailMaxLength);

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NameMaxLength);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NameMaxLength);

        builder.Property(e => e.PasswordHash)
            .IsRequired();

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();
    }
}
