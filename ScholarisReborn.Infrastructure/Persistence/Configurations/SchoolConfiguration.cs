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
        builder.Property(s => s.Logo).HasColumnType("bytea");
        builder.Property(s => s.LogoContentType).HasMaxLength(128);

        builder.Metadata.FindNavigation(nameof(School.Terms))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(s => s.Terms, tb =>
        {
            tb.ToTable("Terms");
            tb.WithOwner().HasForeignKey("SchoolId");

            // Use a shadow store-generated key (like every other owned collection here) rather than
            // Term.Id. EF marks a graph-discovered entity Added only when its key is the CLR default;
            // Term.Id is a client-set Guid (never default), so keying on it made EF treat a brand-new
            // Term as Modified -> UPDATE of a nonexistent row -> "0 rows affected". Term.Id stays a
            // normal, queryable Guid column (referenced by TermRecord.TermId and the UI).
            tb.Property<int>("TermKey").ValueGeneratedOnAdd();
            tb.HasKey("TermKey");
            tb.HasIndex(t => t.Id).IsUnique();
        });

        builder.Ignore(s => s.DomainEvents);
    }
}
