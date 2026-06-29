public record GradeTranscript
{
    public string TrueCopyOfGradesURL { get; private init; } = string.Empty;
    public double GWA { get; private init; }
    public List<CourseRecord> CourseRecords { get; private init; } = new();

    private GradeTranscript() { }

    public static GradeTranscript Create(string url, List<CourseRecord> records)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Grade transcript URL cannot be empty.");
        if (records is null || !records.Any())
            throw new DomainException("Grade transcript must have at least one course record.");

        double gwa = CalculateGWA(records);
        return new GradeTranscript
        {
            TrueCopyOfGradesURL = url,
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

