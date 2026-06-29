using FluentValidation;

public class ApproveTermRecordCommandValidator : AbstractValidator<ApproveTermRecordCommand>
{
    public ApproveTermRecordCommandValidator()
    {
        RuleFor(c => c.RecordId).NotEmpty();
        RuleFor(c => c.AdminUserId).NotEmpty();
    }
}

public class DeferTermRecordCommandValidator : AbstractValidator<DeferTermRecordCommand>
{
    public DeferTermRecordCommandValidator()
    {
        RuleFor(c => c.RecordId).NotEmpty();
        RuleFor(c => c.AdminUserId).NotEmpty();
        RuleFor(c => c.Reason).NotEmpty().WithMessage("A reason is required when deferring a record.");
    }
}

public class DenyTermRecordCommandValidator : AbstractValidator<DenyTermRecordCommand>
{
    public DenyTermRecordCommandValidator()
    {
        RuleFor(c => c.RecordId).NotEmpty();
        RuleFor(c => c.AdminUserId).NotEmpty();
        RuleFor(c => c.Reason).NotEmpty().WithMessage("A reason is required when denying a record.");
    }
}
