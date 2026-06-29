public class IsFirstRunQueryHandler : IQueryHandler<IsFirstRunQuery, bool>
{
    private readonly IIdentityService _identityService;

    public IsFirstRunQueryHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<bool> Handle(IsFirstRunQuery request, CancellationToken cancellationToken)
        => !await _identityService.AnyUsersExistAsync(cancellationToken);
}
