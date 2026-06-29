public class School : AggregateRoot
{
    public Guid Id { get; private set; }
    public string SchoolCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Region Region { get; private set; }
    public TermSystem TermSystem { get; private set; }

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
             
            SchoolCode = schoolCode,
            Name = name,
            Description = description,
            Region = region,
            TermSystem = termSystem
        };
    }

    public Term OpenTerm(int termNumber)
    {
        if (_terms.Any(t => t.IsOpen))
            throw new DomainException("A term is already open for this school.");

        var term = Term.Create(Id, termNumber);
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
