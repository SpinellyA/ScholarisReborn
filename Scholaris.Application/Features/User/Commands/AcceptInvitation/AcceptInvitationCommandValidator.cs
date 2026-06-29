using FluentValidation;

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(c => c.Token).NotEmpty();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
    }
}
