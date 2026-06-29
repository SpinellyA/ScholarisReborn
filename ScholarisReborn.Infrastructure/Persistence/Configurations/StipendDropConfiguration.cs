using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StipendDropConfiguration : IEntityTypeConfiguration<StipendDrop>
{
    public void Configure(EntityTypeBuilder<StipendDrop> builder)
    {
        builder.ToTable("StipendDrops");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Region).HasConversion<string>().HasMaxLength(32);

        builder.Metadata.FindNavigation(nameof(StipendDrop.Receipts))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(d => d.Receipts, rb =>
        {
            rb.ToTable("StipendReceipts");
            rb.WithOwner().HasForeignKey("StipendDropId");
            rb.Property<int>("Id").ValueGeneratedOnAdd();
            rb.HasKey("Id");
        });

        builder.Ignore(d => d.DomainEvents);
    }
}
