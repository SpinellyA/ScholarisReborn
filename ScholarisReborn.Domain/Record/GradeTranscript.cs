public record GradeTranscript
{
    // Nullable: a first-year scholar with no prior-term courses submits grades-less (file optional).
    public Guid? FileId { get; private init; }
    public double GWA { get; private init; }
    public List<CourseRecord> CourseRecords { get; private init; } = new();

    private GradeTranscript() { }

    public static GradeTranscript Create(Guid? fileId, List<CourseRecord> records)
    {
        if (records is null || !records.Any())
            throw new DomainException("Grade transcript must have at least one course record.");

        double gwa = CalculateGWA(records);
        return new GradeTranscript
        {
            FileId = fileId,
            CourseRecords = records,
            GWA = gwa
        };
    }

    private static double CalculateGWA(List<CourseRecord> records)
    {
        double totalWeighted = records.Sum(r => r.Grade * r.Course.Units);
        double totalUnits = records.Sum(r => r.Course.Units);
        return totalUnits > 0 ? totalWeighted / totalUnits : 0;
    }
}
