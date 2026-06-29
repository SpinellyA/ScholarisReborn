public record ProofOfRegistration
{
    public Guid FileId { get; private init; }
    public List<Course> Courses { get; private init; } = new();

    private ProofOfRegistration() { }

    public static ProofOfRegistration Create(Guid fileId, List<Course> courses)
    {
        if (fileId == Guid.Empty)
            throw new DomainException("Proof of registration file is required.");
        if (courses is null || !courses.Any())
            throw new DomainException("Proof of registration must have at least one course.");
        return new ProofOfRegistration { FileId = fileId, Courses = courses };
    }
}
