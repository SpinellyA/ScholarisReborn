public class Scholarship : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private List<FinancialGrant> _grants = new();
    public IReadOnlyCollection<FinancialGrant> Grants => _grants.AsReadOnly();

    private Scholarship() { }

    public static Scholarship Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Scholarship name cannot be empty.");

        return new Scholarship { Id = Guid.CreateVersion7(), Name = name, Description = description };
    }

    public void AddGrant(FinancialGrant grant)
    {
        if (_grants.Any(g => g.Name == grant.Name))
            throw new DomainException($"A grant named '{grant.Name}' already exists.");
        _grants.Add(grant);
    }
}
