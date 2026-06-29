using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("StoredFiles");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName).IsRequired().HasMaxLength(512);
        builder.Property(f => f.ContentType).IsRequired().HasMaxLength(128);
        builder.Property(f => f.Content).HasColumnType("bytea");

        builder.Ignore(f => f.DomainEvents);
    }
}
