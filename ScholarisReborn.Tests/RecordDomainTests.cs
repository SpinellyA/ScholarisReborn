public class RecordDomainTests
{
    private static ProofOfRegistration SamplePor()
        => ProofOfRegistration.Create(Guid.NewGuid(), [Course.Create("CS101", 3)]);

    private static GradeTranscript SampleTranscript()
        => GradeTranscript.Create(Guid.NewGuid(), [CourseRecord.Create(Course.Create("CS101", 3), 1.5)]);

    [Fact]
    public void FirstYearRecord_IsApprovable_WithProofOfRegistrationOnly()
    {
        var record = TermRecord.Create(Guid.NewGuid(), Guid.NewGuid(), gradesRequired: false);
        record.SubmitPOR(SamplePor());

        record.Approve(Guid.NewGuid());

        Assert.Equal(RecordStatus.Approved, record.Status);
    }

    [Fact]
    public void ReturningScholarRecord_CannotBeApproved_WithoutGradeTranscript()
    {
        var record = TermRecord.Create(Guid.NewGuid(), Guid.NewGuid(), gradesRequired: true);
        record.SubmitPOR(SamplePor());

        Assert.Throws<DomainException>(() => record.Approve(Guid.NewGuid()));
    }

    [Fact]
    public void ReturningScholarRecord_IsApprovable_WithBothSubmitted()
    {
        var record = TermRecord.Create(Guid.NewGuid(), Guid.NewGuid(), gradesRequired: true);
        record.SubmitPOR(SamplePor());
        record.SubmitTranscript(SampleTranscript());

        record.Approve(Guid.NewGuid());

        Assert.Equal(RecordStatus.Approved, record.Status);
    }

    [Fact]
    public void GradeTranscript_ComputesUnitWeightedGwa()
    {
        var records = new List<CourseRecord>
        {
            CourseRecord.Create(Course.Create("A", 3), 1.0),
            CourseRecord.Create(Course.Create("B", 1), 2.0)
        };

        var transcript = GradeTranscript.Create(Guid.NewGuid(), records);

        // (1.0*3 + 2.0*1) / (3+1) = 5/4 = 1.25
        Assert.Equal(1.25, transcript.GWA, precision: 3);
    }

    [Fact]
    public void GradeTranscript_AllowsNullFileId()
    {
        var transcript = GradeTranscript.Create(null, [CourseRecord.Create(Course.Create("A", 3), 1.0)]);
        Assert.Null(transcript.FileId);
    }

    [Fact]
    public void Scholar_Create_RequiresPositiveBatchAndNonEmptyProgram()
    {
        Assert.Throws<DomainException>(() => Scholar.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, "BS CS"));
        Assert.Throws<DomainException>(() => Scholar.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2024, ""));
    }

    [Fact]
    public void StoredFile_Create_RequiresContent()
    {
        Assert.Throws<DomainException>(() => StoredFile.Create("a.pdf", "application/pdf", Array.Empty<byte>(), Guid.NewGuid()));

        var file = StoredFile.Create("a.pdf", "application/pdf", [1, 2, 3], Guid.NewGuid());
        Assert.Equal("a.pdf", file.FileName);
    }
}
