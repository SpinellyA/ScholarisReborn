using Microsoft.EntityFrameworkCore;

public class GetTermRecordDetailQueryHandler : IQueryHandler<GetTermRecordDetailQuery, TermRecordDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetTermRecordDetailQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<TermRecordDetailDto?> Handle(GetTermRecordDetailQuery request, CancellationToken cancellationToken)
    {
        var record = await _context.TermRecords.AsNoTracking().FirstOrDefaultAsync(r => r.Id == request.RecordId, cancellationToken);
        if (record is null)
            return null;

        var scholar = await _context.Scholars.AsNoTracking().FirstOrDefaultAsync(s => s.Id == record.ScholarId, cancellationToken);
        var school = scholar is null ? null : await _context.Schools.AsNoTracking().FirstOrDefaultAsync(s => s.Id == scholar.SchoolId, cancellationToken);

        var lookupIds = new List<Guid>();
        if (scholar is not null) lookupIds.Add(scholar.UserId);
        if (record.ProcessedByAdminId.HasValue) lookupIds.Add(record.ProcessedByAdminId.Value);
        var users = await _context.DomainUsers.AsNoTracking().Where(u => lookupIds.Contains(u.Id)).ToListAsync(cancellationToken);

        var scholarUser = scholar is null ? null : users.FirstOrDefault(u => u.Id == scholar.UserId);
        var processor = record.ProcessedByAdminId.HasValue ? users.FirstOrDefault(u => u.Id == record.ProcessedByAdminId.Value) : null;
        var termNumber = school?.Terms.FirstOrDefault(t => t.Id == record.TermId)?.TermNumber ?? 0;

        var registrationCourses = record.ProofOfRegistration?.Courses
            .Select(c => new DetailCourseDto(c.CourseCode, c.Units)).ToList() ?? new();
        var grades = record.GradeTranscript?.CourseRecords
            .Select(g => new DetailGradeDto(g.Course.CourseCode, g.Course.Units, g.Grade)).ToList() ?? new();

        // Look up content types so the UI can render images vs PDFs inline.
        var fileIds = new List<Guid>();
        if (record.ProofOfRegistration is not null) fileIds.Add(record.ProofOfRegistration.FileId);
        if (record.GradeTranscript?.FileId is { } gtf) fileIds.Add(gtf);
        var files = await _context.StoredFiles.AsNoTracking()
            .Where(f => fileIds.Contains(f.Id))
            .Select(f => new { f.Id, f.ContentType })
            .ToListAsync(cancellationToken);
        string? Ct(Guid? id) => id is null ? null : files.FirstOrDefault(f => f.Id == id)?.ContentType;

        return new TermRecordDetailDto(
            record.Id,
            UserDisplay.NameOf(scholarUser),
            scholar?.DegreeProgram ?? string.Empty,
            scholar?.BatchNumber ?? 0,
            school?.Name ?? string.Empty,
            termNumber,
            record.Status,
            record.GradesRequired,
            record.ProcessedByAdminId.HasValue ? UserDisplay.NameOf(processor) : null,
            record.ProofOfRegistration?.FileId,
            Ct(record.ProofOfRegistration?.FileId),
            registrationCourses,
            record.GradeTranscript?.FileId,
            Ct(record.GradeTranscript?.FileId),
            record.GradeTranscript?.GWA,
            grades);
    }
}
