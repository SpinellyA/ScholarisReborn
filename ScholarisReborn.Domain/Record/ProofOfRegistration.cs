public record ProofOfRegistration
{
    public string ProofOfRegistrationURL { get; }
    public IReadOnlyCollection<Course> Courses { get; }

    private ProofOfRegistration(string url, List<Course> courses)
    {
        ProofOfRegistrationURL = url;
        Courses = courses.AsReadOnly();
    }

    public static ProofOfRegistration Create(string url, List<Course> courses)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Proof of registration URL cannot be empty.");
        if (courses is null || !courses.Any())
            throw new DomainException("Proof of registration must have at least one course.");
        return new ProofOfRegistration(url, courses);
    }
}

