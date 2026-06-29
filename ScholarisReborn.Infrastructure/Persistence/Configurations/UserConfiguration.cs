using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).HasMaxLength(512);

        builder.Metadata.FindNavigation(nameof(User.Statuses))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(u => u.Statuses, sb =>
        {
            sb.ToTable("UserStatuses");
            sb.WithOwner().HasForeignKey("UserId");
            sb.Property<int>("Id").ValueGeneratedOnAdd();
            sb.HasKey("Id");

            sb.Property(s => s.Status).HasConversion<string>().HasMaxLength(32);
        });

        builder.Ignore(u => u.DomainEvents);
    }
}
