using Corvel.ToDo.Common.Constants;
using Corvel.ToDo.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corvel.ToDo.Repository.Configurations;

public class ToDoItemEntityConfiguration : IEntityTypeConfiguration<ToDoItemEntity>
{
    public void Configure(EntityTypeBuilder<ToDoItemEntity> builder)
    {
        builder.ToTable("ToDoItems");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(ValidationConstants.TitleMaxLength);

        builder.Property(e => e.Description)
            .HasMaxLength(ValidationConstants.DescriptionMaxLength);

        builder.Property(e => e.Priority)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(e => e.Status);
    }
}
