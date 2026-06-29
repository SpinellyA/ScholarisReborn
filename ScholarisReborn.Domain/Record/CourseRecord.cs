public record CourseRecord
{
    public Course Course { get; private init; } = null!;
    public double Grade { get; private init; }

    private CourseRecord() { }

    public static CourseRecord Create(Course course, double grade)
    {
        if (course is null)
            throw new DomainException("Course cannot be null.");
        if (grade < 0)
            throw new DomainException("Grade cannot be negative.");
        return new CourseRecord { Course = course, Grade = grade };
    }
}

