public class CloseTermCommandHandler : ICommandHandler<CloseTermCommand>
{
    private readonly IUnitOfWork _uow;

    public CloseTermCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(CloseTermCommand command, CancellationToken cancellationToken)
    {
        var school = await _uow.SchoolRepository.GetById(command.SchoolId)
            ?? throw new DomainException("School not found.");

        school.CloseTerm(command.TermId);

        // Auto-withhold: any scholar at this school with no complete (transcript + proof of
        // registration submitted) record for the term being closed loses Active status.
        var scholars = await _uow.ScholarRepository.FindAsync(s => s.SchoolId == command.SchoolId, cancellationToken);
        var termRecords = await _uow.TermRepository.FindAsync(r => r.TermId == command.TermId, cancellationToken);

        var submittedScholarIds = termRecords
            .Where(r => r.GradeTranscript is not null && r.ProofOfRegistration is not null)
            .Select(r => r.ScholarId)
            .ToHashSet();

        foreach (var scholar in scholars)
        {
            if (scholar.CurrentStatus.Status == ScholasticStatus.Active && !submittedScholarIds.Contains(scholar.Id))
            {
                // Tracked entity (loaded via FindAsync) — no Update() call needed; see OpenTermCommandHandler.
                scholar.Withhold("No submission for term");
            }
        }

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
