public class Term
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }

    // Monotonic sequence within a school, used for ordering / "most recent term".
    public int TermNumber { get; private set; }

    // Academic period this term represents, e.g. AcademicYearStart=2025, PeriodNumber=2 =>
    // "2nd Semester, AY 2025-2026" (the period name depends on the school's TermSystem).
    public int AcademicYearStart { get; private set; }
    public int PeriodNumber { get; private set; }

    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public bool IsOpen => EndedAt is null;

    private Term() { }

    public static Term Create(Guid schoolId, int termNumber, int academicYearStart, int periodNumber)
    {
        if (termNumber <= 0)
            throw new DomainException("Term number must be positive.");
        if (academicYearStart <= 0)
            throw new DomainException("Academic year must be valid.");
        if (periodNumber <= 0)
            throw new DomainException("Period number must be positive.");

        return new Term
        {
            Id = Guid.CreateVersion7(),
            SchoolId = schoolId,
            TermNumber = termNumber,
            AcademicYearStart = academicYearStart,
            PeriodNumber = periodNumber,
            StartedAt = DateTime.UtcNow
        };
    }

    public void Close()
    {
        if (!IsOpen)
            throw new DomainException("Term is already closed.");
        EndedAt = DateTime.UtcNow;
    }
}
