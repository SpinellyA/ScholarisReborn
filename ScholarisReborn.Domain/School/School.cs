public class School : AggregateRoot
{
    public Guid Id { get; private set; }
    public string SchoolCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Region Region { get; private set; }
    public TermSystem TermSystem { get; private set; }

    public byte[]? Logo { get; private set; }
    public string? LogoContentType { get; private set; }

    private List<Term> _terms = new();
    public IReadOnlyCollection<Term> Terms => _terms.AsReadOnly();

    private School() { }

    public static School Create(string schoolCode, string name, string description, Region region, TermSystem termSystem)
    {
        if (string.IsNullOrWhiteSpace(schoolCode))
            throw new DomainException("School code cannot be empty.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("School name cannot be empty.");

        return new School
        {
            Id = Guid.CreateVersion7(),
            SchoolCode = schoolCode,
            Name = name,
            Description = description,
            Region = region,
            TermSystem = termSystem
        };
    }

    public void UpdateDetails(string schoolCode, string name, string description, Region region, TermSystem termSystem)
    {
        if (string.IsNullOrWhiteSpace(schoolCode))
            throw new DomainException("School code cannot be empty.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("School name cannot be empty.");

        SchoolCode = schoolCode;
        Name = name;
        Description = description;
        Region = region;
        TermSystem = termSystem;
    }

    public void SetLogo(byte[] content, string contentType)
    {
        if (content is null || content.Length == 0)
            throw new DomainException("Logo content cannot be empty.");
        Logo = content;
        LogoContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
    }

    public void RemoveLogo()
    {
        Logo = null;
        LogoContentType = null;
    }

    // The academic period that should come after the latest term, based on the term system
    // (e.g. semestral has 2 periods/year, so 2nd Semester AY X -> 1st Semester AY X+1).
    public (int AcademicYearStart, int PeriodNumber) SuggestNextTerm()
    {
        var latest = _terms.OrderByDescending(t => t.TermNumber).FirstOrDefault();
        if (latest is null)
            return (DefaultAcademicYearStart(), 1);

        var periodsPerYear = TermSystemInfo.PeriodsPerYear(TermSystem);
        return latest.PeriodNumber < periodsPerYear
            ? (latest.AcademicYearStart, latest.PeriodNumber + 1)
            : (latest.AcademicYearStart + 1, 1);
    }

    private static int DefaultAcademicYearStart()
    {
        var now = DateTime.UtcNow;
        // Philippine school years typically start mid-year; treat Jan–May as the previous AY.
        return now.Month >= 6 ? now.Year : now.Year - 1;
    }

    public Term OpenTerm(int academicYearStart, int periodNumber)
    {
        if (_terms.Any(t => t.IsOpen))
            throw new DomainException("A term is already open for this school.");

        var periodsPerYear = TermSystemInfo.PeriodsPerYear(TermSystem);
        if (periodNumber < 1 || periodNumber > periodsPerYear)
            throw new DomainException($"A {TermSystem} school has {periodsPerYear} period(s) per year; period {periodNumber} is invalid.");

        if (_terms.Any(t => t.AcademicYearStart == academicYearStart && t.PeriodNumber == periodNumber))
            throw new DomainException($"{TermSystemInfo.Label(TermSystem, academicYearStart, periodNumber)} has already been opened.");

        var termNumber = (_terms.Select(t => t.TermNumber).DefaultIfEmpty(0).Max()) + 1;
        var term = Term.Create(Id, termNumber, academicYearStart, periodNumber);
        _terms.Add(term);
        RaiseEvent(new TermOpenedEvent(term.Id, Id, termNumber));
        return term;
    }

    public void CloseTerm(Guid termId)
    {
        var term = _terms.FirstOrDefault(t => t.Id == termId)
            ?? throw new DomainException("Term not found.");
        term.Close();
        RaiseEvent(new TermClosedEvent(termId, Id));
    }
}
