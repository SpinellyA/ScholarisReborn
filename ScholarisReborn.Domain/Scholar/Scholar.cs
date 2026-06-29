public class Scholar : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid SchoolId { get; private set; }
    public Guid ScholarshipId { get; private set; }

    private List<ScholarStatus> _statuses = new();
    public IReadOnlyCollection<ScholarStatus> Statuses => _statuses.AsReadOnly();
    public ScholarStatus CurrentStatus => _statuses.Last();

    private Scholar() { }

    public static Scholar Create(Guid userId, Guid schoolId, Guid scholarshipId)
    {
        if (userId == Guid.Empty) throw new DomainException("UserId cannot be empty.");
        if (schoolId == Guid.Empty) throw new DomainException("SchoolId cannot be empty.");
        if (scholarshipId == Guid.Empty) throw new DomainException("ScholarshipId cannot be empty.");

        var scholar = new Scholar
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            SchoolId = schoolId,
            ScholarshipId = scholarshipId
        };

        var initialStatus = ScholarStatus.Create(ScholasticStatus.Active);
        scholar._statuses.Add(initialStatus);
        scholar.RaiseEvent(new ScholarActivatedEvent(scholar.Id, userId));
        return scholar;
    }

    public void Terminate(string reason)
    {
        EnsureNotIn(ScholasticStatus.Terminated, ScholasticStatus.Graduated);
        TransitionTo(ScholasticStatus.Terminated, reason);
        RaiseEvent(new ScholarTerminatedEvent(Id, reason));
    }

    public void Withhold(string reason)
    {
        EnsureNotIn(ScholasticStatus.Terminated, ScholasticStatus.Graduated, ScholasticStatus.Withheld);
        TransitionTo(ScholasticStatus.Withheld, reason);
        RaiseEvent(new ScholarWithheldEvent(Id, reason));
    }

    public void Activate()
    {
        EnsureNotIn(ScholasticStatus.Terminated, ScholasticStatus.Graduated, ScholasticStatus.Active);
        TransitionTo(ScholasticStatus.Active);
        RaiseEvent(new ScholarActivatedEvent(Id, UserId));
    }

    public void Graduate()
    {
        EnsureNotIn(ScholasticStatus.Terminated, ScholasticStatus.Graduated);
        TransitionTo(ScholasticStatus.Graduated);
        RaiseEvent(new ScholarGraduatedEvent(Id));
    }

    private void TransitionTo(ScholasticStatus status, string? reason = null)
    {
        var current = _statuses.LastOrDefault();
        if (current is not null)
            _statuses[_statuses.Count - 1] = current.Close();

        _statuses.Add(ScholarStatus.Create(status, reason));
    }

    private void EnsureNotIn(params ScholasticStatus[] invalidStatuses)
    {
        if (invalidStatuses.Contains(CurrentStatus.Status))
            throw new DomainException($"Scholar cannot transition from '{CurrentStatus.Status}'.");
    }
}
