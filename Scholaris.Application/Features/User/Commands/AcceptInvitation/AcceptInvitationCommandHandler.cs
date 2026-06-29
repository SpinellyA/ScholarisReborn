public class AcceptInvitationCommandHandler : ICommandHandler<AcceptInvitationCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly IIdentityService _identityService;

    public AcceptInvitationCommandHandler(IUnitOfWork uow, IIdentityService identityService)
    {
        _uow = uow;
        _identityService = identityService;
    }

    public async Task Handle(AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        var matches = await _uow.InvitationRepository.FindAsync(i => i.Token == command.Token, cancellationToken);
        var invitation = matches.SingleOrDefault()
            ?? throw new DomainException("Invitation not found.");

        invitation.MarkAsUsed();

        var firstName = invitation.SeedData?.FirstName ?? command.FirstName ?? string.Empty;
        var lastName = invitation.SeedData?.LastName ?? command.LastName ?? string.Empty;
        var dateOfBirth = invitation.SeedData?.DateOfBirth ?? command.DateOfBirth;
        var address = invitation.SeedData?.Address ?? command.Address;
        var contactNumber = invitation.SeedData?.ContactNumber ?? command.ContactNumber;

        var userId = Guid.CreateVersion7();

        await using var transaction = await _uow.BeginTransactionAsync(cancellationToken: cancellationToken);

        var createResult = await _identityService.CreateUserAsync(userId, invitation.Email, command.Password, cancellationToken);
        if (!createResult.Succeeded)
            throw new DomainException(string.Join(" ", createResult.Errors));

        var role = invitation.Type == InvitationType.Scholar ? ApplicationRoles.Scholar : ApplicationRoles.Admin;
        var roleResult = await _identityService.AddToRoleAsync(userId, role, cancellationToken);
        if (!roleResult.Succeeded)
            throw new DomainException(string.Join(" ", roleResult.Errors));

        var profile = UserProfile.Create(firstName, lastName, dateOfBirth, address, contactNumber);
        var user = User.CreateWithId(userId, invitation.Email, profile);
        _uow.UserRepository.Add(user);

        if (invitation.Type == InvitationType.Scholar)
        {
            var scholar = Scholar.Create(
                userId,
                invitation.SchoolId!.Value,
                invitation.ScholarshipId!.Value,
                invitation.BatchNumber!.Value,
                invitation.DegreeProgram!);
            _uow.ScholarRepository.Add(scholar);
        }

        _uow.InvitationRepository.Update(invitation);

        await _uow.SaveChangesAsync(cancellationToken);
        await _uow.CommitAsync(cancellationToken);
    }
}
