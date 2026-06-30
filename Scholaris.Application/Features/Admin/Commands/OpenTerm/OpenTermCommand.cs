using MediatR;

public record OpenTermCommand(Guid SchoolId, int AcademicYearStart, int PeriodNumber) : ICommand;
