using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("Schools");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SchoolCode).IsRequired().HasMaxLength(32);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(256);
        builder.Property(s => s.Region).HasConversion<string>().HasMaxLength(32);
        builder.Property(s => s.TermSystem).HasConversion<string>().HasMaxLength(32);

        builder.Metadata.FindNavigation(nameof(School.Terms))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(s => s.Terms, tb =>
        {
            tb.ToTable("Terms");
            tb.WithOwner().HasForeignKey("SchoolId");
            tb.HasKey(t => t.Id);
        });

        builder.Ignore(s => s.DomainEvents);
    }
}
