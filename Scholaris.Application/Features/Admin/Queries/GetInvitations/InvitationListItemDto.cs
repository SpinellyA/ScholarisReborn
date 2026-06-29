public record InvitationListItemDto(
    Guid Id,
    string Email,
    InvitationType Type,
    InvitationStatus Status,
    Guid InvitedByAdminId,
    string? InvitedByEmail,
    string? SchoolName,
    string? ScholarshipName,
    DateTime CreatedAt,
    DateTime ExpiresAt);
