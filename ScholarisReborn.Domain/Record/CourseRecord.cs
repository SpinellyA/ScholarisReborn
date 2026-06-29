public record CourseRecord(Course Course, double Grade)
{
    public static CourseRecord Create(Course course, double grade)
    {
        if (course is null)
            throw new DomainException("Course cannot be null.");
        if (grade < 0)
            throw new DomainException("Grade cannot be negative.");
        return new CourseRecord(course, grade);
    }
}

