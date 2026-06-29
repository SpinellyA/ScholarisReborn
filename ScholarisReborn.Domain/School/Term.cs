public class Term
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public int TermNumber { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public bool IsOpen => EndedAt is null;

    private Term() { }

    public static Term Create(Guid schoolId, int termNumber)
    {
        if (termNumber <= 0)
            throw new DomainException("Term number must be positive.");
        return new Term
        {
            Id = Guid.CreateVersion7(),
            SchoolId = schoolId,
            TermNumber = termNumber,
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
