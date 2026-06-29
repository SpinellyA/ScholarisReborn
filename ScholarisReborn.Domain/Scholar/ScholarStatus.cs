
public record ScholarStatus(
    ScholasticStatus Status,
    DateTime StartTime,
    DateTime? EndTime = null,
    string? Reason = null)
{
    public static ScholarStatus Create(ScholasticStatus status, string? reason = null, DateTime? startTime = null)
        => new(status, startTime ?? DateTime.UtcNow, null, reason);

    public ScholarStatus Close(DateTime? endTime = null)
        => this with { EndTime = endTime ?? DateTime.UtcNow };
}

