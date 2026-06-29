using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TermRecordConfiguration : IEntityTypeConfiguration<TermRecord>
{
    public void Configure(EntityTypeBuilder<TermRecord> builder)
    {
        builder.ToTable("TermRecords");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(32);

        builder.OwnsOne(r => r.GradeTranscript, gb =>
        {
            gb.ToTable("TermRecordGradeTranscripts");
            gb.WithOwner().HasForeignKey("TermRecordId");
            gb.Property<int>("Id").ValueGeneratedOnAdd();
            gb.HasKey("Id");

            gb.OwnsMany(g => g.CourseRecords, crb =>
            {
                crb.ToTable("TermRecordCourseRecords");
                crb.WithOwner().HasForeignKey("GradeTranscriptId");
                crb.Property<int>("Id").ValueGeneratedOnAdd();
                crb.HasKey("Id");

                crb.OwnsOne(cr => cr.Course, cb =>
                {
                    cb.ToTable("TermRecordCourseRecordCourses");
                    cb.WithOwner().HasForeignKey("CourseRecordId");
                    cb.Property<int>("Id").ValueGeneratedOnAdd();
                    cb.HasKey("Id");
                });
            });
        });

        builder.OwnsOne(r => r.ProofOfRegistration, pb =>
        {
            pb.ToTable("TermRecordProofOfRegistrations");
            pb.WithOwner().HasForeignKey("TermRecordId");
            pb.Property<int>("Id").ValueGeneratedOnAdd();
            pb.HasKey("Id");

            pb.OwnsMany(p => p.Courses, cb =>
            {
                cb.ToTable("TermRecordProofOfRegistrationCourses");
                cb.WithOwner().HasForeignKey("ProofOfRegistrationId");
                cb.Property<int>("Id").ValueGeneratedOnAdd();
                cb.HasKey("Id");
            });
        });

        builder.Ignore(r => r.DomainEvents);
    }
}
