using FluentValidation;

public class RegisterSuperAdminCommandValidator : AbstractValidator<RegisterSuperAdminCommand>
{
    public RegisterSuperAdminCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty();
    }
}
