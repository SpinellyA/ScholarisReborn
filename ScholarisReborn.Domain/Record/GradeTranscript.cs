public record GradeTranscript
{
    public string TrueCopyOfGradesURL { get; }
    public double GWA { get; }
    public IReadOnlyCollection<CourseRecord> CourseRecords { get; }

    private GradeTranscript(string url, List<CourseRecord> records, double gwa)
    {
        TrueCopyOfGradesURL = url;
        CourseRecords = records.AsReadOnly();
        GWA = gwa;
    }

    public static GradeTranscript Create(string url, List<CourseRecord> records)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Grade transcript URL cannot be empty.");
        if (records is null || !records.Any())
            throw new DomainException("Grade transcript must have at least one course record.");

        double gwa = CalculateGWA(records);
        return new GradeTranscript(url, records, gwa);
    }

    private static double CalculateGWA(List<CourseRecord> records)
    {
        double totalWeighted = records.Sum(r => r.Grade * r.Course.Units);
        double totalUnits = records.Sum(r => r.Course.Units);
        return totalUnits > 0 ? totalWeighted / totalUnits : 0;
    }
}

