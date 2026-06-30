public class OverrideScholarCommandHandler : ICommandHandler<OverrideScholarCommand>
{
    private readonly IUnitOfWork _uow;
    public OverrideScholarCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(OverrideScholarCommand command, CancellationToken cancellationToken)
    {
        var scholar = await _uow.ScholarRepository.GetById(command.ScholarId)
            ?? throw new DomainException("Scholar not found.");

        scholar.OverrideDetails(command.SchoolId, command.ScholarshipId, command.BatchNumber, command.DegreeProgram);
        scholar.ForceStatus(command.Status, command.StatusReason);

        var user = await _uow.UserRepository.GetById(scholar.UserId)
            ?? throw new DomainException("Scholar's user account not found.");

        user.SetProfile(UserProfile.Create(
            command.FirstName,
            command.LastName,
            command.DateOfBirth,
            command.Address,
            command.ContactNumber));

        await _uow.SaveChangesAsync(cancellationToken);
    }
}
