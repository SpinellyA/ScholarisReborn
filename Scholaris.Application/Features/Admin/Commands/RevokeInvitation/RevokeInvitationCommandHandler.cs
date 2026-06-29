public class RevokeInvitationCommandHandler : ICommandHandler<RevokeInvitationCommand>
{
    private readonly IUnitOfWork _uow;

    public RevokeInvitationCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(RevokeInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await _uow.InvitationRepository.GetById(command.InvitationId)
            ?? throw new DomainException("Invitation not found.");

        invitation.Revoke();

        _uow.InvitationRepository.Update(invitation);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
