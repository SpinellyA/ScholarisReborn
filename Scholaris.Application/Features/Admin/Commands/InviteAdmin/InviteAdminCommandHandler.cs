public class InviteAdminCommandHandler : ICommandHandler<InviteAdminCommand>
{
    private readonly IUnitOfWork _uow;

    public InviteAdminCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task Handle(InviteAdminCommand command, CancellationToken cancellationToken)
    {
        var invitation = Invitation.Create(command.Email, InvitationType.Admin, command.invitedByAdminId);

        _uow.InvitationRepository.Add(invitation);
        await _uow.SaveChangesAsync();
    }
}


