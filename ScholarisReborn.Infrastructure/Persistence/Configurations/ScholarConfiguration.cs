using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ScholarConfiguration : IEntityTypeConfiguration<Scholar>
{
    public void Configure(EntityTypeBuilder<Scholar> builder)
    {
        builder.ToTable("Scholars");
        builder.HasKey(s => s.Id);

        builder.Metadata.FindNavigation(nameof(Scholar.Statuses))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(s => s.Statuses, sb =>
        {
            sb.ToTable("ScholarStatuses");
            sb.WithOwner().HasForeignKey("ScholarId");
            sb.Property<int>("Id").ValueGeneratedOnAdd();
            sb.HasKey("Id");

            sb.Property(s => s.Status).HasConversion<string>().HasMaxLength(32);
        });

        builder.Ignore(s => s.CurrentStatus);
        builder.Ignore(s => s.DomainEvents);
    }
}
