public record ProofOfRegistration
{
    public string ProofOfRegistrationURL { get; private init; } = string.Empty;
    public List<Course> Courses { get; private init; } = new();

    private ProofOfRegistration() { }

    public static ProofOfRegistration Create(string url, List<Course> courses)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Proof of registration URL cannot be empty.");
        if (courses is null || !courses.Any())
            throw new DomainException("Proof of registration must have at least one course.");
        return new ProofOfRegistration { ProofOfRegistrationURL = url, Courses = courses };
    }
}

