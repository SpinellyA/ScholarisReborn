public class InviteScholarCommandHandler : ICommandHandler<InviteScholarCommand>
{
    private readonly IUnitOfWork _uow;

    public InviteScholarCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(InviteScholarCommand command, CancellationToken cancellationToken)
    {
        var seedData = BuildSeedData(command);

        var invitation = Invitation.Create(
            command.Email,
            InvitationType.Scholar,
            command.invitedByAdminId,
            command.schoolId,
            command.scholarshipId,
            seedData);

        _uow.InvitationRepository.Add(invitation);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    private static InvitationSeedData? BuildSeedData(InviteScholarCommand command)
    {
        if (command.FirstName is null && command.LastName is null && command.DateOfBirth is null
            && command.Address is null && command.ContactNumber is null)
            return null;

        return InvitationSeedData.Create(command.FirstName, command.LastName, command.DateOfBirth, command.Address, command.ContactNumber);
    }
}
