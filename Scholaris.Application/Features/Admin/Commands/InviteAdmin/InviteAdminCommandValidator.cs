using FluentValidation;

public class InviteAdminCommandValidator : AbstractValidator<InviteAdminCommand>
{
    public InviteAdminCommandValidator()
    {
        RuleFor(c => c.invitedByAdminId).NotEmpty();
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
    }
}
