public class OpenTermCommandHandler : ICommandHandler<OpenTermCommand>
{
    private readonly IUnitOfWork _uow;

    public OpenTermCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(OpenTermCommand command, CancellationToken cancellationToken)
    {
        var school = await _uow.SchoolRepository.GetById(command.SchoolId)
            ?? throw new DomainException("School not found.");

        school.OpenTerm(command.AcademicYearStart, command.PeriodNumber);

        // No Update() call: 'school' is tracked (loaded via GetById), so EF detects the new Term as
        // an insert. Calling Update() would force the whole graph to Modified and try to UPDATE the
        // brand-new Term row (client-assigned Guid key) that doesn't exist yet -> 0 rows -> throws.
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
