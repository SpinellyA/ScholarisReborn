using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("Invitations");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Email).IsRequired().HasMaxLength(256);
        builder.Property(i => i.Type).HasConversion<string>().HasMaxLength(32);

        builder.Ignore(i => i.DomainEvents);
    }
}
