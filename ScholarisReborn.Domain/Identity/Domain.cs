
public class Student
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid SchoolId { get; private set; }

    private Student() { }

    public static Student Create(Guid userId, Guid schoolId)
    {
        if (userId == Guid.Empty) throw new DomainException("UserId cannot be empty.");
        if (schoolId == Guid.Empty) throw new DomainException("SchoolId cannot be empty.");
        return new Student { Id = Guid.CreateVersion7(), UserId = userId, SchoolId = schoolId };
    }
}

