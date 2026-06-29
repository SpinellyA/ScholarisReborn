using FluentValidation;

public class AnnounceStipendDropCommandValidator : AbstractValidator<AnnounceStipendDropCommand>
{
    public AnnounceStipendDropCommandValidator()
    {
        RuleFor(c => c.Amount).GreaterThanOrEqualTo(0);
        RuleFor(c => c.Description).NotEmpty().MaximumLength(512);
    }
}
