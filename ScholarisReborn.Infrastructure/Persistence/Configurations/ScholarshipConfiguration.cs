using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ScholarshipConfiguration : IEntityTypeConfiguration<Scholarship>
{
    public void Configure(EntityTypeBuilder<Scholarship> builder)
    {
        builder.ToTable("Scholarships");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(256);

        builder.Metadata.FindNavigation(nameof(Scholarship.Grants))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(s => s.Grants, gb =>
        {
            gb.ToTable("ScholarshipGrants");
            gb.WithOwner().HasForeignKey("ScholarshipId");
            gb.Property<int>("Id").ValueGeneratedOnAdd();
            gb.HasKey("Id");
        });

        builder.Ignore(s => s.DomainEvents);
    }
}
