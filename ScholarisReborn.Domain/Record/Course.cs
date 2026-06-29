public record Course(string CourseCode, double Units)
{
    public static Course Create(string courseCode, double units)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
            throw new DomainException("Course code cannot be empty.");
        if (units <= 0)
            throw new DomainException("Units must be greater than zero.");
        return new Course(courseCode, units);
    }
}

