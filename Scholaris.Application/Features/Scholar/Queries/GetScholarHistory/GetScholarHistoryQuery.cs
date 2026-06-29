public record ComplianceHistoryItem(int TermNumber, double? Gwa, RecordStatus Status, bool PorSubmitted, bool GradesSubmitted);
public record StatusHistoryItem(ScholasticStatus Status, DateTime StartTime, DateTime? EndTime, string? Reason);
public record StipendHistoryItem(double Amount, string Description, DateTime AnnouncedAt, bool? Received);

public record ScholarHistoryDto(
    List<ComplianceHistoryItem> Compliance,
    List<StatusHistoryItem> Statuses,
    List<StipendHistoryItem> Stipends);

public record GetScholarHistoryQuery(Guid ScholarUserId) : IQuery<ScholarHistoryDto>;
