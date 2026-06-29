using System;
using System.Collections.Generic;
using System.Text;


public class InviteScholarCommandHandler : ICommandHandler<InviteScholarCommand>
{
    private readonly IUnitOfWork _uow;

    public InviteScholarCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(InviteScholarCommand command, CancellationToken cancellationToken)
    {
        var invitation = Invitation.Create(command.Email, InvitationType.Scholar, command.invitedByAdminId, command.schoolId, command.scholarshipId);

        _uow.InvitationRepository.Add(invitation);
        await _uow.SaveChangesAsync();
    }
}